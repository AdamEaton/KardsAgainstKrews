using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public static class UnityEventExt
{
	public static bool TryInvoke(this UnityEvent e)
	{
		if (e == null)
			return false;

		e.Invoke();
		return true;
	}
	public static bool TryInvoke<T>(this UnityEvent<T> e, T arg)
	{
		if (e == null)
			return false;

		e.Invoke(arg);
		return true;
	}
	public static bool TryInvoke<T0, T1>(this UnityEvent<T0, T1> e, T0 arg0, T1 arg1)
	{
		if (e == null)
			return false;

		e.Invoke(arg0, arg1);
		return true;
	}
	public static bool TryInvoke<T0, T1, T2>(this UnityEvent<T0, T1, T2> e, T0 arg0, T1 arg1, T2 arg2)
	{
		if (e == null)
			return false;

		e.Invoke(arg0, arg1, arg2);
		return true;
	}
	public static bool TryInvoke<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> e, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
	{
		if (e == null)
			return false;

		e.Invoke(arg0, arg1, arg2, arg3);
		return true;
	}
}