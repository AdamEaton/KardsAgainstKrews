using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class BinaryReaderExt
{
	public static NetworkMessageID ReadNetworkMessageID(this BinaryReader reader)
	{
		return (NetworkMessageID)reader.ReadByte();
	}
	public static CardData ReadCard(this BinaryReader reader)
	{
		var output = ScriptableObject.CreateInstance<CardData>();
		output.id = reader.ReadInt32();
		output.isBlank = reader.ReadBoolean();
		output.text = reader.ReadString();
		output.pack = reader.ReadString();
		return output;
	}
}
