// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using Depra.Bootstrap.Scenes;
using UnityEditor;
using UnityEngine;

namespace Depra.Bootstrap.Editor
{
	[CustomEditor(typeof(SceneContext))]
	internal sealed class SceneContextEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var context = (SceneContext)target;
			if (GUILayout.Button(nameof(SceneContext.Refill)))
			{
				context.Refill();
				Undo.RecordObject(target, nameof(SceneContext.Refill));
				EditorUtility.SetDirty(target);
			}

			if (GUILayout.Button("Open Bootstrap Settings"))
			{
				SettingsService.OpenProjectSettings(BootstrapSettingsProvider.MENU_PATH);
			}
		}
	}
}
