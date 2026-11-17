// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using System;
using Depra.Bootstrap.Scenes;
using Depra.IoC.Composition;
using Depra.IoC.Scope;

namespace Depra.Bootstrap
{
	public sealed class ProjectComposition
	{
		private readonly SceneContext _context;
		private readonly ProjectContext _project;

		private bool _scenesComposed;

		internal ProjectComposition(ProjectContext project, SceneContext context)
		{
			_context = context;
			_project = project;
		}

		public void Compose(ICompositionRoot root)
		{
			_project.EnsureCurrent(this);
			_project.Compose(root);
		}

		public void Compose(Action<IScope> compose)
		{
			_project.EnsureCurrent(this);
			_project.Compose(new DelegateCompositionRoot(compose));
		}

		public void ComposeScenes()
		{
			_project.EnsureCurrent(this);
			if (_scenesComposed)
			{
				throw new ScenesAlreadyComposedException();
			}

			_scenesComposed = true;
			_project.ComposeScenes(_context);
		}

		internal void Complete()
		{
			if (!_scenesComposed)
			{
				throw new ScenesNotComposedException();
			}

			_project.Leave(this);
		}

		private sealed class DelegateCompositionRoot : ICompositionRoot
		{
			private readonly Action<IScope> _compose;
			public DelegateCompositionRoot(Action<IScope> compose) => _compose = compose;

			void ICompositionRoot.Compose(IScope scope) => _compose(scope);
		}
	}
}
