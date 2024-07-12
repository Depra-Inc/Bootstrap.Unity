// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using Depra.Bootstrap.Scenes;
using Depra.IoC;
using Depra.IoC.Activation;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using Depra.IoC.Scope;
using UnityEngine;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Project
{
	[AddComponentMenu(MENU_PATH + RELATIVE_PATH, DEFAULT_ORDER)]
	public sealed class ProjectEntryPoint : MonoBehaviour
	{
		[SerializeField] private bool _dontDestroyOnLoad;
		[SerializeField] private SceneCompositionRoot[] _sceneCompositionRoots;

		internal const string RELATIVE_PATH = "Project Entry Point";

		private IContainer _container;
		private IEnumerable<ILifetimeScope> _allScopes;
		private IEnumerable<ICompositionRoot> _allRoots;

		private void Start()
		{
			if (_dontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}

			var builder = new ContainerBuilder(new LambdaBasedActivationBuilder());
			var projectContext = ProjectContext.Load();
			_allScopes = projectContext.LifetimeScopes;
			_allRoots = CollectRoots(projectContext);

			ConfigureAll(builder, _allScopes);
			_container = builder.Build();
			ComposeAll(_container.CreateScope(), _allRoots);
		}

		private void OnDestroy()
		{
			foreach (var item in _allRoots)
			{
				if (item is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}

			_allRoots = null;
			_allScopes = null;

			if (_container != null)
			{
				_container.Dispose();
				_container = null;
			}
		}

		private IEnumerable<ICompositionRoot> CollectRoots(ProjectContext context)
		{
			var projectRoots = context.CompositionRoots;
			var roots = new List<ICompositionRoot>(projectRoots.Count + _sceneCompositionRoots.Length);
			roots.AddRange(projectRoots);
			roots.AddRange(_sceneCompositionRoots);

			return roots;
		}

		private void ConfigureAll(IContainerBuilder builder, IEnumerable<ILifetimeScope> scopes)
		{
			foreach (var scope in scopes)
			{
				scope.Configure(builder);
			}
		}

		private void ComposeAll(IScope scope, IEnumerable<ICompositionRoot> compositionRoots)
		{
			foreach (var compositionRoot in compositionRoots)
			{
				compositionRoot.Compose(scope);
			}
		}

#if UNITY_EDITOR
		internal void Refill() => _sceneCompositionRoots = GetComponents<SceneCompositionRoot>();
#endif
	}
}