// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Bootstrap.Project;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Bootstrap.Editor
{
	internal sealed class ProjectContextProvider : SettingsProvider
	{
		internal static readonly string SETTINGS_PATH = $"Project/{ProjectContext.RELATIVE_PATH}";

		private static readonly string[] TABS = { "Scopes", "Composition Roots" };
		private static readonly string ABSOLUTE_PATH = $"Assets/Resources/{ProjectContext.RELATIVE_PATH}.asset";

		[SettingsProvider]
		public static SettingsProvider Provide() => new ProjectContextProvider(SETTINGS_PATH);

		private static ProjectContext LoadOrCreate() =>
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

		private ProjectContextProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope) { }

		public override void OnActivate(string searchContext, VisualElement rootElement) =>
			_serializedSettings = new SerializedObject(LoadOrCreate());

		public override void OnGUI(string searchContext)
		{
			EditorGUILayout.Separator();
			DrawTabs();
			_serializedSettings.ApplyModifiedPropertiesWithoutUndo();
			if (GUILayout.Button("Select Entry Point"))
			{
				ProjectEntryPointEditor.Select();
			}
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
			}
		}
	}
}