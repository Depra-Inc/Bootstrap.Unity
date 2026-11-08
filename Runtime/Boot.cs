// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using System;
using Depra.Bootstrap.Scenes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Bootstrap
{
	public static class Boot
	{
		public static void Start<TConfig>(TConfig config,
			Action<ProjectInstallation, TConfig> install,
			Action<ProjectComposition, TConfig> compose)
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

			var project = new ProjectContext();
			Application.quitting += project.Stop;

			try
			{
				var context = CreateProjectContext();
				var installation = project.Enter(new ProjectInstallation(project, context));
				install(installation, config);

				var composition = installation.Complete();
				compose(composition, config);
				composition.Complete();
			}
			catch (Exception error)
			{
				Abort(project, error);
				throw;
			}
		}

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
