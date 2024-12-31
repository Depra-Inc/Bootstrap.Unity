// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Bootstrap.Internal;
using Depra.Bootstrap.Project;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Bootstrap.Editor
{
	internal sealed class ProjectContextProvider : SettingsProvider
	{
		internal const string MENU_PATH = "Project/" + nameof(Bootstrap);
		private const SettingsScope MENU_SCOPE = SettingsScope.Project;
		private const string ABSOLUTE_PATH = "Assets/Resources/Project Context.asset";
		private static readonly string[] TABS = { "Scopes", "Composition Roots", "Misc" };

		[SettingsProvider]
		public static SettingsProvider Provide() => new ProjectContextProvider(MENU_PATH);

		internal static ProjectContext LoadOrCreate() =>
			AssetDatabase.LoadAssetAtPath<ProjectContext>(ABSOLUTE_PATH) ?? Create();

		private static ProjectContext Create()
		{
			var instance = ScriptableObject.CreateInstance<ProjectContext>();
			AssetDatabase.CreateAsset(instance, ABSOLUTE_PATH);
			AssetDatabase.SaveAssets();

			return instance;
		}

		private int _tabIndex;
		private SerializedObject _serializedSettings;

		private ProjectContextProvider(string path) : base(path, MENU_SCOPE) { }

		public override void OnActivate(string searchContext, VisualElement rootElement) =>
			_serializedSettings = new SerializedObject(LoadOrCreate());

		public override void OnGUI(string searchContext)
		{
			EditorGUILayout.Separator();

			DrawTabs();
			_serializedSettings.ApplyModifiedPropertiesWithoutUndo();

			EditorGUILayout.Separator();
			DrawButtons();
		}

		private void DrawTabs()
		{
			_tabIndex = GUILayout.Toolbar(_tabIndex, TABS);
			switch (_tabIndex)
			{
				case 0:
					EditorGUILayout.PropertyField(_serializedSettings.FindProperty("_scopes"),
						new GUIContent(TABS[0], EditorIcons.SCOPE));
					break;
				case 1:
					EditorGUILayout.PropertyField(_serializedSettings.FindProperty("_compositionRoots"),
						new GUIContent(TABS[1], EditorIcons.SCOPE));
					break;
				case 2:
					var initialScene = _serializedSettings.FindProperty("_initialScene");
					initialScene.stringValue = EditorGUILayout.TextField("Initial Scene", initialScene.stringValue);
					break;
			}
		}

		private void DrawButtons()
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Ping Context"))
			{
				EditorGUIUtility.PingObject(_serializedSettings.targetObject);
			}

			if (GUILayout.Button("Ping Entry Point"))
			{
				EditorGUIUtility.PingObject(Bootstrapper.Factory.GetOriginal());
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}