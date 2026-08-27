using System.Collections;

public interface IJsonWrapper : IDictionary, IList, IEnumerable, ICollection, IOrderedDictionary
{
	bool MENGHLDLPDP { get; }

	bool BKKEJEHCHAK { get; }

	bool LPEHBKJIAJB { get; }

	bool MKGEMBAAPBL { get; }

	bool LNLKOGMCNNF { get; }

	bool PDKNNMDCPDJ { get; }

	bool JDALJCCIBIN { get; }

	bool NKLOBJNAFOL();

	bool DBAOMEBNMPH();

	bool OEIGDMENBKN();

	bool BGDHACEDILB();

	bool BPKJMLDOLPH();

	bool HKCKGNMIKBM();

	bool FMFILGDCAKM();

	bool GetBoolean();

	double GetDouble();

	int GetInt();

	GGIECEPGFNH NCGOKKHFKJF();

	long GetLong();

	string GetString();

	void SetBoolean(bool PKHDLOGJKAD);

	void SetDouble(double PKHDLOGJKAD);

	void SetInt(int PKHDLOGJKAD);

	void FJKDNANFIHA(GGIECEPGFNH LFLGCDNKNJI);

	void SetLong(long PKHDLOGJKAD);

	void SetString(string PKHDLOGJKAD);

	string ToJson();

	void ToJson(JsonWriter writer);
}
