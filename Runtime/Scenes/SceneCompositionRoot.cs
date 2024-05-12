// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.Composition;
using Depra.IoC.Scope;
using UnityEngine;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Scenes
{
	[AddComponentMenu(MENU_PATH + nameof(SceneCompositionRoot), DEFAULT_ORDER)]
	public abstract class SceneCompositionRoot : MonoBehaviour, ICompositionRoot
	{
		protected private void OnDestroy() { }

		public abstract void Compose(IScope scope);

		public virtual void Register() { }

		public virtual void Release() { }
	}
}