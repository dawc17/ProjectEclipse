using System;
using System.Collections.Generic;

public sealed class AnchorAssigningObjectGraphVisitor : ChainedObjectGraphVisitor
{
	private readonly IEventEmitter OPIGMJHGIDL;

	private readonly IAliasProvider JNNJMIPHLBI;

	private readonly HashSet<string> emittedAliases = new HashSet<string>();

	public AnchorAssigningObjectGraphVisitor(IObjectGraphVisitor GDMFLLGPLNO, IEventEmitter OPIGMJHGIDL, IAliasProvider JNNJMIPHLBI)
		: base(GDMFLLGPLNO)
	{
		this.OPIGMJHGIDL = OPIGMJHGIDL;
		this.JNNJMIPHLBI = JNNJMIPHLBI;
	}

	public override bool Enter(IObjectDescriptor value)
	{
		string text = JNNJMIPHLBI.GetAlias(value.OEAKCOHMIHH());
		if (text != null && !emittedAliases.Add(text))
		{
			IEventEmitter oPIGMJHGIDL = OPIGMJHGIDL;
			AliasEventInfo nDOLNPCPJCJ = new AliasEventInfo(value);
			nDOLNPCPJCJ.set_Alias(text);
			oPIGMJHGIDL.Emit(nDOLNPCPJCJ);
			return false;
		}
		return base.Enter(value);
	}

	public override void VisitMappingStart(IObjectDescriptor JPEFEBICPFI, Type FHNELPLPIPI, Type EJGJHBGMCDM)
	{
		IEventEmitter oPIGMJHGIDL = OPIGMJHGIDL;
		LPADMPIAIPF lPADMPIAIPF = new LPADMPIAIPF(JPEFEBICPFI);
		lPADMPIAIPF.PAANCDPFGCI(JNNJMIPHLBI.GetAlias(JPEFEBICPFI.OEAKCOHMIHH()));
		oPIGMJHGIDL.Emit(lPADMPIAIPF);
	}

	public override void VisitSequenceStart(IObjectDescriptor sequence, Type LKAAAFHOAGD)
	{
		IEventEmitter oPIGMJHGIDL = OPIGMJHGIDL;
		PBGMOJFHMGI pBGMOJFHMGI = new PBGMOJFHMGI(sequence);
		pBGMOJFHMGI.PAANCDPFGCI(JNNJMIPHLBI.GetAlias(sequence.OEAKCOHMIHH()));
		oPIGMJHGIDL.Emit(pBGMOJFHMGI);
	}

	public override void VisitScalar(IObjectDescriptor ADDIBOMFCNH)
	{
		IEventEmitter oPIGMJHGIDL = OPIGMJHGIDL;
		ScalarEventInfo gEPIKBBFJED = new ScalarEventInfo(ADDIBOMFCNH);
		gEPIKBBFJED.PAANCDPFGCI(JNNJMIPHLBI.GetAlias(ADDIBOMFCNH.OEAKCOHMIHH()));
		oPIGMJHGIDL.Emit(gEPIKBBFJED);
	}
}
