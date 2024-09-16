// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.IoC.Composition;
using Depra.IoC.Scope;
using UnityEngine;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Scenes
{
	[DisallowMultipleComponent]
	[AddComponentMenu(MENU_PATH + "Scene Entry Point", DEFAULT_ORDER)]
	internal sealed class SceneEntryPoint : MonoBehaviour, IEntryPointContext
	{
		[SerializeField] private SceneContext _context;
		[SerializeField] private SceneScope[] _lifetimeScopes;
		[SerializeField] private SceneCompositionRoot[] _compositionRoots;

		private bool _needCleanup;

		public IReadOnlyCollection<ILifetimeScope> LifetimeScopes => CombineScopes();

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private IReadOnlyCollection<ILifetimeScope> CombineScopes()
		{
			if (_context == false || _context.LifetimeScopes.Count == 0)
			{
				return _lifetimeScopes;
			}

			var mergedScopes = new List<ILifetimeScope>();
			mergedScopes.AddRange(_lifetimeScopes);
			mergedScopes.AddRange(_context.LifetimeScopes);

			return mergedScopes;
		}
	}
}