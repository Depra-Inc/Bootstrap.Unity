// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.IoC;
using Depra.IoC.Activation;
using Depra.IoC.QoL.Builder;
using Depra.IoC.QoL.Composition;
using Depra.IoC.Scope;
using UnityEngine;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Project
{
	[DisallowMultipleComponent]
	[AddComponentMenu(MENU_PATH + nameof(ProjectEntryPoint), DEFAULT_ORDER)]
	public sealed partial class ProjectEntryPoint : MonoBehaviour, IEntryPoint, ICompositionRoot
	{
		private IContainer _container;

		private void Awake()
		{
			((ICompositionRoot) this).Register();
			Resolve(_container.CreateScope());
		}

		private void OnDestroy() => ((ICompositionRoot) this).Release();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resolve(IScope scope)
		{
			foreach (var element in GetComponents<IEntryPoint>())
			{
				element.Resolve(scope);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void ICompositionRoot.Register()
		{
			var activation = new LambdaBasedActivationBuilder();
			var builder = new ContainerBuilder(activation);
			foreach (var scope in GetComponents<ILifetimeScope>())
			{
				scope.Configure(builder);
			}

			_container = builder.Build();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void ICompositionRoot.Release()
		{
			foreach (var element in GetComponents<ICompositionRoot>())
			{
				element.Release();
			}

			if (_container == null)
			{
				return;
			}

			_container.Dispose();
			_container = null;
		}
	}
}