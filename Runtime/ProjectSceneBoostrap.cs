// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Linq;
using Depra.IoC.Scope;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Bootstrap
{
	[DisallowMultipleComponent]
	internal sealed class ProjectSceneBoostrap : MonoBehaviour, IBootstrapElement
	{
		private IScope _scope;

		private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

		private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

		private void OnSceneLoaded(Scene nextScene, LoadSceneMode arg1)
		{
			var activeScene = SceneManager.GetActiveScene();
			if (nextScene.name != activeScene.name)
			{
				OnSceneUnloaded(activeScene);
			}

			var sceneRoots = SceneRoots(nextScene);
			foreach (var sceneRoot in sceneRoots)
			{
				foreach (var sceneBootstrap in sceneRoot.GetComponents<SceneBootstrap>())
				{
					sceneBootstrap.Initialize(_scope);
				}
			}
		}

		private void OnSceneUnloaded(Scene previousScene)
		{
			var sceneRoots = SceneRoots(previousScene);
			foreach (var sceneRoot in sceneRoots)
			{
				foreach (var sceneBootstrap in sceneRoot.GetComponents<SceneBootstrap>())
				{
					sceneBootstrap.TearDown();
				}
			}
		}

		private IEnumerable<GameObject> SceneRoots(Scene scene) => scene
			.GetRootGameObjects()
			.Where(x => x.GetComponent<SceneBootstrap>() != null);

		void IBootstrapElement.Initialize(IScope scope) => _scope = scope;
	}
}