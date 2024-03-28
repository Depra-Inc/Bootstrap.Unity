// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.IoC.Scope;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Bootstrap
{
	[DisallowMultipleComponent]
	public sealed class MultiSceneBootstrap : MonoBehaviour, IBootstrapElement
	{
		private IScope _scope;

		private void Awake() => SceneManager.activeSceneChanged += OnActiveSceneChanged;

		private void OnDestroy() => SceneManager.activeSceneChanged -= OnActiveSceneChanged;

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
			if (TryFindSceneBootstrap(nextScene, out var sceneBootstrap))
			{
				sceneBootstrap.Initialize(_scope);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnSceneUnloaded(Scene previousScene)
		{
			if (TryFindSceneBootstrap(previousScene, out var sceneBootstrap))
			{
				sceneBootstrap.TearDown();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryFindSceneBootstrap(Scene scene, out SceneBootstrap sceneBootstrap)
		{
			foreach (var rootObject in scene.GetRootGameObjects())
			{
				sceneBootstrap = rootObject.GetComponent<SceneBootstrap>();
				if (sceneBootstrap != null)
				{
					return true;
				}
			}

			sceneBootstrap = null;
			return false;
		}

		void IBootstrapElement.Initialize(IScope scope) => _scope = scope;
	}
}