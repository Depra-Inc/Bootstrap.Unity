using System;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using UnityEngine;

namespace Depra.Bootstrap.Project
{
	public abstract class ProjectScope : ScriptableObject, ILifetimeScope, IDisposable
	{
		public abstract void Dispose();

		public abstract void Configure(IContainerBuilder builder);
	}
}