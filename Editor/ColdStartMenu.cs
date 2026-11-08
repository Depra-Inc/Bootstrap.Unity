// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Depra.Bootstrap.Editor
{
	internal static class ColdStartMenu
	{
		public static string FirstScene => EditorBuildSettings.scenes.FirstOrDefault(scene => scene.enabled)?.path;

		[MenuItem("Bootstrap/Cold Start")]
		public static void StartColdScene()
		{
			var path = FirstScene;
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogWarning("Build Settings have no scenes: add the first scene of the game there.");
				return;
			}

			EditorApplication.isPlaying = false;
			EditorApplication.delayCall += () => EnterPlayMode(path);
		}

		private static void EnterPlayMode(string path)
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.delayCall += () => EnterPlayMode(path);
				return;
			}

			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene(path);
				EditorApplication.isPlaying = true;
			}
		}
	}
}
