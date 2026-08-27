using System;
using System.IO;

namespace SimpleJSON
{
	public class JSONData : JSONNode
	{
		private string m_Data;

		public override string Value
		{
			get
			{
				return m_Data;
			}
			set
			{
				m_Data = value;
				Tag = JSONBinaryTag.Value;
			}
		}

		public JSONData(string aData)
		{
			m_Data = aData;
			Tag = JSONBinaryTag.Value;
		}

		public JSONData(float aData)
		{
			AsFloat = aData;
		}

		public JSONData(double aData)
		{
			AsDouble = aData;
		}

		public JSONData(long aData)
		{
			AsLong = aData;
		}

		public JSONData(bool aData)
		{
			AsBool = aData;
		}

		public JSONData(int aData)
		{
			AsInt = aData;
		}

		public override string ToString()
		{
			if (Tag == JSONBinaryTag.IntValue)
			{
				return JSONNode.Escape(m_Data);
			}
			if (Tag == JSONBinaryTag.FloatValue)
			{
				return JSONNode.Escape(m_Data);
			}
			if (Tag == JSONBinaryTag.LongValue)
			{
				return JSONNode.Escape(m_Data);
			}
			return "\"" + JSONNode.Escape(m_Data) + "\"";
		}

		public override string ToString(string aPrefix)
		{
			return "\"" + JSONNode.Escape(m_Data) + "\"";
		}

		public override string ToJSON(int prefix)
		{
			switch (Tag)
			{
			case JSONBinaryTag.IntValue:
			case JSONBinaryTag.DoubleValue:
			case JSONBinaryTag.FloatValue:
			case JSONBinaryTag.LongValue:
				return m_Data;
			case JSONBinaryTag.Value:
				return string.Format("\"{0}\"", JSONNode.Escape(m_Data));
			default:
				throw new NotSupportedException("This shouldn't be here: " + Tag);
			}
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			JSONData jSONData = new JSONData(string.Empty);
			jSONData.AsInt = AsInt;
			if (jSONData.m_Data == m_Data)
			{
				aWriter.Write((byte)4);
				aWriter.Write(AsInt);
				return;
			}
			jSONData.AsFloat = AsFloat;
			if (jSONData.m_Data == m_Data)
			{
				aWriter.Write((byte)7);
				aWriter.Write(AsFloat);
				return;
			}
			jSONData.AsDouble = AsDouble;
			if (jSONData.m_Data == m_Data)
			{
				aWriter.Write((byte)5);
				aWriter.Write(AsDouble);
				return;
			}
			jSONData.AsLong = AsLong;
			if (jSONData.m_Data == m_Data)
			{
				aWriter.Write((byte)8);
				aWriter.Write(AsLong);
				return;
			}
			jSONData.AsBool = AsBool;
			if (jSONData.m_Data == m_Data)
			{
				aWriter.Write((byte)6);
				aWriter.Write(AsBool);
			}
			else
			{
				aWriter.Write((byte)3);
				aWriter.Write(m_Data);
			}
		}
	}
}
