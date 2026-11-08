// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using UnityEditor;
using UnityEngine;

namespace Depra.Bootstrap.Editor
{
	internal sealed class BootstrapSettingsProvider : SettingsProvider
	{
		internal const string MENU_PATH = "Project/Bootstrap";
		private const string PROJECT_CONTEXT = "_projectContext";

		private static readonly Texture2D PREFAB_ICON = (Texture2D)EditorGUIUtility.IconContent("Prefab Icon").image;
		private static readonly Texture2D SCENE_ICON = (Texture2D)EditorGUIUtility.IconContent("SceneAsset Icon").image;

		private SerializedObject _settings;

		[SettingsProvider]
		public static SettingsProvider Provide() => new BootstrapSettingsProvider(MENU_PATH);

		private BootstrapSettingsProvider(string path) : base(path, SettingsScope.Project) { }

		public override void OnGUI(string searchContext)
		{
			_settings ??= new SerializedObject(BootstrapSettingsAsset.LoadOrCreate());

			EditorGUILayout.Space();
			DrawProjectContext();
			EditorGUILayout.Space();
			DrawColdStart();
		}

		private void DrawProjectContext()
		{
			using var _ = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);
			DrawHeading("Project Context", PREFAB_ICON);

			_settings.Update();
			EditorGUILayout.PropertyField(_settings.FindProperty(PROJECT_CONTEXT), new GUIContent("Prefab"));
			if (_settings.ApplyModifiedProperties())
			{
				AssetDatabase.SaveAssetIfDirty(_settings.targetObject);
			}

			EditorGUILayout.LabelField("Created before the first scene loads and kept for the whole game.",
				EditorStyles.wordWrappedMiniLabel);
		}

		private static void DrawColdStart()
		{
			using var _ = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);
			DrawHeading("Cold Start", SCENE_ICON);

			var path = ColdStartMenu.FirstScene;
			using (new EditorGUILayout.HorizontalScope())
			{
				using (new EditorGUI.DisabledScope(true))
				{
					EditorGUILayout.ObjectField("First Scene",
						string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(path),
						typeof(SceneAsset), false);
				}

				var playing = EditorApplication.isPlayingOrWillChangePlaymode;
				using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(path) || playing || EditorApplication.isCompiling))
				{
					if (GUILayout.Button(new GUIContent("Cold Start",
							    "Save modified scenes, open the first scene in Build Settings and enter Play Mode."),
						    GUILayout.ExpandWidth(false)))
					{
						ColdStartMenu.StartColdScene();
					}
				}
			}

			EditorGUILayout.LabelField("Starts from the first scene in Build Settings, as the build does.",
				EditorStyles.wordWrappedMiniLabel);
		}

		private static void DrawHeading(string text, Texture2D icon)
		{
			var previousSize = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(new Vector2(16, 16));
			GUILayout.Label(new GUIContent(" " + text, icon), EditorStyles.boldLabel, GUILayout.Height(20));
			EditorGUIUtility.SetIconSize(previousSize);
		}
	}
}
