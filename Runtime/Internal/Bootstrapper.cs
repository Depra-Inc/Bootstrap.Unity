// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.Bootstrap.Project;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using Depra.IoC.Scope;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Depra.Bootstrap.Internal
{
	internal static class Bootstrapper
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeOnLoad() => Factory.GetOrCreate().Compose();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ConfigureAll(IContainerBuilder builder, IEnumerable<ILifetimeScope> scopes)
		{
			foreach (var scope in scopes)
			{
				scope.Configure(builder);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ComposeAll(IScope scope, IEnumerable<ICompositionRoot> compositionRoots)
		{
			if (compositionRoots == null)
			{
				return;
			}

			foreach (var compositionRoot in compositionRoots)
			{
				compositionRoot.Compose(scope);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DisposeAll(IEnumerable<ICompositionRoot> compositionRoots)
		{
			if (compositionRoots == null)
			{
				return;
			}

			foreach (var compositionRoot in compositionRoots)
			{
				if (compositionRoot is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}

		internal static class Factory
		{
			private const string CONTEXT_NAME = "Project Context";
			private const string ENTRY_POINT_NAME = "Project Entry Point";
			private const string ENTRY_POINT_PATH = "Assets/Resources/" + ENTRY_POINT_NAME + ".prefab";

			public static ProjectEntryPoint GetOriginal() =>
				LoadStaticEntryPoint() ?? Create(ENTRY_POINT_PATH);

			public static ProjectEntryPoint GetOrCreate() =>
				Object.FindAnyObjectByType<ProjectEntryPoint>(FindObjectsInactive.Include) ?? Instantiate();

			public static ProjectContext LoadStaticContext()
			{
				var context = Resources.Load<ProjectContext>(CONTEXT_NAME);
				if (context == null)
				{
					Logger.Verbose($"Project context not found at '{CONTEXT_NAME}'");
				}

				return context;
			}

			private static ProjectEntryPoint LoadStaticEntryPoint() =>
				Resources.Load<ProjectEntryPoint>(ENTRY_POINT_NAME);

			private static ProjectEntryPoint Create(string relativePath)
			{
#if UNITY_EDITOR
				var objectName = UnityEditor.ObjectNames.NicifyVariableName(nameof(ProjectEntryPoint));
				var gameObject = new GameObject(objectName);
				gameObject.AddComponent<ProjectEntryPoint>();
				var original = UnityEditor.PrefabUtility.SaveAsPrefabAsset(gameObject, relativePath, out var success);
				Object.Destroy(gameObject);

				if (success)
				{
					return original.GetComponent<ProjectEntryPoint>();
				}

				Logger.Verbose($"Failed to create '{nameof(ProjectEntryPoint)}' prefab!");
#endif
				return null;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static ProjectEntryPoint Instantiate()
			{
				var original = GetOriginal();
				if (original == null)
				{
					Logger.Verbose($"Prefab not found at path: '{ENTRY_POINT_NAME}'");
					return null;
				}

				var instance = Object.Instantiate(original);
				if (instance == null)
				{
					Logger.Verbose($"Failed to instantiate '{nameof(ProjectEntryPoint)}' prefab!");
				}
#if DEBUG || DEV_BUILD
				instance.name = original.name;
#endif
				return instance;
			}
		}

		private static class Logger
		{
			public static void Verbose(string message)
			{
				const string LOG_FORMAT = "[Bootstrapper] {0}";
				Debug.LogErrorFormat(LOG_FORMAT, message);
			}
		}
	}
}