using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ClipboardUtil
{
	static TextEditor editor = new TextEditor();

	public static string Value
	{
		get
		{
			editor.SelectAll();
			editor.Delete();
			editor.Paste();
			return editor.text;
		}
		set
		{
			editor.text = value;
			editor.OnFocus();
			editor.Copy();
		}
	}
}