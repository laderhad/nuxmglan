import sys

def parse(filename):
    with open(filename, 'rb') as f:
        data = f.read()
    print(f"Total size: {len(data)} bytes")
    
    # Let's find all ABM tags
    import re
    matches = [m.start() for m in re.finditer(b'ABM', data)]
    print(f"Found {len(matches)} 'ABM' tags at: {matches[:10]}")
    
    # What about 'Ac30 + SteelSing'?
    name = b'Ac30 + SteelSing'
    pos = data.find(name)
    print(f"Found {name} at {pos}")
    
    # What about RIFF?
    riffs = [m.start() for m in re.finditer(b'RIFF', data)]
    print(f"Found {len(riffs)} 'RIFF' tags at: {riffs[:10]}")
    
    if len(riffs) > 0:
        for i in range(min(5, len(riffs))):
            r_start = riffs[i]
            r_size = int.from_bytes(data[r_start+4:r_start+8], byteorder='little')
            print(f"RIFF {i} at {r_start}, size {r_size} (ends at {r_start + 8 + r_size})")

parse('/Users/kerem/Documents/nuxmglan/MG30AllPatch.mg30patch')
