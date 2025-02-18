using System;

namespace UsefulTools.Runtime.DependencyInjection.Attribustes
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ProvideAttribute : Attribute { }
}