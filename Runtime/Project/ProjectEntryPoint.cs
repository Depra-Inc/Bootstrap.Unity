// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using Depra.Bootstrap.Scenes;
using Depra.IoC;
using Depra.IoC.Activation;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using UnityEngine;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Project
{
	[AddComponentMenu(MENU_PATH + nameof(ProjectEntryPoint), DEFAULT_ORDER)]
	public sealed partial class ProjectEntryPoint : MonoBehaviour
	{
		[SerializeField] private bool _dontDestroyOnLoad;
		[SerializeField] private SceneCompositionRoot[] _roots;

		private IContainer _container;
		private ApplicationEntryPoint _application;

		private void Awake()
		{
			if (_dontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}

			var builder = new ContainerBuilder(new LambdaBasedActivationBuilder());
			_application = new ApplicationEntryPoint(
				lifetimeScopes: GetComponents<ILifetimeScope>(),
				compositionRoots: PrepareRoots());
			_application.Configure(builder);
			_container = builder.Build();
			_application.Compose(_container.CreateScope());
		}

		private void OnDestroy()
		{
			Array.ForEach(_roots, root => root.Release());
			_application?.Dispose();

			if (_container != null)
			{
				_container.Dispose();
				_container = null;
			}
		}

		private IEnumerable<ICompositionRoot> PrepareRoots()
		{
			var utilities = GetComponents<SceneCompositionUtility>();
			foreach (var utility in utilities)
			{
				utility.Register();
				yield return utility;
			}

			foreach (var compositionRoot in _roots)
			{
				compositionRoot.Register();
				yield return compositionRoot;
			}
		}
#if UNITY_EDITOR
		[ContextMenu(nameof(Refill))]
		private void Refill()
		{
			_roots = GetComponents<SceneCompositionRoot>();
			UnityEditor.EditorUtility.SetDirty(this);
		}
#endif
	}
}