// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.Scope;

namespace Depra.Bootstrap
{
	public interface IBootstrapElement
	{
		void Initialize(IScope scope);
	}
}