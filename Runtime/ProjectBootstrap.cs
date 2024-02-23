// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

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
		private IScope _scope;
		private IContainer _container;

		private void Awake()
		{
			var activation = new LambdaBasedActivationBuilder();
			var builder = new ContainerBuilder(activation);
			foreach (var element in GetComponents<IInstaller>())
			{
				element.Install(builder);
			}

			_container = builder.Build();
		}

		private void OnDestroy() => _container?.Dispose();

		private void Start()
		{
			_scope = _container.CreateScope();
			foreach (var element in GetComponents<IBootstrapElement>())
			{
				element.Initialize(_scope);
			}
		}
	}
}