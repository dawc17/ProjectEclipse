using System;

public abstract class ChainedObjectGraphVisitor : IObjectGraphVisitor
{
	private readonly IObjectGraphVisitor GDMFLLGPLNO;

	protected ChainedObjectGraphVisitor(IObjectGraphVisitor GDMFLLGPLNO)
	{
		this.GDMFLLGPLNO = GDMFLLGPLNO;
	}

	public virtual bool Enter(IObjectDescriptor value)
	{
		return GDMFLLGPLNO.Enter(value);
	}

	public virtual bool EnterMapping(IObjectDescriptor KGBGENDIMBC, IObjectDescriptor value)
	{
		return GDMFLLGPLNO.EnterMapping(KGBGENDIMBC, value);
	}

	public virtual bool EnterMapping(IPropertyDescriptor KGBGENDIMBC, IObjectDescriptor value)
	{
		return GDMFLLGPLNO.EnterMapping(KGBGENDIMBC, value);
	}

	public virtual void VisitScalar(IObjectDescriptor ADDIBOMFCNH)
	{
		GDMFLLGPLNO.VisitScalar(ADDIBOMFCNH);
	}

	public virtual void VisitMappingStart(IObjectDescriptor JPEFEBICPFI, Type FHNELPLPIPI, Type EJGJHBGMCDM)
	{
		GDMFLLGPLNO.VisitMappingStart(JPEFEBICPFI, FHNELPLPIPI, EJGJHBGMCDM);
	}

	public virtual void VisitMappingEnd(IObjectDescriptor JPEFEBICPFI)
	{
		GDMFLLGPLNO.VisitMappingEnd(JPEFEBICPFI);
	}

	public virtual void VisitSequenceStart(IObjectDescriptor sequence, Type LKAAAFHOAGD)
	{
		GDMFLLGPLNO.VisitSequenceStart(sequence, LKAAAFHOAGD);
	}

	public virtual void VisitSequenceEnd(IObjectDescriptor sequence)
	{
		GDMFLLGPLNO.VisitSequenceEnd(sequence);
	}
}
