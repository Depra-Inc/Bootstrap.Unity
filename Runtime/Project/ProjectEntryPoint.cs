// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using Depra.Bootstrap.Scene;
using Depra.IoC;
using Depra.IoC.Activation;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using UnityEngine;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Project
{
	[AddComponentMenu(MENU_PATH + RELATIVE_PATH, DEFAULT_ORDER)]
	public sealed partial class ProjectEntryPoint : MonoBehaviour
	{
		[SerializeField] private bool _dontDestroyOnLoad;
		[SerializeField] private SceneCompositionRoot[] _sceneCompositionRoots = Array.Empty<SceneCompositionRoot>();

		private IContainer _container;
		private IEnumerable<ILifetimeScope> _allScopes;
		private IEnumerable<ICompositionRoot> _allRoots;

		private void Awake()
		{
			if (_dontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}

			var builder = new ContainerBuilder(new LambdaBasedActivationBuilder());
			var projectContext = ProjectContext.Load();
			_allScopes = CollectScopes(projectContext);
			_allRoots = CollectRoots(projectContext);

			ConfigureAll(builder, _allScopes);
			_container = builder.Build();
			ComposeAll(_container.CreateScope(), _allRoots);
		}

		private void OnDestroy()
		{
			TryDispose(_allRoots);
			_allRoots = null;
			TryDispose(_allScopes);
			_allScopes = null;

			if (_container != null)
			{
				_container.Dispose();
				_container = null;
			}
		}

		private IEnumerable<ILifetimeScope> CollectScopes(IEntryPointContext context)
		{
			var sceneScopes = GetComponents<ILifetimeScope>();
			var projectScopes = context.LifetimeScopes;
			var lifetimeScopes = new List<ILifetimeScope>(projectScopes.Count + sceneScopes.Length);
			lifetimeScopes.AddRange(sceneScopes);
			lifetimeScopes.AddRange(projectScopes);

			return lifetimeScopes;
		}

		private IEnumerable<ICompositionRoot> CollectRoots(ProjectContext context)
		{
			var projectRoots = context.CompositionRoots;
			var roots = new List<ICompositionRoot>(projectRoots.Count + _sceneCompositionRoots.Length);
			roots.AddRange(projectRoots);
			roots.AddRange(_sceneCompositionRoots);

			return roots;
		}
#if UNITY_EDITOR
		[ContextMenu(nameof(Refill))]
		private void Refill()
		{
			_sceneCompositionRoots = GetComponents<SceneCompositionRoot>();
			UnityEditor.EditorUtility.SetDirty(this);
		}
#endif
	}
}