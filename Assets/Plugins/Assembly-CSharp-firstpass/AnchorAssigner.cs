using System;
using System.Collections.Generic;
using System.Globalization;

public sealed class AnchorAssigner : IAliasProvider, IObjectGraphVisitor
{
	private class EOMGEBLLMMG
	{
		public string BJKKJNDLDJN;
	}

	private readonly IDictionary<object, EOMGEBLLMMG> AOPBKMDMKDB = new Dictionary<object, EOMGEBLLMMG>();

	private uint nextId;

	bool IObjectGraphVisitor.Enter(IObjectDescriptor value)
	{
		if (value.OEAKCOHMIHH() == null || value.get_Type().GetTypeCode() != TypeCode.Object)
		{
			return false;
		}
		EOMGEBLLMMG aliasInfo;
		if (AOPBKMDMKDB.TryGetValue(value.OEAKCOHMIHH(), out aliasInfo))
		{
			if (aliasInfo.BJKKJNDLDJN == null)
			{
				aliasInfo.BJKKJNDLDJN = "o" + nextId.ToString(CultureInfo.InvariantCulture);
				nextId++;
			}
			return false;
		}
		AOPBKMDMKDB.Add(value.OEAKCOHMIHH(), new EOMGEBLLMMG());
		return true;
	}

	bool IObjectGraphVisitor.EnterMapping(IObjectDescriptor KGBGENDIMBC, IObjectDescriptor value)
	{
		return true;
	}

	bool IObjectGraphVisitor.EnterMapping(IPropertyDescriptor KGBGENDIMBC, IObjectDescriptor value)
	{
		return true;
	}

	void IObjectGraphVisitor.VisitScalar(IObjectDescriptor ADDIBOMFCNH)
	{
	}

	void IObjectGraphVisitor.VisitMappingStart(IObjectDescriptor JPEFEBICPFI, Type FHNELPLPIPI, Type EJGJHBGMCDM)
	{
	}

	void IObjectGraphVisitor.VisitMappingEnd(IObjectDescriptor JPEFEBICPFI)
	{
	}

	void IObjectGraphVisitor.VisitSequenceStart(IObjectDescriptor sequence, Type LKAAAFHOAGD)
	{
	}

	void IObjectGraphVisitor.VisitSequenceEnd(IObjectDescriptor sequence)
	{
	}

	string IAliasProvider.GetAlias(object target)
	{
		EOMGEBLLMMG aliasInfo;
		if (target != null && AOPBKMDMKDB.TryGetValue(target, out aliasInfo))
		{
			return aliasInfo.BJKKJNDLDJN;
		}
		return null;
	}
}
