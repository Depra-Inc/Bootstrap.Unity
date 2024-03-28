﻿// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.Scope;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Depra.Bootstrap
{
	[DisallowMultipleComponent]
	public sealed class SceneBootstrap : MonoBehaviour
	{
		[SerializeField] private SceneBootstrapElement[] _elements;

		internal void Initialize(IScope scope)
		{
			foreach (var element in _elements)
			{
				element.Initialize(scope);
			}
		}

		internal void TearDown()
		{
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