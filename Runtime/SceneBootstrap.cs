// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.Scope;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Depra.Bootstrap
{
	[DisallowMultipleComponent]
	public sealed class SceneBootstrap : MonoBehaviour, IBootstrapElement
	{
		[SerializeField] private SceneBootstrapElement[] _elements;

		private bool _needCleanup;

		private void OnDestroy()
		{
			if (_needCleanup)
			{
				TearDown();
			}
		}

		public void Initialize(IScope scope)
		{
			if (_elements.Length == 0)
			{
				return;
			}

			_needCleanup = true;
			foreach (var element in _elements)
			{
				element.Initialize(scope);
			}
		}

		internal void TearDown()
		{
			_needCleanup = false;
			foreach (var element in _elements)
			{
				element.TearDown();
			}
		}

#if UNITY_EDITOR
		[ContextMenu(nameof(Refill))]
		private void Refill()
		{
			_elements = FindObjectsOfType<SceneBootstrapElement>(false);
			EditorUtility.SetDirty(this);
		}
#endif
	}
}