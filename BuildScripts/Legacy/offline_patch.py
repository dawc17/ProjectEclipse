import re

def rw(p, s):
    open(p, 'w', encoding='utf-8', newline='').write(s)

def rd(p):
    return open(p, encoding='utf-8').read()

# ============ 1) ServerProvider: global offline switch ============
p = 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/Core/Network/ServerProvider.cs'
s = rd(p)

anchor = '\t\tpublic void DownloadFile(string p_url, Action<byte[], string, string> p_onDownloadComplete, Action<float> MDJEOHMECHA = null, int DGDKHFPEHOG = 0)\n\t\t{\n\t\t\tif (InternetUtils.JLBPKAFHNNN())'
assert anchor in s, 'downloadfile anchor'
s = s.replace(
    '\tpublic void DownloadFile(string p_url, Action<byte[], string, string> p_onDownloadComplete, Action<float> MDJEOHMECHA = null, int DGDKHFPEHOG = 0)\n\t\t{\n\t\t\tif (InternetUtils.JLBPKAFHNNN())',
    '\tpublic static bool OFFLINE = true;\n\n\t\tpublic void DownloadFile(string p_url, Action<byte[], string, string> p_onDownloadComplete, Action<float> MDJEOHMECHA = null, int DGDKHFPEHOG = 0)\n\t\t{\n\t\t\tif (OFFLINE)\n\t\t\t{\n\t\t\t\tp_onDownloadComplete(new byte[0], "offline build", p_url);\n\t\t\t\treturn;\n\t\t\t}\n\t\t\tif (InternetUtils.JLBPKAFHNNN())', 1)

# POST requests: short-circuit inside DOINCEFMGCL
old_post = ('\t\tprivate IEnumerator DOINCEFMGCL(string OBPHDPKKNLO, WWWForm OLMGMKFEOIK, Action<bool, string, object> p_delegate = null, object JHJDJOFPHPH = null)\n'
            '\t\t{\n'
            '\t\t\tWWW wWW = new WWW(')
new_post = ('\t\tprivate IEnumerator DOINCEFMGCL(string OBPHDPKKNLO, WWWForm OLMGMKFEOIK, Action<bool, string, object> p_delegate = null, object JHJDJOFPHPH = null)\n'
            '\t\t{\n'
            '\t\t\tif (OFFLINE)\n'
            '\t\t\t{\n'
            '\t\t\t\tp_delegate?.Invoke(false, "offline build", JHJDJOFPHPH);\n'
            '\t\t\t\tyield break;\n'
            '\t\t\t}\n'
            '\t\t\tWWW wWW = new WWW(')
assert old_post in s, 'post anchor'
s = s.replace(old_post, new_post, 1)
rw(p, s)
print('ServerProvider offline switch installed')

# ============ 2) ListSF: auth timeout 15s -> 1s ============
p = 'Assets/Scripts/Assembly-CSharp/ListSF.cs'
s = rd(p)
old = '\t\tfloat num = AssemblyController.FNCNDGHCDLA() / 1000;'
assert old in s
s = s.replace(old, '\t\tfloat num = 1f;', 1)
rw(p, s)
print('auth timeout shortened')

# ============ 3) SF2Paths JNI silence in editor ============
p = 'Assets/Scripts/Assembly-CSharp/SF2Paths.cs'
s = rd(p)
old = '\tpublic static string CBFMFIHKMFI()\n\t{\n\t\tstring text = string.Empty;'
assert old in s
s = s.replace(old, '\tpublic static string CBFMFIHKMFI()\n\t{\n\t\tstring text = string.Empty;\n\t\tif (Application.isEditor)\n\t\t{\n\t\t\treturn text;\n\t\t}', 1)
rw(p, s)
print('SF2Paths editor guard')

# ============ 4) RemoteLicenseCache: never throw on decoy save ============
p = 'Assets/Scripts/Assembly-CSharp/RemoteLicenseCache.cs'
s = rd(p)
old = '\t\tforeach (KeyValuePair<string, string> item in list)\n\t\t{\n\t\t\txmlElement2.SetAttribute(item.Key, item.Value);\n\t\t}'
assert old in s
new = ('\t\tforeach (KeyValuePair<string, string> item in list)\n'
       '\t\t{\n'
       '\t\t\ttry\n'
       '\t\t\t{\n'
       '\t\t\t\txmlElement2.SetAttribute(item.Key, item.Value);\n'
       '\t\t\t}\n'
       '\t\t\tcatch\n'
       '\t\t\t{\n'
       '\t\t\t}\n'
       '\t\t}')
s = s.replace(old, new, 1)
# wrap whole method body defensively: catch around EncryptBytesToFile too
old2 = '\t\tAESUtils.EncryptBytesToFile(Encoding.UTF8.GetBytes(xmlDocument.OuterXml), Constants.ECHOPKKPDFD, Constants.MCCEADFMLGA, IFJOIBDOBPL());'
new2 = ('\t\ttry\n'
        '\t\t{\n'
        '\t\t\tAESUtils.EncryptBytesToFile(Encoding.UTF8.GetBytes(xmlDocument.OuterXml), Constants.ECHOPKKPDFD, Constants.MCCEADFMLGA, IFJOIBDOBPL());\n'
        '\t\t}\n'
        '\t\tcatch\n'
        '\t\t{\n'
        '\t\t}')
assert old2 in s
s = s.replace(old2, new2, 1)
rw(p, s)
print('RemoteLicenseCache hardened')
