// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Bootstrap.Scenes;
using UnityEditor;
using UnityEngine;

namespace Depra.Bootstrap.Editor
{
	[CustomEditor(typeof(SceneContext))]
	internal sealed class SceneEntryPointEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var entryPoint = (SceneContext) target;
			if (GUILayout.Button(nameof(SceneContext.Refill)))
			{
				entryPoint.Refill();
				Undo.RecordObject(target, nameof(SceneContext.Refill));
				EditorUtility.SetDirty(target);
			}

			if (GUILayout.Button("Open Project Context"))
			{
				SettingsService.OpenProjectSettings(ProjectContextProvider.SETTINGS_PATH);
			}
		}
	}
}