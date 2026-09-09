using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(Range))]
public class RangeDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property.FindPropertyRelative("min"), label);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		position = EditorGUI.PrefixLabel(position, label);
		EditorGUI.PropertyField(position.EqualPartition(1, 0, 2, 0), property.FindPropertyRelative("min"), GUIContent.none);
		EditorGUI.PropertyField(position.EqualPartition(1, 0, 2, 1), property.FindPropertyRelative("max"), GUIContent.none);
	}
}
