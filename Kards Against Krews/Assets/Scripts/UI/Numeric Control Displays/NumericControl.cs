using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NumericControl : ListDisplay<int>
{
	[SerializeField]
	int _minValue;
	public int minValue
	{
		get { return Mathf.Max(_minValue, 0); }
		set
		{
			_minValue = Mathf.Max(value, 0);
			maxValue = maxValue;
			currentValue = currentValue;
		}
	}
	[SerializeField]
	int _maxValue;
	public int maxValue
	{
		get { return Mathf.Max(_maxValue, _minValue); }
		set
		{
			_maxValue = Mathf.Max(value, minValue);
			currentValue = currentValue;
			dirty = true;
		}
	}
	[SerializeField]
	int _currentValue;
	public int currentValue
	{
		get { return Mathf.Clamp(_currentValue, minValue, maxValue); }
		set
		{
			_currentValue = Mathf.Clamp(value, minValue, maxValue);
			onValueChanged.TryInvoke(currentValue);
		}
	}

	public IntEvent onValueChanged;

	bool dirty = true;

	void OnEnable()
	{
		onValueChanged.TryInvoke(currentValue);
	}

	void Update()
	{
		if (dirty)
		{
			Refresh();
		}
	}

	protected override IEnumerable<int> GetUnfilteredItems()
	{
		int max = 1;
		while (max < maxValue)
			max *= 10;

		for (max /= 10; max >= 1; max /= 10)
			yield return max;
	}

	public override void Refresh()
	{
		base.Refresh();

		dirty = false;
	}

	void OnValidate()
	{
		_minValue = minValue;
		_maxValue = maxValue;
		_currentValue = currentValue;
	}
}