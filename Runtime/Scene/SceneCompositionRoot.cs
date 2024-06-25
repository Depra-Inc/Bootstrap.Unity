// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.IoC.Composition;
using Depra.IoC.Scope;
using UnityEngine;

namespace Depra.Bootstrap.Scene
{
	public abstract class SceneCompositionRoot : MonoBehaviour, ICompositionRoot, IDisposable
	{
		// Prevents the method from being called from outside the class.
		protected private void OnDestroy() { }

		public abstract void Compose(IScope scope);

		public virtual void Register() { }

		public virtual void Release() { }

		void IDisposable.Dispose() => Release();
	}
}