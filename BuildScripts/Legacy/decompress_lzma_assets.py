import os
import lzma

roots = ['Assets']
decoded = []
skipped = []
failed = []

for dp, dns, fns in os.walk('Assets'):
    for fn in fns:
        p = os.path.join(dp, fn)
        if not fn.endswith(('.bytes', '.txt', '.asset')):
            continue
        try:
            data = open(p, 'rb').read()
        except Exception:
            continue
        if len(data) < 20 or data[0] >= 225:
            continue
        # LZMA_ALONE heuristic: props byte, plausible dict size, plausible size field
        import struct
        props = data[0]
        dicosz = struct.unpack('<I', data[1:5])[0]
        usize = struct.unpack('<Q', data[5:13])[0]
        if props >= 225 or props == 0:
            continue
        if usize > (1 << 34) or usize == 0:
            continue
        try:
            out = lzma.decompress(data, format=lzma.FORMAT_ALONE)
        except Exception:
            failed.append((p, 'decode-fail'))
            continue
        decoded.append((p, len(data), len(out)))
        open(p, 'wb').write(out)

print('decoded:', len(decoded))
for p, a, b in decoded[:30]:
    print(f'  {p}  {a} -> {b}')
