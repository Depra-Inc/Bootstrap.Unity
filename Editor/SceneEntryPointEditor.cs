// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Bootstrap.Scenes;
using UnityEditor;
using UnityEngine;

namespace Depra.Bootstrap.Editor
{
	[CustomEditor(typeof(SceneEntryPoint))]
	internal sealed class SceneEntryPointEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var entryPoint = (SceneEntryPoint) target;
			if (GUILayout.Button(nameof(SceneEntryPoint.Refill)))
			{
				entryPoint.Refill();
				Undo.RecordObject(target, nameof(SceneEntryPoint.Refill));
				EditorUtility.SetDirty(target);
			}

			if (GUILayout.Button("Open Project Context"))
			{
				SettingsService.OpenProjectSettings(ProjectContextProvider.MENU_PATH);
			}
		}
	}
}