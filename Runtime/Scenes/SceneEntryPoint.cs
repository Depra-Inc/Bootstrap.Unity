// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.IoC.Composition;
using Depra.IoC.Scope;
using Depra.SerializeReference.Extensions;
using UnityEngine;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Scenes
{
	[DisallowMultipleComponent]
	[AddComponentMenu(MENU_PATH + "Scene Entry Point", DEFAULT_ORDER)]
	internal sealed class SceneEntryPoint : MonoBehaviour
	{
		[SerializeField] private SceneContext _context;
		[SerializeField] private SceneCompositionRoot[] _compositionRoots;

		private bool _needCleanup;

		public IEntryPointContext Context => _context;

		public void Compose(IScope scope)
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

		public void Release()
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
		internal void Refill() => _compositionRoots = FindObjectsOfType<SceneCompositionRoot>(false);
#endif

		[Serializable]
		private sealed class SceneContext : IEntryPointContext
		{
			[SerializeField] private SceneScope[] _sceneScopes;

			[SerializeReferenceDropdown]
			[UnityEngine.SerializeReference]
			private ILifetimeScope[] _serializedScopes;

			[SerializeField] private PersistentScope[] _persistentScopes;

			IReadOnlyCollection<ILifetimeScope> IEntryPointContext.LifetimeScopes => MergeScopes();

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private IReadOnlyCollection<ILifetimeScope> MergeScopes()
			{
				var finalLength = _sceneScopes.Length + _serializedScopes.Length + _persistentScopes.Length;
				var mergedScopes = new List<ILifetimeScope>(finalLength);
				mergedScopes.AddRange(_sceneScopes);
				mergedScopes.AddRange(_serializedScopes);
				mergedScopes.AddRange(_persistentScopes);

				return mergedScopes;
			}
		}
	}
}