// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Bootstrap.Internal;
using Depra.Bootstrap.Project;
using UnityEditor;
using UnityEngine;

namespace Depra.Bootstrap.Editor
{
	[CustomEditor(typeof(ProjectEntryPoint))]
	internal sealed class ProjectEntryPointEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var entryPoint = (ProjectEntryPoint) target;
			if (GUILayout.Button(nameof(ProjectEntryPoint.Refill)))
			{
				entryPoint.Refill();
				Undo.RecordObject(target, nameof(ProjectEntryPoint.Refill));
				EditorUtility.SetDirty(target);
			}

			if (GUILayout.Button("Open Project Context"))
			{
				SettingsService.OpenProjectSettings(ProjectContextProvider.SETTINGS_PATH);
			}
		}

		internal static void Select() => Selection.activeObject = Bootstrapper.Factory.Original();
	}
}