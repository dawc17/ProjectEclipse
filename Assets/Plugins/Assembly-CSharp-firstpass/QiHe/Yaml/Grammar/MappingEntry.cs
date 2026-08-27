namespace QiHe.Yaml.Grammar
{
	public class MappingEntry
	{
		public DataItem Key;

		public DataItem Value;

		public override string ToString()
		{
			return string.Format("{{Key:{0}, Value:{1}}}", Key, Value);
		}
	}
}
