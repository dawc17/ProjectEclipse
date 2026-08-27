using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	public class JObject : JContainer, IDictionary<string, JToken>, ICollection<KeyValuePair<string, JToken>>, IEnumerable<KeyValuePair<string, JToken>>, IEnumerable, INotifyPropertyChanged, ICustomTypeDescriptor
	{
		private class JPropertKeyedCollection : KeyedCollection<string, JToken>
		{
			public new IDictionary<string, JToken> Dictionary
			{
				get
				{
					return base.Dictionary;
				}
			}

			public JPropertKeyedCollection(IEqualityComparer<string> comparer)
				: base(comparer)
			{
			}

			protected override string GetKeyForItem(JToken item)
			{
				return ((JProperty)item).Name;
			}

			protected override void InsertItem(int index, JToken item)
			{
				if (Dictionary == null)
				{
					base.InsertItem(index, item);
					return;
				}
				string keyForItem = GetKeyForItem(item);
				Dictionary[keyForItem] = item;
				base.Items.Insert(index, item);
			}
		}

		[CompilerGenerated]
		private sealed class _003CGetEnumerator_003Ec__Iterator0 : IEnumerator<KeyValuePair<string, JToken>>, IDisposable, IEnumerator
		{
			internal IEnumerator<JToken> _0024locvar0;

			internal JProperty _003Cproperty_003E__1;

			internal JObject _0024this;

			internal KeyValuePair<string, JToken> _0024current;

			internal bool _0024disposing;

			internal int _0024PC;

			KeyValuePair<string, JToken> IEnumerator<KeyValuePair<string, JToken>>.Current
			{
				[DebuggerHidden]
				get
				{
					return System_002ECollections_002EGeneric_002EIEnumerator_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002CNewtonsoft_002EJson_002ELinq_002EJToken_003E_003E_002Eget_Current();
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return _0024current;
				}
			}

			[DebuggerHidden]
			public _003CGetEnumerator_003Ec__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)_0024PC;
				_0024PC = -1;
				bool flag = false;
				switch (num)
				{
				case 0u:
					_0024locvar0 = _0024this.ChildrenTokens.GetEnumerator();
					num = 4294967293u;
					goto case 1u;
				case 1u:
					try
					{
						switch (num)
						{
						default:
							if (_0024locvar0.MoveNext())
							{
								_003Cproperty_003E__1 = (JProperty)_0024locvar0.Current;
								_0024current = new KeyValuePair<string, JToken>(_003Cproperty_003E__1.Name, _003Cproperty_003E__1.Value);
								if (!_0024disposing)
								{
									_0024PC = 1;
								}
								flag = true;
								goto IL_00d3;
							}
							break;
						}
					}
					finally
					{
						if (!flag && _0024locvar0 != null)
						{
							_0024locvar0.Dispose();
						}
					}
					_0024PC = -1;
					goto default;
				default:
					{
						return false;
					}
					IL_00d3:
					return true;
				}
			}

			[DebuggerHidden]
			private KeyValuePair<string, JToken> System_002ECollections_002EGeneric_002EIEnumerator_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002CNewtonsoft_002EJson_002ELinq_002EJToken_003E_003E_002Eget_Current()
			{
				return _0024current;
			}

			[DebuggerHidden]
			public void Dispose()
			{
				uint num = (uint)_0024PC;
				_0024disposing = true;
				_0024PC = -1;
				switch (num)
				{
				case 1u:
					try
					{
						break;
					}
					finally
					{
						if (_0024locvar0 != null)
						{
							_0024locvar0.Dispose();
						}
					}
				case 0u:
					break;
				}
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		private JPropertKeyedCollection _properties = new JPropertKeyedCollection(StringComparer.Ordinal);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PropertyChangedEventHandler PropertyChanged__BackingField;

		ICollection<string> IDictionary<string, JToken>.Keys
		{
			get
			{
				return System_002ECollections_002EGeneric_002EIDictionary_003Cstring_002CNewtonsoft_002EJson_002ELinq_002EJToken_003E_002Eget_Keys();
			}
		}

		ICollection<JToken> IDictionary<string, JToken>.Values
		{
			get
			{
				return System_002ECollections_002EGeneric_002EIDictionary_003Cstring_002CNewtonsoft_002EJson_002ELinq_002EJToken_003E_002Eget_Values();
			}
		}

		bool ICollection<KeyValuePair<string, JToken>>.IsReadOnly
		{
			get
			{
				return System_002ECollections_002EGeneric_002EICollection_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002CNewtonsoft_002EJson_002ELinq_002EJToken_003E_003E_002Eget_IsReadOnly();
			}
		}

		protected override IList<JToken> ChildrenTokens
		{
			get
			{
				return _properties;
			}
		}

		public override JTokenType Type
		{
			get
			{
				return JTokenType.Object;
			}
		}

		public override JToken this[object key]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				string text = key as string;
				if (text == null)
				{
					throw new ArgumentException("Accessed JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				return this[text];
			}
			set
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				string text = key as string;
				if (text == null)
				{
					throw new ArgumentException("Set JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				this[text] = value;
			}
		}

		public JToken this[string propertyName]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(propertyName, "propertyName");
				JProperty jProperty = Property(propertyName);
				return (jProperty == null) ? null : jProperty.Value;
			}
			set
			{
				JProperty jProperty = Property(propertyName);
				if (jProperty != null)
				{
					jProperty.Value = value;
					return;
				}
				Add(new JProperty(propertyName, value));
				OnPropertyChanged(propertyName);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				PropertyChangedEventHandler propertyChangedEventHandler = PropertyChanged__BackingField;
				PropertyChangedEventHandler propertyChangedEventHandler2;
				do
				{
					propertyChangedEventHandler2 = propertyChangedEventHandler;
					propertyChangedEventHandler = Interlocked.CompareExchange(ref PropertyChanged__BackingField, (PropertyChangedEventHandler)Delegate.Combine(propertyChangedEventHandler2, value), propertyChangedEventHandler);
				}
				while ((object)propertyChangedEventHandler != propertyChangedEventHandler2);
			}
			remove
			{
				PropertyChangedEventHandler propertyChangedEventHandler = PropertyChanged__BackingField;
				PropertyChangedEventHandler propertyChangedEventHandler2;
				do
				{
					propertyChangedEventHandler2 = propertyChangedEventHandler;
					propertyChangedEventHandler = Interlocked.CompareExchange(ref PropertyChanged__BackingField, (PropertyChangedEventHandler)Delegate.Remove(propertyChangedEventHandler2, value), propertyChangedEventHandler);
				}
				while ((object)propertyChangedEventHandler != propertyChangedEventHandler2);
			}
		}

		public JObject()
		{
		}

		public JObject(JObject other)
			: base(other)
		{
		}

		public JObject(params object[] content)
			: this((object)content)
		{
		}

		public JObject(object content)
		{
			Add(content);
		}

		internal override bool DeepEquals(JToken node)
		{
			JObject jObject = node as JObject;
			return jObject != null && ContentsEqual(jObject);
		}

		internal override void InsertItem(int index, JToken item)
		{
			if (item == null || item.Type != JTokenType.Comment)
			{
				base.InsertItem(index, item);
			}
		}

		internal override void ValidateToken(JToken o, JToken existing)
		{
			ValidationUtils.ArgumentNotNull(o, "o");
			if (o.Type != JTokenType.Property)
			{
				throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), GetType()));
			}
			JProperty jProperty = (JProperty)o;
			if (existing != null)
			{
				JProperty jProperty2 = (JProperty)existing;
				if (jProperty.Name == jProperty2.Name)
				{
					return;
				}
			}
			if (_properties.Dictionary != null && _properties.Dictionary.TryGetValue(jProperty.Name, out existing))
			{
				throw new ArgumentException("Can not add property {0} to {1}. Property with the same name already exists on object.".FormatWith(CultureInfo.InvariantCulture, jProperty.Name, GetType()));
			}
		}

		internal void InternalPropertyChanged(JProperty childProperty)
		{
			OnPropertyChanged(childProperty.Name);
		}

		internal void InternalPropertyChanging(JProperty childProperty)
		{
		}

		internal override JToken CloneToken()
		{
			return new JObject(this);
		}

		public IEnumerable<JProperty> Properties()
		{
			return ChildrenTokens.Cast<JProperty>();
		}

		public JProperty Property(string name)
		{
			if (_properties.Dictionary == null)
			{
				return null;
			}
			if (name == null)
			{
				return null;
			}
			JToken value;
			_properties.Dictionary.TryGetValue(name, out value);
			return (JProperty)value;
		}

		public JEnumerable<JToken> PropertyValues()
		{
			return new JEnumerable<JToken>(from p in Properties()
				select p.Value);
		}

		public new static JObject Load(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw new Exception("Error reading JObject from JsonReader.");
			}
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Error reading JObject from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JObject jObject = new JObject();
			jObject.SetLineInfo(reader as IJsonLineInfo);
			jObject.ReadTokenFrom(reader);
			return jObject;
		}

		public new static JObject Parse(string json)
		{
			JsonReader reader = new JsonTextReader(new StringReader(json));
			return Load(reader);
		}

		public new static JObject FromObject(object o)
		{
			return FromObject(o, new JsonSerializer());
		}

		public new static JObject FromObject(object o, JsonSerializer jsonSerializer)
		{
			JToken jToken = JToken.FromObjectInternal(o, jsonSerializer);
			if (jToken != null && jToken.Type != JTokenType.Object)
			{
				throw new ArgumentException("Object serialized to {0}. JObject instance expected.".FormatWith(CultureInfo.InvariantCulture, jToken.Type));
			}
			return (JObject)jToken;
		}

		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WriteStartObject();
			foreach (JProperty childrenToken in ChildrenTokens)
			{
				childrenToken.WriteTo(writer, converters);
			}
			writer.WriteEndObject();
		}

		public void Add(string propertyName, JToken value)
		{
			Add(new JProperty(propertyName, value));
		}

		bool IDictionary<string, JToken>.ContainsKey(string key)
		{
			if (_properties.Dictionary == null)
			{
				return false;
			}
			return _properties.Dictionary.ContainsKey(key);
		}

		private ICollection<string> System_002ECollections_002EGeneric_002EIDictionary_003Cstring_002CNewtonsoft_002EJson_002ELinq_002EJToken_003E_002Eget_Keys()
		{
			return _properties.Dictionary.Keys;
		}

		public bool Remove(string propertyName)
		{
			JProperty jProperty = Property(propertyName);
			if (jProperty == null)
			{
				return false;
			}
			jProperty.Remove();
			return true;
		}

		public bool TryGetValue(string propertyName, out JToken value)
		{
			JProperty jProperty = Property(propertyName);
			if (jProperty == null)
			{
				value = null;
				return false;
			}
			value = jProperty.Value;
			return true;
		}

		private ICollection<JToken> System_002ECollections_002EGeneric_002EIDictionary_003Cstring_002CNewtonsoft_002EJson_002ELinq_002EJToken_003E_002Eget_Values()
		{
			return _properties.Dictionary.Values;
		}

		void ICollection<KeyValuePair<string, JToken>>.Add(KeyValuePair<string, JToken> item)
		{
			Add(new JProperty(item.Key, item.Value));
		}

		void ICollection<KeyValuePair<string, JToken>>.Clear()
		{
			RemoveAll();
		}

		bool ICollection<KeyValuePair<string, JToken>>.Contains(KeyValuePair<string, JToken> item)
		{
			JProperty jProperty = Property(item.Key);
			if (jProperty == null)
			{
				return false;
			}
			return jProperty.Value == item.Value;
		}

		void ICollection<KeyValuePair<string, JToken>>.CopyTo(KeyValuePair<string, JToken>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
			}
			if (arrayIndex >= array.Length)
			{
				throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
			}
			if (base.Count > array.Length - arrayIndex)
			{
				throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
			}
			int num = 0;
			foreach (JProperty childrenToken in ChildrenTokens)
			{
				array[arrayIndex + num] = new KeyValuePair<string, JToken>(childrenToken.Name, childrenToken.Value);
				num++;
			}
		}

		private bool System_002ECollections_002EGeneric_002EICollection_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002CNewtonsoft_002EJson_002ELinq_002EJToken_003E_003E_002Eget_IsReadOnly()
		{
			return false;
		}

		bool ICollection<KeyValuePair<string, JToken>>.Remove(KeyValuePair<string, JToken> item)
		{
			if (!((ICollection<KeyValuePair<string, JToken>>)this).Contains(item))
			{
				return false;
			}
			((IDictionary<string, JToken>)this).Remove(item.Key);
			return true;
		}

		internal override int GetDeepHashCode()
		{
			return ContentsHashCode();
		}

		[DebuggerHidden]
		public IEnumerator<KeyValuePair<string, JToken>> GetEnumerator()
		{
			//yield-return decompiler failed: Could not find currentField
			_003CGetEnumerator_003Ec__Iterator0 obj = new _003CGetEnumerator_003Ec__Iterator0();
			obj._0024this = this;
			return obj;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged__BackingField != null)
			{
				PropertyChanged__BackingField(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties((Attribute[])null);
		}

		private static Type GetTokenPropertyType(JToken token)
		{
			if (token is JValue)
			{
				JValue jValue = (JValue)token;
				return (jValue.Value == null) ? typeof(object) : jValue.Value.GetType();
			}
			return token.GetType();
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			using (IEnumerator<KeyValuePair<string, JToken>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, JToken> current = enumerator.Current;
					propertyDescriptorCollection.Add(new JPropertyDescriptor(current.Key, GetTokenPropertyType(current.Value)));
				}
				return propertyDescriptorCollection;
			}
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return AttributeCollection.Empty;
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return null;
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return new TypeConverter();
		}

		global::System.ComponentModel.EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		global::System.ComponentModel.PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return EventDescriptorCollection.Empty;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return EventDescriptorCollection.Empty;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(global::System.ComponentModel.PropertyDescriptor pd)
		{
			return null;
		}
	}
}
