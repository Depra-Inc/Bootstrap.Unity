// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.QoL.Builder;
using Depra.IoC.Scope;

namespace Depra.Bootstrap
{
	public interface IBootstrapElement
	{
		void PreInitialize(IContainerBuilder container);

		void Initialize(IScope scope);
	}
}