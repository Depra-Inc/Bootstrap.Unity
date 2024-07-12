// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.IoC.Composition;
using Depra.IoC.Scope;
using Depra.SerializeReference.Extensions;
using UnityEngine.SceneManagement;

namespace Depra.Bootstrap.Scenes
{
	[Serializable]
	[SerializeReferenceMenuPath(nameof(MultiSceneUnity))]
	public sealed class MultiSceneUnity : ICompositionRoot, IDisposable
	{
		private IScope _scope;

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

		void ICompositionRoot.Compose(IScope scope)
		{
			_scope = scope;
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
		}

		void IDisposable.Dispose()
		{
			SceneManager.activeSceneChanged -= OnActiveSceneChanged;
			OnSceneUnloaded(SceneManager.GetActiveScene());
		}
	}
}