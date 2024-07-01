using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;

namespace Depra.Bootstrap.Project
{
	public abstract class ProjectScope : ILifetimeScope
	{
		public abstract void Configure(IContainerBuilder builder);
	}
}