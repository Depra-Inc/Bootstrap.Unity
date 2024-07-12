// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Depra.Bootstrap.Project
{
	public static class ProjectBootstrap
	{
		private const string LOG_CHANNEL = ProjectEntryPoint.RESOURCES_PATH;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ProjectEntryPoint LoadOriginal() =>
			Resources.Load<ProjectEntryPoint>(ProjectEntryPoint.RESOURCES_PATH);

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (Object.FindAnyObjectByType<ProjectEntryPoint>(FindObjectsInactive.Include) == false)
			{
				Create(LoadOriginal());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Create(ProjectEntryPoint prefab)
		{
			if (prefab == null)
			{
				Debug.LogError($"[{LOG_CHANNEL}] Prefab not found at path: {ProjectEntryPoint.RESOURCES_PATH}");
				return;
			}

			var instance = Object.Instantiate(prefab);
			if (instance == null)
			{
				Debug.LogError($"[{LOG_CHANNEL}] Failed to instantiate {nameof(ProjectEntryPoint)} prefab!");
			}
#if DEBUG || DEV_BUILD
			instance.name = prefab.name;
#endif
		}
	}
}