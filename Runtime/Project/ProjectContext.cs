// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using Depra.Bootstrap.Scenes;
using Depra.IoC;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using Depra.IoC.Scope;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Bootstrap
{
	internal sealed class ProjectContext
	{
		private readonly List<IDisposable> _owned = new();

		private IScope _scope;
		private IContainer _container;
		private SceneContext _screenContext;
		private ISceneEntryPoint[] _entryPoints;
		private Action<IScope> _run;

		public IScope Scope => _scope;

		public void Build(ContainerBuilder builder)
		{
			_container = builder.Build();
			_scope = _container.CreateScope();
		}

		public void Compose(IScope scope, ICompositionRoot root)
		{
			if (root is IDisposable disposable)
			{
				_owned.Add(disposable);
			}

			root.Compose(scope);
		}

		public void ComposeScenes(SceneContext context)
		{
			_entryPoints = ResolveEntryPoints();
			if (context)
			{
				_owned.Add(new Releaser(context.Release));
				context.Compose(_container.CreateScope(_scope));
			}

			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public void RunAfterStartScene(Action<IScope> run)
		{
			if (SceneManager.GetActiveScene().isLoaded)
			{
				run(_scope);
				return;
			}

			_run = run;
		}

		public void Stop()
		{
			Application.quitting -= Stop;
			SceneManager.sceneLoaded -= OnSceneLoaded;
			_entryPoints = Array.Empty<ISceneEntryPoint>();
			_run = null;

			var errors = new List<Exception>();
			for (var index = _owned.Count - 1; index >= 0; index--)
			{
				Dispose(_owned[index], errors);
			}

			_owned.Clear();
			Dispose(_scope, errors);
			Dispose(_container, errors);
			_scope = null;
			_container = null;
			if (errors.Count != 0)
			{
				throw new AggregateException("Failed to stop the project.", errors);
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (SceneContext.TryFind(scene, out var screen) || TryEnter(scene, out screen))
			{
				if (_screenContext && _screenContext != screen)
				{
					try
					{
						_screenContext.Release();
					}
					catch (Exception error)
					{
						Debug.LogException(error, _screenContext);
					}
				}

				_screenContext = screen;
				SceneContext.ComposeScene(scene, _scope);
			}

			var run = _run;
			_run = null;
			run?.Invoke(_scope);
		}

		private ISceneEntryPoint[] ResolveEntryPoints() =>
			_scope.CanResolve(typeof(IEnumerable<ISceneEntryPoint>))
				? _scope.Resolve<IEnumerable<ISceneEntryPoint>>().ToArray()
				: _scope.CanResolve(typeof(ISceneEntryPoint))
					? new[] { _scope.Resolve<ISceneEntryPoint>() }
					: Array.Empty<ISceneEntryPoint>();

		private bool TryEnter(Scene scene, out SceneContext context)
		{
			foreach (var entryPoint in _entryPoints)
			{
				if (entryPoint.TryEnter(scene, out var prefab))
				{
					return SceneContext.Spawn(prefab, scene, out context);
				}
			}

			context = null;
			return false;
		}

		private static void Dispose(IDisposable disposable, ICollection<Exception> errors)
		{
			try
			{
				disposable?.Dispose();
			}
			catch (Exception error)
			{
				errors.Add(error);
			}
		}

		private sealed class Releaser : IDisposable
		{
			private readonly Action _release;

			public Releaser(Action release) => _release = release;

			void IDisposable.Dispose() => _release();
		}
	}
}