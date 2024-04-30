// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Depra.Bootstrap.Project
{
	public sealed partial class ProjectEntryPoint
	{
		private const string LOG_CHANNEL = nameof(ProjectEntryPoint);
		private const string RESOURCES_PATH = nameof(ProjectEntryPoint);

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (FindAnyObjectByType<ProjectEntryPoint>(FindObjectsInactive.Include) == false)
			{
				Create(Load());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Create(ProjectEntryPoint prefab)
		{
			if (prefab == null)
			{
				Debug.LogError($"[{LOG_CHANNEL}] Prefab not found at path: {RESOURCES_PATH}");
				return;
			}

			var instance = Instantiate(prefab);
			if (instance == null)
			{
				Debug.LogError($"[{LOG_CHANNEL}] Failed to instantiate {nameof(ProjectEntryPoint)} prefab!");
			}
#if DEBUG || DEV_BUILD
			instance.name = prefab.name;
#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ProjectEntryPoint Load() => Resources.Load<ProjectEntryPoint>(RESOURCES_PATH);
	}
}