// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Depra.IoC.Composition;
using Depra.IoC.Scope;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Scenes
{
	[DisallowMultipleComponent]
	[AddComponentMenu(MENU_PATH + nameof(ScenesEntryPoint), DEFAULT_ORDER)]
	public sealed class ScenesEntryPoint : MonoBehaviour, IEntryPoint
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IEntryPoint FindEntryPoint(Scene scene) => scene
			.GetRootGameObjects()
			.Select(gameObject => gameObject.GetComponent<SceneEntryPoint>())
			.Select(entryPoint => entryPoint)
			.FirstOrDefault();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void OnSceneUnloaded(Scene previousScene)
		{
			if (FindEntryPoint(previousScene) is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		private IScope _scope;

		IEnumerable<ILifetimeScope> IEntryPoint.LifetimeScopes => GetComponents<ILifetimeScope>();

		private void Start()
		{
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
		}

		private void OnDestroy()
		{
			_scope = null;
			SceneManager.activeSceneChanged -= OnActiveSceneChanged;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
			var entryPoint = FindEntryPoint(nextScene);
			entryPoint.Compose(_scope);
		}

		void ICompositionRoot.Compose(IScope scope) => _scope = scope;
	}
}