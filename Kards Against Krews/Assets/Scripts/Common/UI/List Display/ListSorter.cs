using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class ListSorter<T> : CustomBehaviour
{
	protected abstract int Compare(T a, T b);

	public IOrderedEnumerable<T> Sort(IEnumerable<T> items)
	{
		if (items is IOrderedEnumerable<T>)
		{
			return (items as IOrderedEnumerable<T>).ThenBy(x => x, new Comparer(this));
		}
		else
		{
			return items.OrderBy(x => x, new Comparer(this));
		}
	}

	class Comparer : IComparer<T>
	{
		ListSorter<T> owner;

		public Comparer(ListSorter<T> owner)
		{
			this.owner = owner;
		}

		public int Compare(T a, T b)
		{
			return owner.Compare(a, b);
		}
	}
}