using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class GameMetrics : ScriptableObject
{
	[SerializeField]
	int _handSize;
	public int handSize { get { return _handSize; } set { _handSize = value; } }
	[SerializeField]
	int _targetScore;
	public int targetScore { get { return _targetScore; } set { _targetScore = value; } }
	[SerializeField]
	int _blankCardCount;
	public int blankCardCount { get { return _blankCardCount; } set { _blankCardCount = value; } }
	[SerializeField]
	int _personalCardCount;
	public int personalCardCount { get { return _personalCardCount; } set { _personalCardCount = value; } }
	[SerializeField]
	bool _disablePackResponses;
	public bool disablePackResponses { get { return _disablePackResponses; } set { _disablePackResponses = value; } }
	[SerializeField]
	int _maximumResponseCards;
	public int maximumResponseCards { get { return _maximumResponseCards; } set { _maximumResponseCards = value; } }
	[SerializeField]
	int _checkPointFrequency;
	public int checkPointFrequency { get { return _checkPointFrequency; } set { _checkPointFrequency = value; } }

	public GameMetrics Clone()
	{
		GameMetrics output = CreateInstance<GameMetrics>();

		output.handSize = handSize;
		output.targetScore = targetScore;
		output.blankCardCount = blankCardCount;
		output.personalCardCount = personalCardCount;
		output.disablePackResponses = disablePackResponses;
		output.maximumResponseCards = maximumResponseCards;
		output.checkPointFrequency = checkPointFrequency;

		return output;
	}
}
