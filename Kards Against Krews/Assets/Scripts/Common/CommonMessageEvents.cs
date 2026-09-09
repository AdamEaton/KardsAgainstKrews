using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class CommonMessageEvents : CustomBehaviour
{
	public UnityEvent awake; void Awake() { awake.TryInvoke(); }
	public UnityEvent start; void Start() { start.TryInvoke(); }
	public UnityEvent onDestroy; void OnDestroy() { onDestroy.TryInvoke(); }
	public UnityEvent onEnable; void OnEnable() { onEnable.TryInvoke(); }
	public UnityEvent onDisable; void OnDisable() { onDisable.TryInvoke(); }
}
