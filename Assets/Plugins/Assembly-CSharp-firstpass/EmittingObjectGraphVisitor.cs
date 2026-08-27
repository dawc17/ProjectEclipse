using System;

public sealed class EmittingObjectGraphVisitor : IObjectGraphVisitor
{
	private readonly IEventEmitter OPIGMJHGIDL;

	public EmittingObjectGraphVisitor(IEventEmitter OPIGMJHGIDL)
	{
		this.OPIGMJHGIDL = OPIGMJHGIDL;
	}

	bool IObjectGraphVisitor.Enter(IObjectDescriptor value)
	{
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
		OPIGMJHGIDL.Emit(new ScalarEventInfo(ADDIBOMFCNH));
	}

	void IObjectGraphVisitor.VisitMappingStart(IObjectDescriptor JPEFEBICPFI, Type FHNELPLPIPI, Type EJGJHBGMCDM)
	{
		OPIGMJHGIDL.Emit(new LPADMPIAIPF(JPEFEBICPFI));
	}

	void IObjectGraphVisitor.VisitMappingEnd(IObjectDescriptor JPEFEBICPFI)
	{
		OPIGMJHGIDL.Emit(new EKKDGIILGMA(JPEFEBICPFI));
	}

	void IObjectGraphVisitor.VisitSequenceStart(IObjectDescriptor sequence, Type LKAAAFHOAGD)
	{
		OPIGMJHGIDL.Emit(new PBGMOJFHMGI(sequence));
	}

	void IObjectGraphVisitor.VisitSequenceEnd(IObjectDescriptor sequence)
	{
		OPIGMJHGIDL.Emit(new NCGDJIDCIIM(sequence));
	}
}
