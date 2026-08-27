using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public sealed class Serializer
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IList<IYamlTypeConverter> BBFPDBKOOKF;

	private readonly PAFDJLCFOGH LHONCAIFCAF;

	private readonly INamingConvention LELOAKPLJEH;

	private readonly ITypeResolver CBMKGNIHPFO;

	internal IList<IYamlTypeConverter> JLJDNDGJEKC
	{
		get
		{
			return NGJMKPBNGPP();
		}
		private set
		{
			EEMMNEEEJPJ(value);
		}
	}

	public Serializer(PAFDJLCFOGH LHONCAIFCAF = PAFDJLCFOGH.None, INamingConvention LELOAKPLJEH = null)
	{
		this.LHONCAIFCAF = LHONCAIFCAF;
		this.LELOAKPLJEH = LELOAKPLJEH ?? new GCEHGHHBNOA();
		EEMMNEEEJPJ(new List<IYamlTypeConverter>());
		foreach (IYamlTypeConverter item in YamlTypeConverters.LGAMHDFHDHG())
		{
			NGJMKPBNGPP().Add(item);
		}
		object cBMKGNIHPFO;
		if (KCMBMCCNOHC(PAFDJLCFOGH.DefaultToStaticType))
		{
			ITypeResolver oEBJGLALCDH = new NBGCAMIJKDA();
			cBMKGNIHPFO = oEBJGLALCDH;
		}
		else
		{
			cBMKGNIHPFO = new CLPJGGPLMAB();
		}
		CBMKGNIHPFO = (ITypeResolver)cBMKGNIHPFO;
	}

	internal IList<IYamlTypeConverter> NGJMKPBNGPP()
	{
		return BBFPDBKOOKF;
	}

	private void EEMMNEEEJPJ(IList<IYamlTypeConverter> value)
	{
		BBFPDBKOOKF = value;
	}

	private bool KCMBMCCNOHC(PAFDJLCFOGH LFJBBPIDBCL)
	{
		return (LHONCAIFCAF & LFJBBPIDBCL) != 0;
	}

	public void JCPBBODBIBI(IYamlTypeConverter GMPKPHNBCHA)
	{
		NGJMKPBNGPP().Add(GMPKPHNBCHA);
	}

	public void Serialize(TextWriter writer, object OFDNAFPEAGP)
	{
		Serialize(new Emitter(writer), OFDNAFPEAGP);
	}

	public void Serialize(TextWriter writer, object OFDNAFPEAGP, Type LFLGCDNKNJI)
	{
		Serialize(new Emitter(writer), OFDNAFPEAGP, LFLGCDNKNJI);
	}

	public void Serialize(NEKGJNOFOFN NPIDIMCLNEM, object OFDNAFPEAGP)
	{
		if (NPIDIMCLNEM == null)
		{
			throw new ArgumentNullException("emitter");
		}
		ELFOILDKJMP(NPIDIMCLNEM, new ObjectDescriptor(OFDNAFPEAGP, (OFDNAFPEAGP == null) ? typeof(object) : OFDNAFPEAGP.GetType(), typeof(object)));
	}

	public void Serialize(NEKGJNOFOFN NPIDIMCLNEM, object OFDNAFPEAGP, Type LFLGCDNKNJI)
	{
		if (NPIDIMCLNEM == null)
		{
			throw new ArgumentNullException("emitter");
		}
		if (LFLGCDNKNJI == null)
		{
			throw new ArgumentNullException("type");
		}
		ELFOILDKJMP(NPIDIMCLNEM, new ObjectDescriptor(OFDNAFPEAGP, LFLGCDNKNJI, LFLGCDNKNJI));
	}

	private void ELFOILDKJMP(NEKGJNOFOFN NPIDIMCLNEM, IObjectDescriptor OFDNAFPEAGP)
	{
		BIGFDIOHKIG bIGFDIOHKIG = KPFMLJLAEPA();
		IEventEmitter oPIGMJHGIDL = KJGEMGOFAIA(NPIDIMCLNEM);
		IObjectGraphVisitor nKECMANOOEM = FFCJCHJGJOD(NPIDIMCLNEM, bIGFDIOHKIG, oPIGMJHGIDL, OFDNAFPEAGP);
		NPIDIMCLNEM.Emit(new StreamStart());
		NPIDIMCLNEM.Emit(new DocumentStart());
		bIGFDIOHKIG.Traverse(OFDNAFPEAGP, nKECMANOOEM);
		NPIDIMCLNEM.Emit(new DocumentEnd(true));
		NPIDIMCLNEM.Emit(new HNKFEGCMBJB());
	}

	private IObjectGraphVisitor FFCJCHJGJOD(NEKGJNOFOFN NPIDIMCLNEM, BIGFDIOHKIG PECHNBFNJJG, IEventEmitter OPIGMJHGIDL, IObjectDescriptor OFDNAFPEAGP)
	{
		IObjectGraphVisitor gDMFLLGPLNO = new EmittingObjectGraphVisitor(OPIGMJHGIDL);
		gDMFLLGPLNO = new CustomSerializationObjectGraphVisitor(NPIDIMCLNEM, gDMFLLGPLNO, NGJMKPBNGPP());
		if (!KCMBMCCNOHC(PAFDJLCFOGH.DisableAliases))
		{
			AnchorAssigner cKGCHJDJLCD = new AnchorAssigner();
			PECHNBFNJJG.Traverse(OFDNAFPEAGP, cKGCHJDJLCD);
			gDMFLLGPLNO = new AnchorAssigningObjectGraphVisitor(gDMFLLGPLNO, OPIGMJHGIDL, cKGCHJDJLCD);
		}
		if (!KCMBMCCNOHC(PAFDJLCFOGH.EmitDefaults))
		{
			gDMFLLGPLNO = new DefaultExclusiveObjectGraphVisitor(gDMFLLGPLNO);
		}
		return gDMFLLGPLNO;
	}

	private IEventEmitter KJGEMGOFAIA(NEKGJNOFOFN NPIDIMCLNEM)
	{
		DEFBJFOKOPM jDJEJDIJLLE = new DEFBJFOKOPM(NPIDIMCLNEM);
		if (KCMBMCCNOHC(PAFDJLCFOGH.JsonCompatible))
		{
			return new JsonEventEmitter(jDJEJDIJLLE);
		}
		return new TypeAssigningEventEmitter(jDJEJDIJLLE, KCMBMCCNOHC(PAFDJLCFOGH.Roundtrip));
	}

	private BIGFDIOHKIG KPFMLJLAEPA()
	{
		ITypeInspector cECGLIIIJJH = new ReadablePropertiesTypeInspector(CBMKGNIHPFO);
		if (KCMBMCCNOHC(PAFDJLCFOGH.Roundtrip))
		{
			cECGLIIIJJH = new ReadableAndWritablePropertiesTypeInspector(cECGLIIIJJH);
		}
		cECGLIIIJJH = new NamingConventionTypeInspector(cECGLIIIJJH, LELOAKPLJEH);
		cECGLIIIJJH = new YamlAttributesTypeInspector(cECGLIIIJJH);
		if (KCMBMCCNOHC(PAFDJLCFOGH.Roundtrip))
		{
			return new RoundtripObjectGraphTraversalStrategy(this, cECGLIIIJJH, CBMKGNIHPFO, 50);
		}
		return new FullObjectGraphTraversalStrategy(this, cECGLIIIJJH, CBMKGNIHPFO, 50);
	}
}
