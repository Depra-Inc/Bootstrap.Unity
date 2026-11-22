// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using System;
using Depra.Bootstrap.Scenes;
using Depra.IoC.Activation;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using Depra.IoC.Scope;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Bootstrap
{
	public static class Boot
	{
		internal const int DEFAULT_ORDER = 52;
		internal const string MENU_PATH = "Depra/Bootstrap/";

		private static ProjectContext _project;

		public static void Start<TConfig>(TConfig config,
			Action<IContainerBuilder, TConfig> install,
			Action<IScope, TConfig> compose,
			Action<IScope> run)
		{
			if (config is null)
			{
				throw new ArgumentNullException(nameof(config), $"{typeof(TConfig).Name} is not found.");
			}

			if (install == null)
			{
				throw new ArgumentNullException(nameof(install));
			}

			if (compose == null)
			{
				throw new ArgumentNullException(nameof(compose));
			}

			if (run == null)
			{
				throw new ArgumentNullException(nameof(run));
			}

			var project = _project = new ProjectContext();
			Application.quitting += project.Stop;

			try
			{
				var context = CreateProjectContext();
				var builder = new ContainerBuilder(new LambdaBasedActivationBuilder());
				if (context)
				{
					context.Install(builder, parent: null);
				}

				install(builder, config);
				project.Build(builder);
				compose(project.Scope, config);
				project.ComposeScenes(context);
				project.RunAfterStartScene(run);
			}
			catch (Exception error)
			{
				Abort(project, error);
				throw;
			}
		}

		public static void Install(this IContainerBuilder builder, IInstaller installer) => installer.Install(builder);

		public static void Compose(this IScope scope, ICompositionRoot root) => _project.Compose(scope, root);

		private static SceneContext CreateProjectContext()
		{
			var settings = BootstrapSettings.Load();
			var original = settings ? settings.ProjectContext : null;
			if (!original)
			{
				return null;
			}

			var context = Object.Instantiate(original);
			context.name = original.name;
			Object.DontDestroyOnLoad(context.gameObject);

			return context;
		}

		private static void Abort(ProjectContext project, Exception error)
		{
			try
			{
				project.Stop();
			}
			catch (Exception cleanup)
			{
				throw new AggregateException(error, cleanup);
			}
		}
	}
}