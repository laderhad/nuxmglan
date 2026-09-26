import sys
with open('/Users/kerem/Documents/nuxmglan/MG30AllPatch.mg30patch', 'rb') as f:
    data = f.read()

presets = 128
all_params = []
for i in range(presets):
    for j in range(3):
        start = i * 9902 + 4 + j * 142
        params = data[start+12 : start+94]
        all_params.append(params)

for i in range(82):
    vals = set([p[i] for p in all_params])
    print(f"Byte {i+12:02d}: min={min(vals):3d}, max={max(vals):3d}, unique={len(vals)}")
