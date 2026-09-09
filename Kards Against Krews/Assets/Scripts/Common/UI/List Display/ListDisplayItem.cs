using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListDisplayItem<T> : CustomBehaviour
{
	public T currentItem { get; protected set; }

	protected ListDisplay<T> owner;

	public virtual void Populate(ListDisplay<T> owner, T item)
	{
		this.owner = owner;
		currentItem = item;
	}
}