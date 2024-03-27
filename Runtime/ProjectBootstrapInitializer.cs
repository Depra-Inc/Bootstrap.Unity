// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Bootstrap
{
	internal static class ProjectBootstrapInitializer
	{
		private const string RESOURCES_PATH = nameof(ProjectBootstrap);
		private const string LOG_CHANNEL = nameof(ProjectBootstrapInitializer);

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			var activeScene = SceneManager.GetActiveScene();
			if (activeScene.IsValid() == false)
			{
				Debug.LogWarning($"[{LOG_CHANNEL}] Active scene is not valid!");
				return;
			}

			var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			if (rootObjects.Length == 0)
			{
				Create(Load());
			}
			else
			{
				if (ContainsProjectBootstrap(rootObjects) == false)
				{
					Create(Load());
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ContainsProjectBootstrap(IEnumerable<GameObject> rootObjects)
		{
			foreach (var rootObject in rootObjects)
			{
				if (rootObject.GetComponent<ProjectBootstrap>() != null)
				{
					return true;
				}
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Create(ProjectBootstrap prefab)
		{
			if (prefab == null)
			{
				Debug.LogError($"[{LOG_CHANNEL}] Prefab not found at path: {RESOURCES_PATH}");
				return;
			}

			var instance = Object.Instantiate(prefab);
			if (instance == null)
			{
				Debug.LogError($"[{LOG_CHANNEL}] Failed to instantiate {nameof(ProjectBootstrap)} prefab!");
			}
#if DEBUG || DEV_BUILD
			instance.name = prefab.name;
#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ProjectBootstrap Load() => Resources.Load<ProjectBootstrap>(RESOURCES_PATH);
	}
}