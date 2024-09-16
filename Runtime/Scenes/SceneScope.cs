// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using UnityEngine;

namespace Depra.Bootstrap.Scenes
{
	internal abstract class SceneScope : MonoBehaviour, ILifetimeScope
	{
		public abstract void Configure(IContainerBuilder builder);
	}
}