// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC;
using Depra.IoC.Activation;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using UnityEngine;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Project
{
	[AddComponentMenu(MENU_PATH + nameof(ProjectEntryPoint), DEFAULT_ORDER)]
	public sealed partial class ProjectEntryPoint : MonoBehaviour
	{
		[SerializeField] private bool _dontDestroyOnLoad;

		private IContainer _container;
		private ApplicationEntryPoint _application;

		private void Awake()
		{
			if (_dontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}

			var builder = new ContainerBuilder(new LambdaBasedActivationBuilder());
			_application = new ApplicationEntryPoint(GetComponents<IEntryPoint>());
			_application.Configure(builder, GetComponents<ILifetimeScope>());
			_container = builder.Build();
			_application.Compose(_container.CreateScope());
		}

		private void OnDestroy()
		{
			_application?.Dispose();
			if (_container != null)
			{
				_container.Dispose();
				_container = null;
			}
		}
	}
}