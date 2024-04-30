// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.Bootstrap.Internal
{
	internal static class Module
	{
		public const int DEFAULT_ORDER = 52;
		public const string MENU_PATH = nameof(Depra) + SLASH + nameof(Bootstrap) + SLASH;

		private const string SLASH = "/";
	}
}