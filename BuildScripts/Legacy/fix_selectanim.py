p = 'Assets/Scripts/Assembly-CSharp/SelectAnimation.cs'
s = open(p, encoding='utf-8').read()

# Block 1: priority filter (outer+inner)
old1 = '''\t\t\tint num2 = 0;
\t\t\tforeach (SelectInfo item in MAHEJFLCCHP)
\t\t\t{
\t\t\t\tInfoAnimation.CapabilityTable iCANLHJKKNE = item.FGICHADOEHF.ICANLHJKKNE;
\t\t\t\tbool flag = true;
\t\t\t\tforeach (SelectInfo item2 in MAHEJFLCCHP)
\t\t\t\t{
\t\t\t\t\tif (!iCANLHJKKNE.IsThePriority(item2.FGICHADOEHF))
\t\t\t\t\t{
\t\t\t\t\t\tflag = false;
\t\t\t\t\t\tbreak;
\t\t\t\t\t}
\t\t\t\t}
\t\t\t\tif (flag)
\t\t\t\t{
\t\t\t\t\tMAHEJFLCCHP[num2] = item;
\t\t\t\t\tnum2++;
\t\t\t\t}
\t\t\t}'''
new1 = '''\t\t\tint num2 = 0;
\t\t\tfor (int i = 0; i < MAHEJFLCCHP.Count; i++)
\t\t\t{
\t\t\t\tInfoAnimation.CapabilityTable iCANLHJKKNE = MAHEJFLCCHP[i].FGICHADOEHF.ICANLHJKKNE;
\t\t\t\tbool flag = true;
\t\t\t\tfor (int j = 0; j < MAHEJFLCCHP.Count; j++)
\t\t\t\t{
\t\t\t\t\tif (!iCANLHJKKNE.IsThePriority(MAHEJFLCCHP[j].FGICHADOEHF))
\t\t\t\t\t{
\t\t\t\t\t\tflag = false;
\t\t\t\t\t\tbreak;
\t\t\t\t\t}
\t\t\t\t}
\t\t\t\tif (flag)
\t\t\t\t{
\t\t\t\t\tMAHEJFLCCHP[num2] = MAHEJFLCCHP[i];
\t\t\t\t\tnum2++;
\t\t\t\t}
\t\t\t}'''
assert old1 in s, 'block1'
s = s.replace(old1, new1, 1)

# Block 2: transition filter
old2 = '''\t\t\tint num3 = 0;
\t\t\tforeach (SelectInfo item3 in MAHEJFLCCHP)
\t\t\t{
\t\t\t\tif (item3.FGICHADOEHF.ODACDCDONJE.NIDNJFOGBFO.Count != 0)
'''
new2 = '''\t\t\tint num3 = 0;
\t\t\tfor (int l = 0; l < MAHEJFLCCHP.Count; l++)
\t\t\t{
\t\t\t\tSelectInfo item3 = MAHEJFLCCHP[l];
\t\t\t\tif (item3.FGICHADOEHF.ODACDCDONJE.NIDNJFOGBFO.Count != 0)
'''
assert old2 in s, 'block2 head'
s = s.replace(old2, new2, 1)

# Block 3: condition-keys filter
old3 = '''\t\tforeach (SelectInfo item4 in MAHEJFLCCHP)
\t\t{
\t\t\tConditionKeys bHDEBDIHDFM2 = item4.FGICHADOEHF.ILBCHANCOBP();'''
new3 = '''\t\tfor (int m = 0; m < MAHEJFLCCHP.Count; m++)
\t\t{
\t\t\tSelectInfo item4 = MAHEJFLCCHP[m];
\t\t\tConditionKeys bHDEBDIHDFM2 = item4.FGICHADOEHF.ILBCHANCOBP();'''
assert old3 in s, 'block3'
s = s.replace(old3, new3, 1)

open(p, 'w', encoding='utf-8', newline='').write(s)
print('PlayAnimationRandom loops converted')
