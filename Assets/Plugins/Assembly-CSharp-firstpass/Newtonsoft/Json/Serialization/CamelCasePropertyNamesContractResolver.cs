using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	public class CamelCasePropertyNamesContractResolver : DefaultContractResolver
	{
		public CamelCasePropertyNamesContractResolver()
			: base(true)
		{
		}

		protected internal override string ResolvePropertyName(string propertyName)
		{
			return StringUtils.ToCamelCase(propertyName);
		}
	}
}
