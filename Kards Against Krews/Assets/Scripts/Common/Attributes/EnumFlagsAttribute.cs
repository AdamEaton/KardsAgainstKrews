using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Marks an enum value as an enum mask, so it can be constructed using toggle buttons in the inspector.
/// </summary>
public class EnumFlagsAttribute : PropertyAttribute
{
	public EnumFlagsAttribute() { }
}