using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public static class NetworkingUtil
{
	#region Message Writers/Readers

	#region Basic

	public static byte[] WriteBasicMessage(NetworkMessageID id)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadBasicMessage(byte[] buffer, out NetworkMessageID id)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
			}
		}
	}

	#endregion

	#region Bool

	public static byte[] WriteBoolMessage(NetworkMessageID id, bool value)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
				writer.Write(value);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadBoolMessage(byte[] buffer, out NetworkMessageID id, out bool value)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
				value = reader.ReadBoolean();
			}
		}
	}

	#endregion

	#region Int

	public static byte[] WriteIntMessage(NetworkMessageID id, int value)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
				writer.Write(value);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadIntMessage(byte[] buffer, out NetworkMessageID id, out int value)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
				value = reader.ReadInt32();
			}
		}
	}

	#endregion

	#region Float

	public static byte[] WriteFloatMessage(NetworkMessageID id, float value)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
				writer.Write(value);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadFloatMessage(byte[] buffer, out NetworkMessageID id, out float value)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
				value = reader.ReadSingle();
			}
		}
	}

	#endregion

	#region String

	public static byte[] WriteStringMessage(NetworkMessageID id, string value)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
				writer.Write(value);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadStringMessage(byte[] buffer, out NetworkMessageID id, out string value)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
				value = reader.ReadString();
			}
		}
	}

	#endregion

	#region Int/Int

	public static byte[] WriteIntIntMessage(NetworkMessageID id, int value1, int value2)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
				writer.Write(value1);
				writer.Write(value2);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadIntIntMessage(byte[] buffer, out NetworkMessageID id, out int value1, out int value2)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
				value1 = reader.ReadInt32();
				value2 = reader.ReadInt32();
			}
		}
	}

	#endregion

	#region Bool/String

	public static byte[] WriteBoolStringMessage(NetworkMessageID id, bool value1, string value2)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
				writer.Write(value1);
				writer.Write(value2);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadBoolStringMessage(byte[] buffer, out NetworkMessageID id, out bool value1, out string value2)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
				value1 = reader.ReadBoolean();
				value2 = reader.ReadString();
			}
		}
	}

	#endregion

	#region Int/String

	public static byte[] WriteIntStringMessage(NetworkMessageID id, int value1, string value2)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
				writer.Write(value1);
				writer.Write(value2);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadIntStringMessage(byte[] buffer, out NetworkMessageID id, out int value1, out string value2)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
				value1 = reader.ReadInt32();
				value2 = reader.ReadString();
			}
		}
	}

	#endregion

	#region Int List

	public static byte[] WriteIntListMessage(NetworkMessageID id, int[] values)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
				writer.Write(values.Length);
				foreach (var value in values)
					writer.Write(value);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadIntListMessage(byte[] buffer, out NetworkMessageID id, out int[] values)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
				values = new int[reader.ReadInt32()];
				for (int i = 0; i < values.Length; i++)
					values[i] = reader.ReadInt32();
			}
		}
	}

	#endregion

	#region Card

	public static byte[] WriteCardMessage(NetworkMessageID id, int cardID, bool cardIsBlank, string cardText, string packText)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
				writer.Write(cardID);
				writer.Write(cardIsBlank);
				writer.Write(cardText);
				writer.Write(packText);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadCardMessage(byte[] buffer, out NetworkMessageID id, out int cardID, out bool cardIsBlank, out string cardText, out string packText)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
				cardID = reader.ReadInt32();
				cardIsBlank = reader.ReadBoolean();
				cardText = reader.ReadString();
				packText = reader.ReadString();
			}
		}
	}
	public static byte[] WriteCardMessage(NetworkMessageID id, CardData value)
	{
		return WriteCardMessage(id, value.id, value.isBlank, value.text, value.pack);
	}
	public static void ReadCardMessage(byte[] buffer, out NetworkMessageID id, out CardData value)
	{
		value = ScriptableObject.CreateInstance<CardData>();
		ReadCardMessage(buffer, out id, out value.id, out value.isBlank, out value.text, out value.pack);
	}

	#endregion

	#region Response

	public static byte[] WriteResponseMessage(NetworkMessageID id, params CardData[] cards)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
				writer.Write(cards.Length);
				foreach (var card in cards)
					writer.WriteCard(card);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadResponseMessage(byte[] buffer, out NetworkMessageID id, out CardData[] cards)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
				cards = new CardData[reader.ReadInt32()];
				for (int i = 0; i < cards.Length; i++)
					cards[i] = reader.ReadCard();
			}
		}
	}

	#endregion

	#region Int/Response

	public static byte[] WriteIntResponseMessage(NetworkMessageID id, int value1, params CardData[] value2)
	{
		using (var outputStream = new MemoryStream())
		{
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write(id);
				writer.Write(value1);
				writer.Write(value2.Length);
				foreach (var card in value2)
					writer.WriteCard(card);
			}

			return outputStream.ToArray();
		}
	}
	public static void ReadIntResponseMessage(byte[] buffer, out NetworkMessageID id, out int value1, out CardData[] value2)
	{
		using (var inputStream = new MemoryStream(buffer))
		{
			using (var reader = new BinaryReader(inputStream))
			{
				id = reader.ReadNetworkMessageID();
				value1 = reader.ReadInt32();
				value2 = new CardData[reader.ReadInt32()];
				for (int i = 0; i < value2.Length; i++)
					value2[i] = reader.ReadCard();
			}
		}
	}

	#endregion

	#endregion
}
