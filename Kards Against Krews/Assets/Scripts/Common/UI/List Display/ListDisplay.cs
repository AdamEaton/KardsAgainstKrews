using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public abstract class ListDisplay<T> : CustomBehaviour
{
	[SerializeField]
	GameObject _itemPrefab;
	[SerializeField]
	protected Transform entryHolder;

	public UnityEvent onListUpdated;

	protected List<ListDisplayItem<T>> currentItems = new List<ListDisplayItem<T>>();

	protected ListDisplayItem<T> itemPrefab
	{
		get { return (_itemPrefab == null ? null : _itemPrefab.GetComponent<ListDisplayItem<T>>()); }
	}

	protected abstract IEnumerable<T> GetUnfilteredItems();

	public virtual void Refresh()
	{
		Populate(GetUnfilteredItems());
	}
	void Populate(IEnumerable<T> items)
	{
		Clear();

		if (entryHolder == null || itemPrefab == null)
			return;

		foreach (var filter in GetComponents<ListFilter<T>>())
		{
			items = filter.Filter(items);
		}

		foreach (var sorter in GetComponents<ListSorter<T>>())
		{
			items = sorter.Sort(items);
		}

		foreach (var item in items)
		{
			var newItem = Instantiate(itemPrefab);
			currentItems.Add(newItem);
			newItem.gameObject.SetActive(true);
			newItem.transform.SetParent(entryHolder, false);
			newItem.transform.localScale = Vector3.one;

			newItem.Populate(this, item);
		}

		if (!isActiveAndEnabled)
			return;

		onListUpdated.TryInvoke();
	}
	void Clear()
	{
		foreach (var item in currentItems)
		{
			Destroy(item.gameObject);
		}
		currentItems.Clear();
	}

	void OnValidate()
	{
		if (_itemPrefab == null)
			return;

		if (itemPrefab != null)
			return;

		_itemPrefab = null;
	}
}