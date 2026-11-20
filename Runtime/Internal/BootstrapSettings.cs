// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using Depra.Bootstrap.Scenes;
using UnityEngine;

namespace Depra.Bootstrap
{
	internal sealed class BootstrapSettings : ScriptableObject
	{
		[Tooltip("Created before the first scene loads and kept for the whole game.")]
		[SerializeField] private SceneContext _projectContext;

		internal const string RESOURCE_NAME = "Bootstrap Settings";
		internal static BootstrapSettings Load() => Resources.Load<BootstrapSettings>(RESOURCE_NAME);

		public SceneContext ProjectContext => _projectContext;
	}
}