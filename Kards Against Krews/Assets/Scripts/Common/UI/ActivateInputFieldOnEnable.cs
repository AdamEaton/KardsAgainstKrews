using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Selectable))]
public class ActivateInputFieldOnEnable : CustomBehaviour
{
	InputField _field;
	public InputField field
	{
		get
		{
			if (!_field)
				_field = GetComponent<InputField>();

			return _field;
		}
	}

	void OnEnable()
	{
		field.ActivateInputField();
	}
}
