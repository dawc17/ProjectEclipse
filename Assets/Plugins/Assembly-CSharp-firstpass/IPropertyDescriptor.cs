using System;

public interface IPropertyDescriptor
{
	string MENAJEAJJBE { get; }

	bool KBHICFPAIFJ { get; }

	Type JDCDCGFHLPC { get; set; }

	int PECDGDLCAAA { get; set; }

	string get_Name();

	bool HHHGHBBDMHC();

	Type get_Type();

	Type MAGHEGMMNOF();

	void set_TypeOverride(Type value);

	int BHDEMLGCNOJ();

	void set_Order(int value);

	T PJLLHGDNCIF<T>() where T : Attribute;

	IObjectDescriptor Read(object target);

	void Write(object target, object value);
}
