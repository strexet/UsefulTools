using System;

namespace StrexetGames.UsefulTools.Runtime.DependencyInjection.Attribustes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
	public sealed class InjectAttribute : Attribute { }
}