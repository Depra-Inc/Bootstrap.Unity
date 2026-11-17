// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using System;
using Depra.Bootstrap.Scenes;
using Depra.IoC.Activation;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;

namespace Depra.Bootstrap
{
	public sealed class ProjectInstallation
	{
		private readonly SceneContext _context;
		private readonly ProjectContext _project;
		private readonly ContainerBuilder _builder = new(new LambdaBasedActivationBuilder());

		private bool _contextInstalled;

		internal ProjectInstallation(ProjectContext project, SceneContext context)
		{
			_context = context;
			_project = project;
		}

		public void InstallProjectContext()
		{
			_project.EnsureCurrent(this);
			if (_contextInstalled)
			{
				throw new ProjectContextAlreadyInstalledException();
			}

			_contextInstalled = true;
			if (_context)
			{
				_context.Install(_builder, parent: null);
			}
		}

		public void Install(IInstaller installer)
		{
			_project.EnsureCurrent(this);
			installer.Install(_builder);
		}

		public void Install(Action<IContainerBuilder> install)
		{
			_project.EnsureCurrent(this);
			install(_builder);
		}

		internal ProjectComposition Complete()
		{
			if (_context && !_contextInstalled)
			{
				throw new ProjectContextNotInstalledException();
			}

			_project.Leave(this);
			_project.Build(_builder);
			return _project.Enter(new ProjectComposition(_project, _context));
		}
	}
}
