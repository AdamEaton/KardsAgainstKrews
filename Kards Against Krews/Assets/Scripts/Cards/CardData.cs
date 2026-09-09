using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class CardData : ScriptableObject
{
	public static IEnumerable<CardData> GetBlankCards(int count)
	{
		CardData output;

		for (int i = 0; i < count; i++)
		{
			output = ScriptableObject.CreateInstance<CardData>();
			output.isBlank = true;
			output.pack = "Shootin' Blanks";
			yield return output;
		}
	}
	public static IEnumerable<CardData> GetCards(string text, int count)
	{
		CardData output;

		for (int i = 0; i < count; i++)
		{
			output = ScriptableObject.CreateInstance<CardData>();
			output.text = text;
			output.pack = "Let's Get Personal";
			yield return output;
		}
	}

	public int id;
	public bool isBlank;
	public string text = "";
	public string pack = "";

	public int responseCount { get { return Mathf.Max(text.Where(x => x == '_').Count(), 1); } }

	public CardData Clone()
	{
		var output = ScriptableObject.CreateInstance<CardData>();

		output.id = id;
		output.isBlank = isBlank;
		output.text = text;
		output.pack = pack;

		return output;
	}
}
