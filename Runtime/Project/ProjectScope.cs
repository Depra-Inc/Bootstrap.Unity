using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using UnityEngine;

namespace Depra.Bootstrap.Project
{
	public abstract class ProjectScope : ScriptableObject, ILifetimeScope
	{
		public abstract void Configure(IContainerBuilder builder);
	}
}