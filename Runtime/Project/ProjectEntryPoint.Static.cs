// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.IoC.Composition;
using Depra.IoC.QoL.Builder;
using Depra.IoC.Scope;

namespace Depra.Bootstrap.Project
{
	public sealed partial class ProjectEntryPoint
	{
		internal const string RELATIVE_PATH = "Project Entry Point";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void TryDispose(IEnumerable collection)
		{
			foreach (var item in collection)
			{
				if (item is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ConfigureAll(IContainerBuilder builder, IEnumerable<ILifetimeScope> scopes)
		{
			foreach (var scope in scopes)
			{
				scope.Configure(builder);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ComposeAll(IScope scope, IEnumerable<ICompositionRoot> compositionRoots)
		{
			foreach (var compositionRoot in compositionRoots)
			{
				compositionRoot.Compose(scope);
			}
		}
	}
}