// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Bootstrap.Project
{
	internal static class ProjectEntryPointFactory
	{
		internal static ProjectEntryPoint Original() =>
			Load() ?? Create($"Assets/Resources/{ProjectEntryPoint.RELATIVE_PATH}.prefab");

		private static ProjectEntryPoint Load() =>
			Resources.Load<ProjectEntryPoint>(ProjectEntryPoint.RELATIVE_PATH);

		private static ProjectEntryPoint Create(string relativePath)
		{
#if UNITY_EDITOR
			var objectName = UnityEditor.ObjectNames.NicifyVariableName(nameof(ProjectEntryPoint));
			var gameObject = new GameObject(objectName);
			gameObject.AddComponent<ProjectEntryPoint>();
			var original = UnityEditor.PrefabUtility.SaveAsPrefabAsset(gameObject, relativePath, out var success);
			Object.Destroy(gameObject);

			if (success)
			{
				return original.GetComponent<ProjectEntryPoint>();
			}

			ProjectBootstrap.Logger.Verbose($"Failed to create '{nameof(ProjectEntryPoint)}' prefab!");
#endif
			return null;
		}
	}
}