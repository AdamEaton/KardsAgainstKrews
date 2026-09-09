using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static partial class WaitFor
{
	public class Condition : CustomYieldInstruction
	{
		Func<bool> condition;

		public override bool keepWaiting
		{
			get { return !condition(); }
		}

		public Condition(Func<bool> condition)
		{
			this.condition = condition;
		}
	}

	public class Seconds : CustomYieldInstruction
	{
		float seconds;
		float startTime;

		public override bool keepWaiting { get { return Time.time - startTime < seconds; } }

		public Seconds(float seconds)
		{
			this.seconds = seconds;
			this.startTime = Time.time;
		}
	}

	public class RealSeconds : CustomYieldInstruction
	{
		float seconds;
		float startTime;

		public override bool keepWaiting { get { return Time.realtimeSinceStartup - startTime < seconds; } }

		public RealSeconds(float seconds)
		{
			this.seconds = seconds;
			this.startTime = Time.realtimeSinceStartup;
		}
	}

	public class Event<T> : CustomYieldInstruction where T : EventInstance
	{
		bool gotEvent;
		public override bool keepWaiting { get { return !gotEvent; } }

		public Event()
		{
			gotEvent = false;
			EventSystem.AddListener<T>(OnEvent);
		}

		void OnEvent(T e)
		{
			EventSystem.RemoveListener<T>(OnEvent);
			gotEvent = true;
		}
	}

	public class Any : CustomYieldInstruction
	{
		CustomYieldInstruction[] instructions;

		public override bool keepWaiting { get { return instructions.All(x => x.keepWaiting); } }

		public Any(params CustomYieldInstruction[] instructions)
		{
			this.instructions = instructions;
		}
	}

	public class All : CustomYieldInstruction
	{
		CustomYieldInstruction[] instructions;

		public override bool keepWaiting { get { return instructions.Any(x => x.keepWaiting); } }

		public All(params CustomYieldInstruction[] instructions)
		{
			this.instructions = instructions;
		}
	}
}