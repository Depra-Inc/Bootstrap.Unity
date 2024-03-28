// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.IoC;
using Depra.IoC.Activation;
using Depra.IoC.QoL.Builder;
using Depra.IoC.QoL.Composition;
using Depra.IoC.Scope;
using UnityEngine;

namespace Depra.Bootstrap
{
	[DisallowMultipleComponent]
	public sealed class ProjectBootstrap : MonoBehaviour
	{
		private IContainer _container;

		private void Awake()
		{
			var activation = new LambdaBasedActivationBuilder();
			var builder = new ContainerBuilder(activation);
			Install(builder);

			_container = builder.Build();
			Initialize(_container.CreateScope());
		}

		private void OnDestroy() => _container?.Dispose();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Install(IContainerBuilder builder)
		{
			foreach (var element in GetComponents<IInstaller>())
			{
				element.Install(builder);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Initialize(IScope scope)
		{
			foreach (var element in GetComponents<IBootstrapElement>())
			{
				element.Initialize(scope);
			}
		}
	}
}