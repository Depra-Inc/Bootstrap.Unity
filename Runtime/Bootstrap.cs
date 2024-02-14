// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC;
using Depra.IoC.Activation;
using Depra.IoC.QoL.Builder;
using UnityEngine;

namespace Depra.Bootstrap
{
	[DisallowMultipleComponent]
	public sealed class Bootstrap : MonoBehaviour
	{
		private IContainer _container;
		private IBootstrapElement[] _elements;

		private void Awake()
		{
			var activation = new LambdaBasedActivationBuilder();
			var builder = new ContainerBuilder(activation);

			_elements = GetComponents<IBootstrapElement>();
			foreach (var element in _elements)
			{
				element.PreInitialize(builder);
			}

			_container = builder.Build();
		}

		private void Start()
		{
			var scope = _container.CreateScope();
			foreach (var element in _elements)
			{
				element.Initialize(scope);
			}
		}

		private void OnDestroy() => _container?.Dispose();
	}
}