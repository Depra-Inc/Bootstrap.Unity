using System.Collections.Generic;
using Depra.Bootstrap.Internal;
using Depra.IoC.Composition;
using Depra.SerializeReference.Extensions;
using UnityEngine;

namespace Depra.Bootstrap.Scenes
{
	[CreateAssetMenu(menuName = Module.MENU_PATH + "Scene Context", order = Module.DEFAULT_ORDER)]
	internal sealed class SceneContext : ScriptableObject, IEntryPointContext
	{
		[SerializeReferenceDropdown]
		[UnityEngine.SerializeReference]
		private ILifetimeScope[] _scopes;

		public IReadOnlyCollection<ILifetimeScope> LifetimeScopes => _scopes;
	}
}