using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extension class for GameObject
/// </summary>
public static class GameObjectExt
{
	/// <summary>
	/// Adds a Component of type T to the GameObject if one doesn't exist, then returns it.
	/// </summary>
	/// <typeparam name="T">The type of Component to get or add.</typeparam>
	/// <param name="gameObject">The GameObject to get or add to.</param>
	/// <returns>The Component of type T attached to the GameObject, which will be added if none exists.</returns>
	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
	{
		T output = gameObject.GetComponent<T>();
		if (output == null)
			output = gameObject.AddComponent<T>();
		return output;
	}
}