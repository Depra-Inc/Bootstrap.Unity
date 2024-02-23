// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.Scope;
using UnityEngine;

namespace Depra.Bootstrap
{
	public abstract class SceneBootstrap : MonoBehaviour
	{
		public abstract void Initialize(IScope scope);

		public abstract void TearDown();
	}
}