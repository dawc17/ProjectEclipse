using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace Nekki.Yaml
{
	[Serializable]
	public class YamlDocumentNekki
	{
		private YamlStream _yamlStream;

		private YamlDocument _yamlDocument;

		private Mapping _rootMapping;

		private string _content;

		public YamlDocumentNekki()
		{
			_yamlStream = new YamlStream();
		}

		public static YamlDocumentNekki IBKNALEEBNI(string PMFEIPCHENB)
		{
			if (!File.Exists(PMFEIPCHENB))
			{
				AdvLog.CCOFFJPPAKC("YAML file is not exists!!!");
				return null;
			}
			using (TextReader textReader = new StreamReader(PMFEIPCHENB))
			{
				return LGCMPFBMDFJ(textReader.ReadToEnd());
			}
		}

		public static YamlDocumentNekki LGCMPFBMDFJ(string DNHDOEEDDBD)
		{
			YamlDocumentNekki yamlDocumentNekki = new YamlDocumentNekki();
			yamlDocumentNekki._yamlStream.Load(new StringReader(DNHDOEEDDBD));
			yamlDocumentNekki._yamlDocument = yamlDocumentNekki._yamlStream.Documents[0];
			if (yamlDocumentNekki._yamlDocument.RootNode is YamlMappingNode)
			{
				yamlDocumentNekki._rootMapping = new Mapping("Root", (YamlMappingNode)yamlDocumentNekki._yamlDocument.RootNode);
			}
			yamlDocumentNekki._content = DNHDOEEDDBD;
			return yamlDocumentNekki;
		}

		public override string ToString()
		{
			return _yamlDocument.ToString();
		}

		public void GGGEHAGCLGC(string PMFEIPCHENB, bool EENMGCCBIHF = true)
		{
			using (TextWriter jGEEEDKMKKH = new StreamWriter(PMFEIPCHENB, false, Encoding.UTF8))
			{
				_yamlStream.Save(jGEEEDKMKKH, EENMGCCBIHF);
			}
		}

		public string GOGLGIJMFGG()
		{
			StringWriter stringWriter = new StringWriter();
			_yamlStream.Save(stringWriter);
			return stringWriter.ToString();
		}

		public Node GetRoot(string name)
		{
			return _rootMapping.GetNode(name);
		}

		public Mapping GetRoot(int index = 0)
		{
			return _rootMapping;
		}

		public void Serialize(string PMFEIPCHENB)
		{
			TextReader nILNDHEKNLJ = new StringReader(_content);
			Deserializer iLDIAJJOJJD = new Deserializer();
			object obj = iLDIAJJOJJD.Deserialize(nILNDHEKNLJ);
			if (obj == null)
			{
				return;
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (FileStream serializationStream = File.OpenWrite(PMFEIPCHENB))
			{
				binaryFormatter.Serialize(serializationStream, obj);
			}
		}
	}
}
