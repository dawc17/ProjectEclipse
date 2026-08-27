p = 'Assets/Plugins/Assembly-CSharp-firstpass/PEILCKGHHDP.cs'
s = open(p, encoding='utf-8').read()

old = '''\t\tif (!PCJAKPJMKGN.ContainsKey("AndroidPublicKey"))
\t\t{
\t\t\tDebug.LogError("[Store_Android] PublicKey is missing!");
\t\t\treturn;
\t\t}
\t\tConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
\t\tconfigurationBuilder.Configure<IGooglePlayConfiguration>().SetPublicKey((string)PCJAKPJMKGN["AndroidPublicKey"]);
\t\tconfigurationBuilder.AddProducts(OCMDJBDPLJK);
\t\tUnityPurchasing.Initialize(this, configurationBuilder);'''

new = '''\t\tDebug.Log("[Store_Android] Purchasing is disabled in this build.");'''

assert old in s, 'ctor pattern not found'
s = s.replace(old, new, 1)
open(p, 'w', encoding='utf-8', newline='').write(s)
print('PEILCKGHHDP gutted')
