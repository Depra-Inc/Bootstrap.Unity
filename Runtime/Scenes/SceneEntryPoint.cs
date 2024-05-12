// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using Depra.IoC.Composition;
using Depra.IoC.Scope;
using UnityEngine;
using UnityEngine.Serialization;
using static Depra.Bootstrap.Internal.Module;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Depra.Bootstrap.Scenes
{
	[DisallowMultipleComponent]
	[AddComponentMenu(MENU_PATH + nameof(SceneEntryPoint), DEFAULT_ORDER)]
	public sealed class SceneEntryPoint : MonoBehaviour, IEntryPoint
	{
		[FormerlySerializedAs("_elements")]
		[SerializeField] private SceneCompositionRoot[] _compositionRoots;

		private bool _needCleanup;

		IEnumerable<ILifetimeScope> IEntryPoint.LifetimeScopes => GetComponents<ILifetimeScope>();

		private void OnDestroy()
		{
			if (_needCleanup == false)
			{
				return;
			}

			_needCleanup = false;
			foreach (var compositionRoot in _compositionRoots)
			{
				compositionRoot.Release();
			}
		}
#if UNITY_EDITOR
		[ContextMenu(nameof(Refill))]
		private void Refill()
		{
			_compositionRoots = FindObjectsOfType<SceneCompositionRoot>(false);
			EditorUtility.SetDirty(this);
		}
#endif
		void ICompositionRoot.Compose(IScope scope)
		{
			if (_compositionRoots.Length == 0)
			{
				return;
			}

			_needCleanup = true;
			foreach (var compositionRoot in _compositionRoots)
			{
				compositionRoot.Compose(scope);
			}
		}
	}
}