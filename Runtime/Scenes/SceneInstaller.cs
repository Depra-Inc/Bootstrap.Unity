// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using Depra.IoC.QoL.Builder;
using Depra.IoC.Scope;
using JetBrains.Annotations;
using UnityEngine;

namespace Depra.Bootstrap.Scenes
{
	public abstract class SceneInstaller : MonoBehaviour
	{
		/// <param name="builder">Container of this scene.</param>
		/// <param name="parent">
		/// Already built project scope: resolve project services here instead of a service locator.
		/// It is <c>null</c> for the Project Context, whose installers fill the project container itself.
		/// </param>
		public abstract void Install(IContainerBuilder builder, [CanBeNull] IScope parent);
	}
}
