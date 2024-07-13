// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor;
using UnityEngine;

namespace Depra.Bootstrap.Editor
{
	internal static class EditorIcons
	{
		public static readonly Texture2D SCOPE = EditorGUIUtility.Load("d_Settings@2x") as Texture2D;
	}
}