using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class ListFilter<T> : CustomBehaviour
{
	protected abstract bool IsAllowed(T item);

	public IEnumerable<T> Filter(IEnumerable<T> items)
	{
		return items.Where(IsAllowed);
	}
}