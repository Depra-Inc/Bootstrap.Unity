// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.Bootstrap.Project;
using Depra.IoC.Composition;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Bootstrap.Editor
{
	internal sealed class ProjectContextProvider : SettingsProvider
	{
		internal static readonly string SETTINGS_PATH =
			$"Project/{ObjectNames.NicifyVariableName(nameof(ProjectContext))}";

		private static readonly string ASSET_PATH =
			$"Assets/Resources/{nameof(ProjectContext)}.asset";

		[SettingsProvider]
		public static SettingsProvider Provide() => new ProjectContextProvider(SETTINGS_PATH);

		private static SerializedObject Serialize() =>
			new(AssetDatabase.LoadAssetAtPath<ProjectContext>(ASSET_PATH) ?? Create());

		private static ProjectContext Create()
		{
			var instance = ScriptableObject.CreateInstance<ProjectContext>();
			instance._scopes = Array.Empty<ILifetimeScope>();
			AssetDatabase.CreateAsset(instance, ASSET_PATH);
			AssetDatabase.SaveAssets();

			return instance;
		}

		private SerializedObject _serializedSettings;

		private ProjectContextProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope) { }

		public override void OnActivate(string searchContext, VisualElement rootElement) =>
			_serializedSettings = Serialize();

		public override void OnGUI(string searchContext)
		{
			EditorGUILayout.PropertyField(_serializedSettings.FindProperty("_scopes"));
			_serializedSettings.ApplyModifiedPropertiesWithoutUndo();

			if (GUILayout.Button("Select Entry Point"))
			{
				ProjectEntryPointEditor.Select();
			}
		}
	}
}