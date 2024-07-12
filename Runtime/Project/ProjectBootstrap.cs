// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Depra.Bootstrap.Project
{
	public static class ProjectBootstrap
	{
		internal static ProjectEntryPoint LoadOrCreate() =>
			Resources.Load<ProjectEntryPoint>(ProjectEntryPoint.RELATIVE_PATH) ??
			CreateOriginal($"Assets/Resources/{ProjectEntryPoint.RELATIVE_PATH}.prefab");

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (Object.FindAnyObjectByType<ProjectEntryPoint>(FindObjectsInactive.Include) == false)
			{
				Instantiate(LoadOrCreate());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Instantiate(ProjectEntryPoint prefab)
		{
			if (prefab == null)
			{
				Verbose($"Prefab not found at path: '{ProjectEntryPoint.RELATIVE_PATH}'");
				return;
			}

			var instance = Object.Instantiate(prefab);
			if (instance == null)
			{
				Verbose($"Failed to instantiate '{nameof(ProjectEntryPoint)}' prefab!");
			}
#if DEBUG || DEV_BUILD
			instance.name = prefab.name;
#endif
		}

		private static ProjectEntryPoint CreateOriginal(string relativePath)
		{
#if UNITY_EDITOR
			var name = UnityEditor.ObjectNames.NicifyVariableName(nameof(ProjectEntryPoint));
			var gameObject = new GameObject(name);
			gameObject.AddComponent<ProjectEntryPoint>();
			var original = UnityEditor.PrefabUtility.SaveAsPrefabAsset(gameObject, relativePath, out var success);
			Object.Destroy(gameObject);

			if (success)
			{
				return original.GetComponent<ProjectEntryPoint>();
			}

			Verbose($"Failed to create '{nameof(ProjectEntryPoint)}' prefab!");
#endif
			return null;
		}

		private static void Verbose(string message)
		{
			const string LOG_FORMAT = "[Project Bootstrap] {0}";
			Debug.LogErrorFormat(LOG_FORMAT, message);
		}
	}
}