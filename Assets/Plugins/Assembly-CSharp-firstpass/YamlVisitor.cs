using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

public abstract class YamlVisitor : IYamlVisitor
{
	protected virtual void Visit(YamlStream ABJIEFMMIEK)
	{
	}

	protected virtual void BPONBLBIMOE(YamlStream ABJIEFMMIEK)
	{
	}

	protected virtual void Visit(YamlDocument DPMKHPJABAF)
	{
	}

	protected virtual void BPONBLBIMOE(YamlDocument DPMKHPJABAF)
	{
	}

	protected virtual void Visit(YamlScalarNode ADDIBOMFCNH)
	{
	}

	protected virtual void BPONBLBIMOE(YamlScalarNode ADDIBOMFCNH)
	{
	}

	protected virtual void Visit(YamlSequenceNode sequence)
	{
	}

	protected virtual void BPONBLBIMOE(YamlSequenceNode sequence)
	{
	}

	protected virtual void Visit(YamlMappingNode JPEFEBICPFI)
	{
	}

	protected virtual void BPONBLBIMOE(YamlMappingNode JPEFEBICPFI)
	{
	}

	protected virtual void GPLMILONKGG(YamlStream ABJIEFMMIEK)
	{
		foreach (YamlDocument document in ABJIEFMMIEK.Documents)
		{
			document.GPHIFFOGOGN(this);
		}
	}

	protected virtual void GPLMILONKGG(YamlDocument DPMKHPJABAF)
	{
		if (DPMKHPJABAF.RootNode != null)
		{
			DPMKHPJABAF.RootNode.GPHIFFOGOGN(this);
		}
	}

	protected virtual void GPLMILONKGG(YamlSequenceNode sequence)
	{
		foreach (YamlNode child in sequence.Children)
		{
			child.GPHIFFOGOGN(this);
		}
	}

	protected virtual void GPLMILONKGG(YamlMappingNode JPEFEBICPFI)
	{
		foreach (KeyValuePair<YamlNode, YamlNode> child in JPEFEBICPFI.Children)
		{
			child.Key.GPHIFFOGOGN(this);
			child.Value.GPHIFFOGOGN(this);
		}
	}

	void IYamlVisitor.Visit(YamlStream ABJIEFMMIEK)
	{
		Visit(ABJIEFMMIEK);
		GPLMILONKGG(ABJIEFMMIEK);
		BPONBLBIMOE(ABJIEFMMIEK);
	}

	void IYamlVisitor.Visit(YamlDocument DPMKHPJABAF)
	{
		Visit(DPMKHPJABAF);
		GPLMILONKGG(DPMKHPJABAF);
		BPONBLBIMOE(DPMKHPJABAF);
	}

	void IYamlVisitor.Visit(YamlScalarNode ADDIBOMFCNH)
	{
		Visit(ADDIBOMFCNH);
		BPONBLBIMOE(ADDIBOMFCNH);
	}

	void IYamlVisitor.Visit(YamlSequenceNode sequence)
	{
		Visit(sequence);
		GPLMILONKGG(sequence);
		BPONBLBIMOE(sequence);
	}

	void IYamlVisitor.Visit(YamlMappingNode JPEFEBICPFI)
	{
		Visit(JPEFEBICPFI);
		GPLMILONKGG(JPEFEBICPFI);
		BPONBLBIMOE(JPEFEBICPFI);
	}
}
