using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using YamlDotNet.Core;

[DefaultMember("Item")]
internal sealed class ObjectAnchorCollection
{
	private readonly IDictionary<string, object> objectsByAnchor = new Dictionary<string, object>();

	private readonly IDictionary<object, string> anchorsByObject = new Dictionary<object, string>();

	// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
	public object get_DLKPBAJDHBO(string KOLNNNLOCFE)
	{
		return get_Item(KOLNNNLOCFE);
	}

	public void Add(string KOLNNNLOCFE, object EGJFDKEKAJL)
	{
		objectsByAnchor.Add(KOLNNNLOCFE, EGJFDKEKAJL);
		if (EGJFDKEKAJL != null)
		{
			anchorsByObject.Add(EGJFDKEKAJL, KOLNNNLOCFE);
		}
	}

	public bool TryGetAnchor(object EGJFDKEKAJL, out string KOLNNNLOCFE)
	{
		return anchorsByObject.TryGetValue(EGJFDKEKAJL, out KOLNNNLOCFE);
	}

	public object get_Item(string KOLNNNLOCFE)
	{
		object value;
		if (objectsByAnchor.TryGetValue(KOLNNNLOCFE, out value))
		{
			return value;
		}
		throw new AnchorNotFoundException(string.Format(CultureInfo.InvariantCulture, "The anchor '{0}' does not exists", KOLNNNLOCFE));
	}
}
