// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using Depra.IoC;
using Depra.IoC.Activation;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using Depra.IoC.Scope;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Depra.Bootstrap.Boot;

namespace Depra.Bootstrap.Scenes
{
	[DisallowMultipleComponent]
	[AddComponentMenu(MENU_PATH + "Scene Context", DEFAULT_ORDER)]
	public sealed class SceneContext : MonoBehaviour
	{
		[SerializeField] private SceneInstaller[] _sceneInstallers;

		[SerializeField] private SceneCompositionRoot[] _compositionRoots;

		private IScope _scope;
		private IContainer _container;
		private int _composedCount;

		private void OnDestroy() => Release();

		private bool IsComposed => _scope != null;

		internal void Install(IContainerBuilder builder, IScope parent)
		{
			foreach (var installer in _sceneInstallers)
			{
				installer.Install(builder, parent);
			}
		}

		internal void Compose(IScope scope)
		{
			if (_scope != null)
			{
				throw new SceneAlreadyComposedException(gameObject.scene.path);
			}

			_scope = scope;
			foreach (var compositionRoot in _compositionRoots)
			{
				_composedCount++;
				compositionRoot.Compose(scope);
			}
		}

		internal void Release()
		{
			var errors = new List<Exception>();
			while (_composedCount > 0)
			{
				try
				{
					_compositionRoots[--_composedCount].Release();
				}
				catch (Exception exception)
				{
					errors.Add(exception);
				}
			}

			try
			{
				_scope?.Dispose();
			}
			catch (Exception exception)
			{
				errors.Add(exception);
			}

			_scope = null;
			try
			{
				_container?.Dispose();
			}
			catch (Exception exception)
			{
				errors.Add(exception);
			}

			_container = null;
			if (errors.Count != 0)
			{
				throw new AggregateException("Failed to release the scene.", errors);
			}
		}

		internal static void ComposeScene(Scene scene, IScope root)
		{
			if (!scene.isLoaded || !TryFind(scene, out var context) || context.IsComposed)
			{
				return;
			}

			// Before its objects start: what they create and look up belongs to this scene.
			SceneManager.SetActiveScene(scene);

			var builder = new ContainerBuilder(new LambdaBasedActivationBuilder());
			context.Install(builder, root);

			try
			{
				context.Compose(builder.Build(), root);
			}
			catch (Exception composeError)
			{
				try
				{
					context.Release();
				}
				catch (Exception cleanupError)
				{
					throw new AggregateException("Scene composition and cleanup failed.", composeError, cleanupError);
				}

				throw;
			}
		}

		internal static bool Spawn(SceneContext prefab, Scene scene, out SceneContext context)
		{
			context = null;
			if (!prefab)
			{
				return false;
			}

			context = Instantiate(prefab);
#if UNITY_EDITOR
			context.name = prefab.name;
#endif
			SceneManager.MoveGameObjectToScene(context.gameObject, scene);
			return true;
		}

		internal static bool TryFind(Scene scene, out SceneContext context)
		{
			context = null;
			foreach (var root in scene.GetRootGameObjects())
			{
				if (!root.TryGetComponent(out SceneContext candidate))
				{
					continue;
				}

				if (context)
				{
					throw new DuplicateSceneContextException(scene.path);
				}

				context = candidate;
			}

			return context;
		}

		private void Compose(IContainer container, IScope parent)
		{
			_container = container;
			Compose(container.CreateScope(parent));
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (transform.parent)
			{
				Debug.LogWarning($"{nameof(SceneContext)} must be on a root object of the scene.", this);
			}
		}

		internal void Refill() => _compositionRoots = gameObject.scene.GetRootGameObjects()
			.SelectMany(root => root.GetComponentsInChildren<SceneCompositionRoot>(true)).ToArray();
#endif
	}
}
