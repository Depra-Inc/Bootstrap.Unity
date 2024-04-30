// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.IoC.QoL.Composition;
using Depra.IoC.Scope;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Scenes
{
	[DisallowMultipleComponent]
	[AddComponentMenu(MENU_PATH + nameof(ScenesEntryPoint), DEFAULT_ORDER)]
	public sealed class ScenesEntryPoint : MonoBehaviour, ICompositionRoot, IEntryPoint
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ICompositionRoot FindCompositionRoot(Scene scene)
		{
			foreach (var gameObject in scene.GetRootGameObjects())
			{
				var sceneRoot = gameObject.GetComponent<SceneEntryPoint>();
				return sceneRoot == null ? new NullCompositionRoot() : sceneRoot;
			}

			return new NullCompositionRoot();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void OnSceneUnloaded(Scene previousScene) => FindCompositionRoot(previousScene).Release();

		private IScope _scope;

		private void Awake() => ((ICompositionRoot) this).Register();

		private void OnDestroy() => ((ICompositionRoot) this).Release();

		public void Resolve(IScope scope) => _scope = scope;

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
			var sceneRoot = FindCompositionRoot(nextScene);
			sceneRoot.Register();

			if (sceneRoot is IEntryPoint sceneEntryPoint)
			{
				sceneEntryPoint.Resolve(_scope);
			}
		}

		void ICompositionRoot.Register()
		{
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
		}

		void ICompositionRoot.Release()
		{
			_scope = null;
			SceneManager.activeSceneChanged -= OnActiveSceneChanged;
		}
	}
}