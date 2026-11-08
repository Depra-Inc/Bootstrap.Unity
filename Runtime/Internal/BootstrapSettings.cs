// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using Depra.Bootstrap.Scenes;
using UnityEngine;

namespace Depra.Bootstrap
{
	internal sealed class BootstrapSettings : ScriptableObject
	{
		internal const string RESOURCE_NAME = "Bootstrap Settings";

		[Tooltip("Created before the first scene loads and kept for the whole game.")]
		[SerializeField] private SceneContext _projectContext;

		public SceneContext ProjectContext => _projectContext;

		internal static BootstrapSettings Load() => Resources.Load<BootstrapSettings>(RESOURCE_NAME);
	}
}
