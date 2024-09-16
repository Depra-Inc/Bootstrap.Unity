// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.IoC.Activation;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using Depra.IoC.QoL.Scope;
using Depra.IoC.Scope;
using Depra.SerializeReference.Extensions;
using UnityEngine.SceneManagement;

namespace Depra.Bootstrap.Scenes
{
	[Serializable]
	[SerializeReferenceMenuPath(nameof(MultiSceneProject))]
	public sealed class MultiSceneProject : ICompositionRoot, IDisposable
	{
		private IScope _rootScope;

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
			if (TryFindEntryPoint(nextScene, out var entryPoint) == false)
			{
				return;
			}

			var activation = new LambdaBasedActivationBuilder();
			var containerBuilder = new ContainerBuilder(activation);
			foreach (var scope in entryPoint.LifetimeScopes)
			{
				scope.Configure(containerBuilder);
			}

			var sceneContainer = containerBuilder.Build();
			var combinedScope = new CombinedScope(_rootScope, sceneContainer.CreateScope());
			entryPoint.Compose(combinedScope);
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
		private bool TryFindEntryPoint(Scene scene, out SceneContext context)
		{
			foreach (var rootObject in scene.GetRootGameObjects())
			{
				if (rootObject.TryGetComponent(out context))
				{
					return true;
				}
			}

			context = null;
			return false;
		}

		void ICompositionRoot.Compose(IScope scope)
		{
			_rootScope = scope;
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
		}

		void IDisposable.Dispose()
		{
			SceneManager.activeSceneChanged -= OnActiveSceneChanged;
			OnSceneUnloaded(SceneManager.GetActiveScene());
		}
	}
}