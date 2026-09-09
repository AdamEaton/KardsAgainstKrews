using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LocalCardPackFetcher : CardPackFetcher
{
	public static string PacksDirectory { get { return Path.Combine(Application.persistentDataPath, "Packs"); } }

	public override IEnumerable<string> GetPackNames()
	{
		foreach (var fileName in Directory.GetFiles(PacksDirectory, "*.cardpack", SearchOption.AllDirectories))
		{
			yield return fileName;
		}
	}

	public override CardPack FetchPack(string name)
	{
		if (!File.Exists(name))
		{
			Debug.LogError("Card Pack does not exist at path '" + name + "'.", this);
			return null;
		}

		CardPack output = ScriptableObject.CreateInstance<CardPack>();
		CardData card;
		string line;

		using (var stream = new StreamReader(name))
		{
			line = stream.ReadLine();
			output.name = line;

			do
			{
				line = AdjustCardText(stream.ReadLine());

				if (!string.IsNullOrEmpty(line))
				{
					card = ScriptableObject.CreateInstance<CardData>();
					card.text = line;
					card.pack = output.name;
					output.callCards.Add(card);
				}
			} while (!string.IsNullOrEmpty(line));

			do
			{
				line = AdjustCardText(stream.ReadLine());

				if (!string.IsNullOrEmpty(line))
				{
					card = ScriptableObject.CreateInstance<CardData>();
					card.text = line;
					card.pack = output.name;
					output.responseCards.Add(card);
				}
			} while (!string.IsNullOrEmpty(line));
		}

		return output;
	}

	public void SerializeCardPack(CardPack pack, string path)
	{
		if (!pack) return;
		
		using (var writer = new StreamWriter(path))
		{
			writer.WriteLine(pack.name);

			foreach (var callCard in pack.callCards)
			{
				writer.WriteLine(callCard.text);
			}

			writer.WriteLine();

			foreach (var responseCard in pack.responseCards)
			{
				writer.WriteLine(responseCard.text);
			}
		}
	}
}
