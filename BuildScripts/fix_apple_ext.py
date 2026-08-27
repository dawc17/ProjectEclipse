import re

def rw(p, s):
    open(p, 'w', encoding='utf-8', newline='').write(s)

def rd(p):
    return open(p, encoding='utf-8').read()

# 1) Recreate minimal LFFGCBPOGPJ (apple-flavor extension holder)
rw('Assets/Plugins/Assembly-CSharp-firstpass/LFFGCBPOGPJ.cs', '''public class LFFGCBPOGPJ : ADEKACKLIJG
{
\tpublic string ANHBIPONDNE()
\t{
\t\treturn null;
\t}

\tpublic void KJGFLKHCEJM()
\t{
\t}
}
''')
print('LFFGCBPOGPJ recreated')

# stop excluding it from compilation
p = 'BuildScripts/gen_rsp.py'
s = rd(p)
s = s.replace("', 'LFFGCBPOGPJ.cs", "'", 1)
s = s.replace("'LFFGCBPOGPJ.cs', ", '', 1)
rw(p, s)

# 2) Null-guard the four call sites
p = 'Assets/Scripts/Assembly-CSharp/NetworkController.cs'
s = rd(p)
old = 'else if (Application.platform == RuntimePlatform.IPhonePlayer && string.IsNullOrEmpty(ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>().ANHBIPONDNE()) && !DialogsOpener.MOAEBPJBDCD())'
new = ('else if (Application.platform == RuntimePlatform.IPhonePlayer\n'
       '\t\t\t\t\t&& !string.IsNullOrEmpty((ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>() ?? new LFFGCBPOGPJ()).ANHBIPONDNE())'
       ' == false && !DialogsOpener.MOAEBPJBDCD())')
# simpler + safer: keep semantics "receipt string empty" -> treat null as empty
new = ('else if (Application.platform == RuntimePlatform.IPhonePlayer\n'
       '\t\t\t\t\t&& string.IsNullOrEmpty((ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>() != null)\n'
       '\t\t\t\t\t\t? ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>().ANHBIPONDNE()\n'
       '\t\t\t\t\t\t: null) && !DialogsOpener.MOAEBPJBDCD())')
assert old in s
s = s.replace(old, new, 1)
rw(p, s)
print('NetworkController guarded')

p = 'Assets/Scripts/Assembly-CSharp/RemoteLicenseChecker.cs'
s = rd(p)
old = '\t\tstring text = ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>().ANHBIPONDNE();'
new = ('\t\tvar appleExt = ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>();\n'
       '\t\tstring text = (appleExt != null) ? appleExt.ANHBIPONDNE() : null;')
assert old in s
s = s.replace(old, new, 1)
old2 = '\t\tICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>().KJGFLKHCEJM();'
new2 = ('\t\tvar appleExt2 = ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>();\n'
        '\t\tif (appleExt2 != null)\n'
        '\t\t{\n'
        '\t\t\tappleExt2.KJGFLKHCEJM();\n'
        '\t\t}')
assert old2 in s
s = s.replace(old2, new2, 1)
rw(p, s)
print('RemoteLicenseChecker guarded')

p = 'Assets/Scripts/Assembly-CSharp/RaidCheatManager.cs'
s = rd(p)
old = '\t\t\t\tICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>().KJGFLKHCEJM();'
new = ('\t\t\t\tvar appleExt = ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>();\n'
       '\t\t\t\tif (appleExt != null)\n'
       '\t\t\t\t{\n'
       '\t\t\t\t\tappleExt.KJGFLKHCEJM();\n'
       '\t\t\t\t}')
assert old in s
s = s.replace(old, new, 1)
rw(p, s)
print('RaidCheatManager guarded')
