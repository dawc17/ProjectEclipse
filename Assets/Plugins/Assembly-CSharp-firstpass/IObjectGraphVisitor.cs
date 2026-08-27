using System;

public interface IObjectGraphVisitor
{
	bool Enter(IObjectDescriptor value);

	bool EnterMapping(IObjectDescriptor KGBGENDIMBC, IObjectDescriptor value);

	bool EnterMapping(IPropertyDescriptor KGBGENDIMBC, IObjectDescriptor value);

	void VisitScalar(IObjectDescriptor ADDIBOMFCNH);

	void VisitMappingStart(IObjectDescriptor JPEFEBICPFI, Type FHNELPLPIPI, Type EJGJHBGMCDM);

	void VisitMappingEnd(IObjectDescriptor JPEFEBICPFI);

	void VisitSequenceStart(IObjectDescriptor sequence, Type LKAAAFHOAGD);

	void VisitSequenceEnd(IObjectDescriptor sequence);
}
