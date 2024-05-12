// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.IoC.Scope;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Scenes
{
	[DisallowMultipleComponent]
	[AddComponentMenu(MENU_PATH + nameof(ScenesEntryPoint), DEFAULT_ORDER)]
	public sealed class ScenesEntryPoint : SceneCompositionRoot
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
			if (TryFindEntryPoint(nextScene, out var entryPoint))
			{
				entryPoint.Compose(_scope);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnSceneUnloaded(Scene previousScene)
		{
			if (TryFindEntryPoint(previousScene, out var entryPoint))
			{
				entryPoint.Release();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryFindEntryPoint(Scene scene, out SceneEntryPoint entryPoint)
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