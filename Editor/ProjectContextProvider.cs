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
		internal static readonly string SETTINGS_PATH = $"Project/{ProjectContext.RELATIVE_PATH}";
		private static readonly string ABSOLUTE_PATH = $"Assets/Resources/{ProjectContext.RELATIVE_PATH}.asset";

		[SettingsProvider]
		public static SettingsProvider Provide() => new ProjectContextProvider(SETTINGS_PATH);

		private static SerializedObject Serialize() =>
			new(AssetDatabase.LoadAssetAtPath<ProjectContext>(ABSOLUTE_PATH) ?? Create());

		private static ProjectContext Create()
		{
			var instance = ScriptableObject.CreateInstance<ProjectContext>();
			instance._scopes = Array.Empty<ILifetimeScope>();
			AssetDatabase.CreateAsset(instance, ABSOLUTE_PATH);
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