// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using Depra.IoC.Composition;
using Depra.SerializeReference.Extensions;
using UnityEngine;

namespace Depra.Bootstrap.Project
{
	internal sealed class ProjectContext : ScriptableObject, IEntryPointContext
	{
		[SerializeReferenceDropdown]
		[UnityEngine.SerializeReference]
		private ILifetimeScope[] _scopes = Array.Empty<ILifetimeScope>();

		[SerializeReferenceDropdown]
		[UnityEngine.SerializeReference]
		private ICompositionRoot[] _compositionRoots = Array.Empty<ICompositionRoot>();

		[SerializeField] private string _initialScene;

		public IReadOnlyCollection<ILifetimeScope> LifetimeScopes => _scopes;
		public IEnumerable<ICompositionRoot> CompositionRoots => _compositionRoots;

		internal string InitialScene => _initialScene;
	}
}