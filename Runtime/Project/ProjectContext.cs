// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
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

		private object _stage;
		private IScope _scope;
		private IContainer _container;
		private List<ICompositionRoot> _waitingForStartScene;
		private SceneContext _screen;

		public T Enter<T>(T stage)
		{
			_stage = stage;
			return stage;
		}

		public void Leave(object stage)
		{
			EnsureCurrent(stage);
			_stage = null;
		}

		public void EnsureCurrent(object stage)
		{
			if (_stage != stage)
			{
				throw new StageCompletedException(stage.GetType().Name);
			}
		}

		public void Build(ContainerBuilder builder)
		{
			_container = builder.Build();
			_scope = _container.CreateScope();
		}

		public void Compose(ICompositionRoot root)
		{
			if (root is IDisposable disposable)
			{
				_owned.Add(disposable);
			}

			if (_waitingForStartScene != null)
			{
				_waitingForStartScene.Add(root);
				return;
			}

			root.Compose(_scope);
		}

		public void ComposeScenes(SceneContext context)
		{
			if (context)
			{
				_owned.Add(new Releaser(context.Release));
				context.Compose(_container.CreateScope(_scope));
			}

			SceneManager.sceneLoaded += OnSceneLoaded;

			// Built before the first scene loads: roots composed below wait for it.
			if (!SceneManager.GetActiveScene().isLoaded)
			{
				_waitingForStartScene = new List<ICompositionRoot>();
			}
		}

		public void Stop()
		{
			Application.quitting -= Stop;
			SceneManager.sceneLoaded -= OnSceneLoaded;
			_stage = null;
			_waitingForStartScene = null;

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
			// Release the previous screen first: screens share the ECS worlds and the service locator.
			if (SceneContext.TryFind(scene, out var screen))
			{
				if (_screen && _screen != screen)
				{
					try
					{
						_screen.Release();
					}
					catch (Exception error)
					{
						Debug.LogException(error, _screen);
					}
				}

				_screen = screen;
				SceneContext.ComposeScene(scene, _scope);
			}

			if (_waitingForStartScene == null)
			{
				return;
			}

			var roots = _waitingForStartScene;
			_waitingForStartScene = null;
			foreach (var root in roots)
			{
				root.Compose(_scope);
			}
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
