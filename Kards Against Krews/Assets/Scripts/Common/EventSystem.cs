using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for all event types that can be dispatched.
/// </summary>
public class EventInstance { }

/// <summary>
/// Handles event-based programming.
/// </summary>
public static class EventSystem
{
	/// <summary>
	/// Delegate type for listener functions of events of type T.
	/// </summary>
	/// <typeparam name="T">The event type that this delegate can listen for.</typeparam>
	/// <param name="e">The event that was heard by the listener.</param>
	public delegate void EventDelegate<T>(T e) where T : EventInstance;

	/// <summary>
	/// Interface for polymorphically storing delegates in the listeners Dictionary.
	/// </summary>
	interface IInvokable
	{
		void Invoke(object e);
	}
	/// <summary>
	/// Generic implementation of IInvokable, spawned for each type of event listener.
	/// </summary>
	/// <typeparam name="T">The type of event this Invokable listens for.</typeparam>
	class Invokable<T> : IInvokable where T : EventInstance
	{
		public event EventDelegate<T> del;

		public void Invoke(object e)
		{
			if (del != null)
				del.Invoke(e as T);
		}
	}

	/// <summary>
	/// Stores the set of all listeners that have been added.
	/// </summary>
	static Dictionary<System.Type, IInvokable> listeners = new Dictionary<System.Type, IInvokable>();

	/// <summary>
	/// Adds an event listener to call the specified listener function.
	/// </summary>
	/// <typeparam name="T">The type of event to listen for.</typeparam>
	/// <param name="listener">The listener function to call for each event that is heard</param>
	public static void AddListener<T>(EventDelegate<T> listener) where T : EventInstance
	{
		var t = typeof(T);
		if (!listeners.ContainsKey(t))
			listeners[t] = new Invokable<T>();

		var invokable = listeners[t] as Invokable<T>;
		invokable.del += listener;
	}
	/// <summary>
	/// Removes the existing event listener for the specified listener function.
	/// </summary>
	/// <typeparam name="T">The type of event that was listened for.</typeparam>
	/// <param name="listener">The listener function to remove.</param>
	public static void RemoveListener<T>(EventDelegate<T> listener) where T : EventInstance
	{
		var t = typeof(T);
		if (!listeners.ContainsKey(t))
			return;

		var invokable = listeners[t] as Invokable<T>;
		invokable.del -= listener;
	}

	/// <summary>
	/// Dispatches the specified event for all relevant listeners to hear.
	/// </summary>
	/// <typeparam name="T">The (implicit) type of the event to dispatch.</typeparam>
	/// <param name="e">The event to dispatch.</param>
	public static void Dispatch<T>(T e) where T : EventInstance
	{
		var t = typeof(T);

		while (true)
		{
			if (listeners.ContainsKey(t))
				listeners[t].Invoke(e);
			
			if (t == typeof(EventInstance))
				break;
			
			t = t.BaseType;
		}
	}
}