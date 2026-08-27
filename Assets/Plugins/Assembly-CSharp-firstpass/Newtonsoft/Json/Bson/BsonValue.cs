namespace Newtonsoft.Json.Bson
{
	internal class BsonValue : BsonToken
	{
		private object _value;

		private BsonType _type;

		public object Value
		{
			get
			{
				return _value;
			}
		}

		public override BsonType Type
		{
			get
			{
				return _type;
			}
		}

		public BsonValue(object value, BsonType type)
		{
			_value = value;
			_type = type;
		}
	}
}
