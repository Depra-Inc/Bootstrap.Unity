// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using UnityEditor;
using UnityEngine;

namespace Depra.Bootstrap.Editor
{
	internal static class BootstrapSettingsAsset
	{
		private const string ASSET_PATH = "Assets/Resources/" + BootstrapSettings.RESOURCE_NAME + ".asset";

		public static BootstrapSettings Load() => Resources.Load<BootstrapSettings>(BootstrapSettings.RESOURCE_NAME);

		public static BootstrapSettings LoadOrCreate()
		{
			var settings = Load();
			if (settings)
			{
				return settings;
			}

			if (!AssetDatabase.IsValidFolder("Assets/Resources"))
			{
				AssetDatabase.CreateFolder("Assets", "Resources");
			}

			settings = ScriptableObject.CreateInstance<BootstrapSettings>();
			AssetDatabase.CreateAsset(settings, ASSET_PATH);
			AssetDatabase.SaveAssets();
			return settings;
		}
	}
}
