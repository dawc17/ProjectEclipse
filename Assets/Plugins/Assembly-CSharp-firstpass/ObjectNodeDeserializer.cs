using System;

public sealed class ObjectNodeDeserializer : INodeDeserializer
{
	private readonly IObjectFactory IEBGHNHEOBB;

	private readonly ITypeInspector APLJDMHILEN;

	private readonly bool _ignoreUnmatched;

	public ObjectNodeDeserializer(IObjectFactory EJPHFDCKCCE, ITypeInspector GIJPGEHPILC, bool GNFDAJLHBCN)
	{
		IEBGHNHEOBB = EJPHFDCKCCE;
		APLJDMHILEN = GIJPGEHPILC;
		_ignoreUnmatched = GNFDAJLHBCN;
	}

	bool INodeDeserializer.Deserialize(EventReader reader, Type MBLGNMBFHBI, Func<EventReader, Type, object> IJBAEAEDMCC, out object value)
	{
		MappingStart oGMPNFCPPDH = reader.GNNPKHDPGLN<MappingStart>();
		if (oGMPNFCPPDH == null)
		{
			value = null;
			return false;
		}
		value = IEBGHNHEOBB.Create(MBLGNMBFHBI);
		while (!reader.GPHIFFOGOGN<BLFPJCPALDH>())
		{
			Scalar lEACOCDHICF = reader.DODGGCGJJLL<Scalar>();
			IPropertyDescriptor JLCGLCLEGBD = APLJDMHILEN.DBLHKMEGOEK(MBLGNMBFHBI, null, lEACOCDHICF.OEAKCOHMIHH(), _ignoreUnmatched);
			if (JLCGLCLEGBD == null)
			{
				reader.FHCPPKNIOKB();
				continue;
			}
			object obj = IJBAEAEDMCC(reader, JLCGLCLEGBD.get_Type());
			IValuePromise aGAMFLELGLG = obj as IValuePromise;
			if (aGAMFLELGLG == null)
			{
				object bAINMLLIKOL = TypeConverterHelper.ChangeType(obj, JLCGLCLEGBD.get_Type());
				JLCGLCLEGBD.Write(value, bAINMLLIKOL);
				continue;
			}
			object valueRef = value;
			aGAMFLELGLG.add_ValueAvailable((object AFIEJABPAKA) =>
			{
				object bAINMLLIKOL2 = TypeConverterHelper.ChangeType(AFIEJABPAKA, JLCGLCLEGBD.get_Type());
				JLCGLCLEGBD.Write(valueRef, bAINMLLIKOL2);
			});
		}
		reader.DODGGCGJJLL<BLFPJCPALDH>();
		return true;
	}
}
