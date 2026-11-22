// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using System;

namespace Depra.Bootstrap
{
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
