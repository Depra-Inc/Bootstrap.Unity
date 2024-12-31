// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Depra.Bootstrap.Editor
{
	public static class ColdStartFeature
	{
		[MenuItem("Bootstrap/Start Cold Scene")]
		[Obsolete("Obsolete")]
		public static void StartColdScene()
		{
			var projectContext = ProjectContextProvider.LoadOrCreate();
			var sceneName = projectContext.InitialScene;
			SceneHelper.StartScene(sceneName);

			Debug.Log("Cold start initiated with scene: " + sceneName, projectContext);
		}

		private static class SceneHelper
		{
			private static string _sceneToOpen;

			public static void StartScene(string sceneName)
			{
				if (EditorApplication.isPlaying)
				{
					EditorApplication.isPlaying = false;
				}

				_sceneToOpen = sceneName;
				EditorApplication.update += OnUpdate;
			}

			private static void OnUpdate()
			{
				if (string.IsNullOrEmpty(_sceneToOpen) ||
				    EditorApplication.isPlaying || EditorApplication.isPaused ||
				    EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
				{
					return;
				}

				EditorApplication.update -= OnUpdate;

				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					var scenePath = GetScenePathFromAssetDatabase();
					if (string.IsNullOrEmpty(scenePath))
					{
						Debug.LogWarning($"Couldn't find scene file '{_sceneToOpen}'");
						return;
					}

					EditorSceneManager.OpenScene(scenePath);
					EditorApplication.isPlaying = true;
				}

				_sceneToOpen = null;
			}

			private static string GetScenePathFromAssetDatabase()
			{
				// Need to get scene via search because the path to the scene
				// file contains the package version so it'll change over time.
				var guids = AssetDatabase.FindAssets("t:scene " + _sceneToOpen, null);
				return guids.Length != 0 ? AssetDatabase.GUIDToAssetPath(guids[0]) : null;
			}
		}
	}
}