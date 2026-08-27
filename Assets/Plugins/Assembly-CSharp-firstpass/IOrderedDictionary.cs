using System.Collections;
using System.Reflection;

[DefaultMember("Item")]
public interface IOrderedDictionary : IDictionary, IEnumerable, ICollection
{
	// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
	object get_DLKPBAJDHBO(int index);

	void set_DLKPBAJDHBO(int index, object value);

	new IDictionaryEnumerator GetEnumerator();

	void Insert(int index, object KGBGENDIMBC, object value);

	void RemoveAt(int index);

	object get_Item(int index);

	void set_Item(int index, object value);
}
