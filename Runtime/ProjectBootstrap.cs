// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC;
using Depra.IoC.Activation;
using Depra.IoC.QoL.Builder;
using Depra.IoC.Scope;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Bootstrap
{
	[DisallowMultipleComponent]
	public sealed class ProjectBootstrap : MonoBehaviour
	{
		private IScope _scope;
		private IContainer _container;
		private IBootstrapElement[] _elements;

		private void Awake()
		{
			var activation = new LambdaBasedActivationBuilder();
			var builder = new ContainerBuilder(activation);

			_elements = GetComponents<IBootstrapElement>();
			foreach (var element in _elements)
			{
				element.InstallBindings(builder);
			}

			_container = builder.Build();
		}

		private void Start()
		{
			_scope = _container.CreateScope();
			foreach (var element in _elements)
			{
				element.Initialize(_scope);
			}
		}

		private void OnDestroy() => _container?.Dispose();

		private void OnEnable() => SceneManager.activeSceneChanged += OnActiveSceneChanged;

		private void OnDisable() => SceneManager.activeSceneChanged -= OnActiveSceneChanged;

		private void OnActiveSceneChanged(Scene arg0, Scene arg1)
		{
			if (arg0.name != SceneManager.GetActiveScene().name)
			{
				return;
			}

			var sceneBootstraps = FindObjectsByType<SceneBootstrap>(FindObjectsSortMode.None);
			foreach (var sceneBootstrap in sceneBootstraps)
			{
				sceneBootstrap.Initialize(_scope);
			}
		}
	}
}