// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Bootstrap.Scenes;
using Depra.IoC;
using Depra.IoC.Activation;
using Depra.IoC.QoL.Builder;
using UnityEngine;
using static Depra.Bootstrap.Internal.Bootstrapper;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Project
{
	[AddComponentMenu(MENU_PATH + "Project Entry Point", DEFAULT_ORDER)]
	public sealed class ProjectEntryPoint : MonoBehaviour
	{
		[SerializeField] private bool _dontDestroyOnLoad;
		[SerializeField] private SceneScope[] _sceneScopes;
		[SerializeField] private SceneCompositionRoot[] _sceneCompositionRoots;

		private IContainer _container;
		private ProjectContext _context;

		internal void Compose()
		{
			if (_dontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}

			var builder = new ContainerBuilder(new LambdaBasedActivationBuilder());

			_context = Factory.LoadStaticContext();
			ConfigureAll(builder, _context.LifetimeScopes);
			ConfigureAll(builder, _sceneScopes);

			_container = builder.Build();
			var scope = _container.CreateScope();
			ComposeAll(scope, _context.CompositionRoots);
			ComposeAll(scope, _sceneCompositionRoots);
		}

		private void OnDestroy()
		{
			DisposeAll(_sceneCompositionRoots);
			if (_context)
			{
				DisposeAll(_context.CompositionRoots);
				_context = null;
			}

			if (_container != null)
			{
				_container.Dispose();
				_container = null;
			}
		}

#if UNITY_EDITOR
		internal void Refill() => _sceneCompositionRoots = GetComponents<SceneCompositionRoot>();
#endif
	}
}