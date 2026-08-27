p = 'Assets/Scripts/Assembly-CSharp/ResourceManager.cs'
s = open(p, encoding='utf-8').read()

# ensure HashSet available
if 'using System.Collections.Generic;' not in s:
    s = s.replace('using System.IO;', 'using System.Collections.Generic;\nusing System.IO;', 1)

helpers = '''
\t\tprivate static string _devXmlRoot;
\t\tprivate static bool _devXmlRootInit;
\t\tprivate static readonly HashSet<string> _devXmlLogged = new HashSet<string>();

\t\tprivate static string GetDevXmlRoot()
\t\t{
\t\t\tif (!_devXmlRootInit)
\t\t\t{
\t\t\t\t_devXmlRootInit = true;
\t\t\t\ttry
\t\t\t\t{
\t\t\t\t\t_devXmlRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "xml"));
\t\t\t\t}
\t\t\t\tcatch
\t\t\t\t{
\t\t\t\t\t_devXmlRoot = null;
\t\t\t\t}
\t\t\t}
\t\t\treturn _devXmlRoot;
\t\t}

\t\tpublic static bool TryDevXml(string ONEIGMLOGDC, out string text)
\t\t{
\t\t\ttext = null;
\t\t\tstring root = GetDevXmlRoot();
\t\t\tif (root == null || !Directory.Exists(root))
\t\t\t{
\t\t\t\treturn false;
\t\t\t}
\t\t\tstring rel = ONEIGMLOGDC.Replace('\\\\', '/');
\t\t\trel = rel.TrimStart('/');
\t\t\tvar candidates = new List<string>();
\t\t\tcandidates.Add(rel);
\t\t\tint k = rel.IndexOf("gamedata/", StringComparison.OrdinalIgnoreCase);
\t\t\tif (k > 0)
\t\t\t{
\t\t\t\tcandidates.Add(rel.Substring(k + "gamedata/".Length));
\t\t\t}
\t\t\tcandidates.Add(Path.GetFileName(rel));
\t\t\tforeach (string cand in candidates)
\t\t\t{
\t\t\t\tif (string.IsNullOrEmpty(cand))
\t\t\t\t{
\t\t\t\t\tcontinue;
\t\t\t\t}
\t\t\t\tstring file = Path.Combine(root, cand.Replace('/', Path.DirectorySeparatorChar));
\t\t\t\tif (!File.Exists(file) && !file.EndsWith(".xml") && !file.EndsWith(".json"))
\t\t\t\t{
\t\t\t\t\tfile = file + ".xml";
\t\t\t\t}
\t\t\t\tif (File.Exists(file))
\t\t\t\t{
\t\t\t\t\ttry
\t\t\t\t\t{
\t\t\t\t\t\ttext = File.ReadAllText(file);
\t\t\t\t\t}
\t\t\t\t\tcatch
\t\t\t\t\t{
\t\t\t\t\t\treturn false;
\t\t\t\t\t}
\t\t\t\t\tif (_devXmlLogged.Add(file))
\t\t\t\t\t{
\t\t\t\t\t\tDebug.Log("[DevXml] override: " + ONEIGMLOGDC + " -> " + file);
\t\t\t\t\t}
\t\t\t\t\treturn true;
\t\t\t\t}
\t\t\t}
\t\t\treturn false;
\t\t}

'''

anchor = '\tpublic static string GetText(string ONEIGMLOGDC, bool GIEAPLJHHDK = false)\n\t{\n'
assert anchor in s
s = s.replace(anchor, helpers + anchor.replace('{', '{\n\t\tif (TryDevXml(ONEIGMLOGDC, out var t0))\n\t\t{\n\t\t\treturn t0;\n\t\t}\n'), 1)

anchor2 = '\tpublic static string IJMMFCDCOAC(string ONEIGMLOGDC)\n\t{\n'
assert anchor2 in s
s = s.replace(anchor2, anchor2 + '\t\tif (TryDevXml(ONEIGMLOGDC, out var t1))\n\t\t{\n\t\t\treturn t1;\n\t\t}\n', 1)

anchor3 = '\tpublic static string KIHHJGJKMIC(string ONEIGMLOGDC)\n\t{\n'
assert anchor3 in s
s = s.replace(anchor3, anchor3 + '\t\tif (TryDevXml(ONEIGMLOGDC, out var t2))\n\t\t{\n\t\t\treturn t2;\n\t\t}\n', 1)

open(p, 'w', encoding='utf-8', newline='').write(s)
print('ResourceManager hooked')
