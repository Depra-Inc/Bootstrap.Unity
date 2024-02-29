// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Depra.IoC.Scope;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Bootstrap
{
	[DisallowMultipleComponent]
	internal sealed class ProjectSceneBoostrap : MonoBehaviour, IBootstrapElement
	{
		private IScope _scope;

		private void OnEnable() => SceneManager.activeSceneChanged += OnActiveSceneChanged;

		private void OnDisable() => SceneManager.activeSceneChanged -= OnActiveSceneChanged;

		private void OnActiveSceneChanged(Scene arg0, Scene arg1)
		{
			if (arg0.IsValid())
			{
				OnSceneUnloaded(arg0);
			}

			OnSceneLoaded(arg1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnSceneLoaded(Scene nextScene)
		{
			var sceneRoots = SceneRoots(nextScene);
			foreach (var sceneRoot in sceneRoots)
			{
				foreach (var sceneBootstrap in sceneRoot.GetComponents<SceneBootstrap>())
				{
					sceneBootstrap.Initialize(_scope);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private IEnumerable<GameObject> SceneRoots(Scene scene) => scene
			.GetRootGameObjects()
			.Where(x => x.GetComponent<SceneBootstrap>() != null);

		void IBootstrapElement.Initialize(IScope scope) => _scope = scope;
	}
}