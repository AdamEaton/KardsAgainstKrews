using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class BinaryWriterExt
{
	public static void Write(this BinaryWriter writer, NetworkMessageID id)
	{
		writer.Write((byte)id);
	}
	public static void WriteCard(this BinaryWriter writer, CardData card)
	{
		writer.Write(card.id);
		writer.Write(card.isBlank);
		writer.Write(card.text);
		writer.Write(card.pack);
	}
}
