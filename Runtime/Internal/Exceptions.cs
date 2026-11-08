// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using System;

namespace Depra.Bootstrap
{
	internal sealed class StageCompletedException : InvalidOperationException
	{
		public StageCompletedException(string stage) :
			base($"{stage} is already completed; use it only inside its Boot.Start callback.") { }
	}

	internal sealed class ProjectContextNotInstalledException : InvalidOperationException
	{
		public ProjectContextNotInstalledException() : base(
			"Project Context is not installed. Call project.InstallProjectContext() in Install.") { }
	}

	internal sealed class ProjectContextAlreadyInstalledException : InvalidOperationException
	{
		public ProjectContextAlreadyInstalledException() : base("Project Context is already installed.") { }
	}

	internal sealed class ScenesNotComposedException : InvalidOperationException
	{
		public ScenesNotComposedException() : base(
			"Scenes are not composed. Call project.ComposeScenes() at the end of Compose.") { }
	}

	internal sealed class ScenesAlreadyComposedException : InvalidOperationException
	{
		public ScenesAlreadyComposedException() : base(
			"Scenes are already composed: call project.ComposeScenes() once.") { }
	}

	internal sealed class DuplicateSceneContextException : InvalidOperationException
	{
		public DuplicateSceneContextException(string scene) : base(
			$"Scene '{scene}' has more than one Scene Context.") { }
	}

	internal sealed class SceneAlreadyComposedException : InvalidOperationException
	{
		public SceneAlreadyComposedException(string scene) : base($"Scene '{scene}' has already been composed.") { }
	}
}