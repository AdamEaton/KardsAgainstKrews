using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class CardPack : ScriptableObject
{
	public List<CardData> callCards = new List<CardData>();
	public List<CardData> responseCards = new List<CardData>();
}
