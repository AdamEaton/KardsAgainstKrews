using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		int count = property.enumNames.Length;
		float height = base.GetPropertyHeight(property, label);
		
		EditorGUI.LabelField(new Rect(position.x, position.y + (position.height - height) / 2, EditorGUIUtility.labelWidth, height), label);

		for (int i = 0; i < count; i++)
		{
			Rect buttonPos = new Rect(position.x + EditorGUIUtility.labelWidth, position.y + height * (i + 0.25f), position.width - EditorGUIUtility.labelWidth, height - 1);
			if (GUI.Toggle(buttonPos, (property.intValue & 1 << i) != 0, property.enumNames[i], "Button"))
				property.intValue |= 1 << i;
			else
				property.intValue &= ~(1 << i);
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label) * (property.enumNames.Length + 0.5f);
	}
}