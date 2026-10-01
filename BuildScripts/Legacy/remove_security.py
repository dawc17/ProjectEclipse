import re
import os

p = 'Assets/Plugins/Assembly-CSharp-firstpass/KNCKFKIOPHE.cs'
s = open(p, encoding='utf-8').read()

s = s.replace('using UnityEngine.Purchasing.Security;\n', '')

pat_apple = re.compile(
    r"\n?\tpublic static string Log\(this AppleInAppPurchaseReceipt [^\n]*\)\n\t\{.*?\n\t\}\n?",
    re.S)
pat_gp = re.compile(
    r"\n?\tpublic static string Log\(this GooglePlayReceipt [^\n]*\)\n\t\{.*?\n\t\}\n?",
    re.S)

s2, na = pat_apple.subn('', s)
s3, ng = pat_gp.subn('', s2)
print('apple removed:', na, '| gp removed:', ng)
assert na == 1 and ng == 1

assert 'Purchasing.Security' not in s3
assert 'AppleInAppPurchaseReceipt' not in s3
assert 'GooglePlayReceipt' not in s3

open(p, 'w', encoding='utf-8', newline='').write(s3)

os.remove('Assets/Plugins/Security.dll')
os.remove('Assets/Plugins/Security.dll.meta')
print('Security.dll removed')
