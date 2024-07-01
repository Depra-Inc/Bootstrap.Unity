// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.IoC.Scope;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Scene
{
	[DisallowMultipleComponent]
	[AddComponentMenu(MENU_PATH + nameof(MultiSceneEntryPointSupport), DEFAULT_ORDER)]
	internal sealed class MultiSceneEntryPointSupport : SceneCompositionUtility
	{
		private IScope _scope;

		public override void Compose(IScope scope) => _scope = scope;

		public override void Register()
		{
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
		}

		public override void Release()
		{
			SceneManager.activeSceneChanged -= OnActiveSceneChanged;
			OnSceneUnloaded(SceneManager.GetActiveScene());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
		{
			if (arg0.IsValid())
			{
				OnSceneUnloaded(arg0);
			}

			OnSceneLoaded(arg1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnSceneLoaded(UnityEngine.SceneManagement.Scene nextScene)
		{
			if (TryFindEntryPoint(nextScene, out var entryPoint))
			{
				entryPoint.Compose(_scope);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnSceneUnloaded(UnityEngine.SceneManagement.Scene previousScene)
		{
			if (TryFindEntryPoint(previousScene, out var entryPoint))
			{
				entryPoint.Release();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryFindEntryPoint(UnityEngine.SceneManagement.Scene scene, out SceneEntryPoint entryPoint)
		{
			foreach (var rootObject in scene.GetRootGameObjects())
			{
				if (rootObject.TryGetComponent(out entryPoint))
				{
					return true;
				}
			}

			entryPoint = null;
			return false;
		}
	}
}