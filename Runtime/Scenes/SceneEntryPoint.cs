// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.QoL.Composition;
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
	public sealed class SceneEntryPoint : MonoBehaviour, ICompositionRoot, IEntryPoint
	{
		[FormerlySerializedAs("_elements")]
		[SerializeField] private SceneCompositionRoot[] _compositionRoots;

		private bool _needCleanup;
#if UNITY_EDITOR
		[ContextMenu(nameof(Refill))]
		private void Refill()
		{
			_compositionRoots = FindObjectsOfType<SceneCompositionRoot>(false);
			EditorUtility.SetDirty(this);
		}
#endif
		private void OnDestroy() => ((ICompositionRoot) this).Release();

		public void Resolve(IScope scope)
		{
			if (_compositionRoots.Length == 0)
			{
				return;
			}

			_needCleanup = true;
			foreach (var compositionRoot in _compositionRoots)
			{
				compositionRoot.Resolve(scope);
			}
		}

		void ICompositionRoot.Register() { }

		void ICompositionRoot.Release()
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
	}
}