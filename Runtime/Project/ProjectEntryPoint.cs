// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using Depra.Bootstrap.Scene;
using Depra.IoC;
using Depra.IoC.Activation;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using static Depra.Bootstrap.Internal.Module;

namespace Depra.Bootstrap.Project
{
	[AddComponentMenu(MENU_PATH + nameof(ProjectEntryPoint), DEFAULT_ORDER)]
	public sealed partial class ProjectEntryPoint : MonoBehaviour
	{
		[SerializeField] private bool _dontDestroyOnLoad;
		[SerializeField] private ProjectScope[] _projectScopes;

		[FormerlySerializedAs("_roots")]
		[SerializeField] private SceneCompositionRoot[] _sceneCompositionRoots;

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
				lifetimeScopes: CollectScopes(),
				compositionRoots: PrepareRoots());
			_application.Configure(builder);
			_container = builder.Build();
			_application.Compose(_container.CreateScope());
		}

		private void OnDestroy()
		{
			foreach (var compositionRoot in _sceneCompositionRoots)
			{
				compositionRoot.Release();
			}

			foreach (var scope in _projectScopes)
			{
				scope.Dispose();
			}

			_application?.Dispose();
			if (_container != null)
			{
				_container.Dispose();
				_container = null;
			}
		}

		private IEnumerable<ILifetimeScope> CollectScopes()
		{
			var sceneScopes = GetComponents<ILifetimeScope>();
			var lifetimeScopes = new List<ILifetimeScope>(_projectScopes.Length + sceneScopes.Length);
			lifetimeScopes.AddRange(_projectScopes);
			lifetimeScopes.AddRange(sceneScopes);

			return lifetimeScopes;
		}

		private IEnumerable<ICompositionRoot> PrepareRoots()
		{
			var utilities = GetComponents<SceneCompositionUtility>();
			foreach (var utility in utilities)
			{
				utility.Register();
				yield return utility;
			}

			foreach (var compositionRoot in _sceneCompositionRoots)
			{
				compositionRoot.Register();
				yield return compositionRoot;
			}
		}
#if UNITY_EDITOR
		[ContextMenu(nameof(Refill))]
		private void Refill()
		{
			_sceneCompositionRoots = GetComponents<SceneCompositionRoot>();
			EditorUtility.SetDirty(this);
		}
#endif
	}
}