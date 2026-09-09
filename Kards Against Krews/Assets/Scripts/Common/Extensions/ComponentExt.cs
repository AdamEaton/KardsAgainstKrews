using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extension class for Component.
/// </summary>
public static class ComponentExt
{
	/// <summary>
	/// Adds a Component of type T to the Component's GameObject if one doesn't exist, then returns it.
	/// </summary>
	/// <typeparam name="T">The type of Component to get or add.</typeparam>
	/// <param name="component">The Component to get or add to the GameObject of.</param>
	/// <returns>The Component of type T attached to the Component's GameObject, which will be added if none exists.</returns>
	public static T GetOrAddComponent<T>(this Component component) where T : Component
	{
		return component.gameObject.GetOrAddComponent<T>();
	}
}