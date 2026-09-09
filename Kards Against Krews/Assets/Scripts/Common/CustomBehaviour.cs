using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomBehaviour : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField]
	protected bool enableLogs;
	[SerializeField]
	protected Color defaultLogColor = Color.white;
#endif

	public void Log(object message)
	{
#if UNITY_EDITOR
		Log(message, this);
#endif
	}
	public void Log(object message, Color color)
	{
#if UNITY_EDITOR
		Log(message, color, this);
#endif
	}
	public void Log(object message, Object context)
	{
#if UNITY_EDITOR
		Log(message, defaultLogColor, context);
#endif
	}
	public void Log(object message, Color color, Object context)
	{
#if UNITY_EDITOR
		if (!enableLogs)
			return;

		Debug.Log("<color=#" + color.HexCode() + ">" + message + " @" + Time.time + " by '" + name + "'</color>", context);
#endif
	}

	public void LogWarning(object message)
	{
#if UNITY_EDITOR
		LogWarning(message, this);
#endif
	}
	public void LogWarning(object message, Color color)
	{
#if UNITY_EDITOR
		LogWarning(message, color, this);
#endif
	}
	public void LogWarning(object message, Object context)
	{
#if UNITY_EDITOR
		LogWarning(message, defaultLogColor, context);
#endif
	}
	public void LogWarning(object message, Color color, Object context)
	{
#if UNITY_EDITOR
		if (!enableLogs)
			return;

		Debug.LogWarning("<color=#" + color.HexCode() + ">" + message + " @" + Time.time + " by '" + name + "'</color>", context);
#endif
	}

	public void LogError(object message)
	{
#if UNITY_EDITOR
		LogError(message, this);
#endif
	}
	public void LogError(object message, Color color)
	{
#if UNITY_EDITOR
		LogError(message, color, this);
#endif
	}
	public void LogError(object message, Object context)
	{
#if UNITY_EDITOR
		LogError(message, defaultLogColor, context);
#endif
	}
	public void LogError(object message, Color color, Object context)
	{
#if UNITY_EDITOR
		if (!enableLogs)
			return;

		Debug.LogError("<color=#" + color.HexCode() + ">" + message + " @" + Time.time + " by '" + name + "'</color>", context);
#endif
	}
}