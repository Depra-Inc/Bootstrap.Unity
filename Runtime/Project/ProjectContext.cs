// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using Depra.IoC.Composition;
using Depra.SerializeReference.Extensions;
using UnityEngine;

namespace Depra.Bootstrap.Project
{
	public sealed class ProjectContext : ScriptableObject, IEntryPointContext
	{
		[SerializeReferenceDropdown]
		[UnityEngine.SerializeReference]
		private ILifetimeScope[] _scopes = Array.Empty<ILifetimeScope>();

		[SerializeReferenceDropdown]
		[UnityEngine.SerializeReference]
		private ICompositionRoot[] _compositionRoots = Array.Empty<ICompositionRoot>();

		internal const string RELATIVE_PATH = "Project Context";

		internal static ProjectContext Load()
		{
			var context = Resources.Load<ProjectContext>(RELATIVE_PATH);
			if (context == null)
			{
				Debug.LogError($"Project context not found at {RELATIVE_PATH}");
			}

			return context;
		}

		public IReadOnlyCollection<ILifetimeScope> LifetimeScopes => _scopes;
		public IReadOnlyCollection<ICompositionRoot> CompositionRoots => _compositionRoots;
	}
}