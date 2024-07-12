// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Depra.Bootstrap.Project
{
	internal static class ProjectBootstrap
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (Object.FindAnyObjectByType<ProjectEntryPoint>(FindObjectsInactive.Include) == false)
			{
				Instantiate(ProjectEntryPointFactory.Original());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Instantiate(ProjectEntryPoint prefab)
		{
			if (prefab == null)
			{
				Logger.Verbose($"Prefab not found at path: '{ProjectEntryPoint.RELATIVE_PATH}'");
				return;
			}

			var instance = Object.Instantiate(prefab);
			if (instance == null)
			{
				Logger.Verbose($"Failed to instantiate '{nameof(ProjectEntryPoint)}' prefab!");
			}
#if DEBUG || DEV_BUILD
			instance.name = prefab.name;
#endif
		}

		internal static class Logger
		{
			public static void Verbose(string message)
			{
				const string LOG_FORMAT = "[Project Bootstrap] {0}";
				Debug.LogErrorFormat(LOG_FORMAT, message);
			}
		}
	}
}