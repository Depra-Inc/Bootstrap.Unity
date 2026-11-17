// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using UnityEngine.SceneManagement;

namespace Depra.Bootstrap.Scenes
{
	public interface ISceneEntryPoint
	{
		bool TryEnter(Scene scene, out SceneContext context);
	}
}
