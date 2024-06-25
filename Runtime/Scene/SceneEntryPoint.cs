// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.Scope;
using UnityEngine;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Scene
{
	[DisallowMultipleComponent]
	[AddComponentMenu(MENU_PATH + nameof(SceneEntryPoint), DEFAULT_ORDER)]
	internal sealed class SceneEntryPoint : SceneCompositionUtility
	{
		[SerializeField] private SceneCompositionRoot[] _roots;

		private bool _needCleanup;

		public override void Compose(IScope scope)
		{
			if (_roots.Length == 0)
			{
				return;
			}

			_needCleanup = true;
			foreach (var compositionRoot in _roots)
			{
				compositionRoot.Compose(scope);
			}
		}

		public override void Release()
		{
			if (_needCleanup == false)
			{
				return;
			}

			_needCleanup = false;
			foreach (var compositionRoot in _roots)
			{
				compositionRoot.Release();
			}
		}
#if UNITY_EDITOR
		[ContextMenu(nameof(Refill))]
		private void Refill()
		{
			_roots = FindObjectsOfType<SceneCompositionRoot>(false);
			_roots = System.Array.FindAll(_roots, root => root != this);
			UnityEditor.EditorUtility.SetDirty(this);
		}
#endif
	}
}