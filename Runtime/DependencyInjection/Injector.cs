using StrexetGames.UsefulTools.Runtime.DependencyInjection.Attribustes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UsefulTools.Runtime;

namespace StrexetGames.UsefulTools.Runtime.DependencyInjection
{
	[DefaultExecutionOrder(-1000)]
	public class Injector : Singleton<Injector>
	{
		private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		private Dictionary<Type, object> registry;

		private static bool IsInjectable(MonoBehaviour monoBehaviour)
		{
			var members = monoBehaviour.GetType().GetMembers(BINDING_FLAGS);
			return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
		}

		override protected void Initialization()
		{
			registry = new Dictionary<Type, object>();

			// Find all modules implementing IDependencyProvider
			var providers = FindAllMonoBehaviours().OfType<IDependencyProvider>();

			foreach (var provider in providers)
			{
				RegisterProvider(provider);
			}

			// Find all injectable objects and inject their dependencies
			var injectables = FindAllMonoBehaviours().Where(IsInjectable);

			foreach (var injectable in injectables)
			{
				InjectInto(injectable);
			}
		}

		private void InjectInto(object instance)
		{
			var type = instance.GetType();
			var injectableFields = type.GetFields(BINDING_FLAGS)
			   .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
			var injectableProperties = type.GetProperties(BINDING_FLAGS)
			   .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
			var injectableMethods = type.GetMethods(BINDING_FLAGS)
			   .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

			foreach (var field in injectableFields)
			{
				var fieldType = field.FieldType;
				var resolvedInstance = Resolve(fieldType);

				if (resolvedInstance == null)
				{
					throw new Exception($"Failed to resolve {fieldType.Name} for {type.Name}.{field.Name}");
				}

				field.SetValue(instance, resolvedInstance);
			}

			foreach (var property in injectableProperties)
			{
				var propertyType = property.PropertyType;
				var resolvedInstance = Resolve(propertyType);

				if (resolvedInstance == null)
				{
					throw new Exception($"Failed to resolve {propertyType.Name} for {type.Name}.{property.Name}");
				}

				property.SetValue(instance, resolvedInstance);
			}

			foreach (var method in injectableMethods)
			{
				var requiredParameters = method.GetParameters()
				   .Select(parameter => parameter.ParameterType)
				   .ToArray();

				var resolvedInstances = requiredParameters.Select(Resolve).ToArray();

				if (resolvedInstances.Any(instance => instance == null))
				{
					throw new Exception($"Failed to resolve parameters for {type.Name}.{method.Name}");
				}

				method.Invoke(instance, resolvedInstances);
			}
		}

		private object Resolve(Type type) => registry.GetValueOrDefault(type);


		private void RegisterProvider(IDependencyProvider provider)
		{
			var methods = provider.GetType().GetMethods(BINDING_FLAGS);

			foreach (var method in methods)
			{
				if (!Attribute.IsDefined(method, typeof(ProvideAttribute)))
				{
					continue;
				}

				var returnType = method.ReturnType;
				var providedInstance = method.Invoke(provider, null);

				if (providedInstance != null)
				{
					registry.Add(returnType, providedInstance);
				}
				else
				{
					throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}");
				}
			}
		}

		private MonoBehaviour[] FindAllMonoBehaviours() => FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
	}
}