using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(InputField))]
public class ListenForInputFieldSubmit : CustomBehaviour
{
	public StringEvent onSubmit = new StringEvent();

	InputField _field;
	public InputField field
	{
		get
		{
			if (!_field) _field = GetComponent<InputField>();
			return _field;
		}
	}

	bool focused;

	void OnEnable()
	{
		focused = false;
	}
	void Update()
	{
		if (focused && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
		{
			onSubmit.TryInvoke(field.text);
			if (field.isFocused)
				FindObjectOfType<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
		}

		focused = field.isFocused;
	}
}
