import struct

def dump_preset(index):
    with open('/Users/kerem/Documents/nuxmglan/MG30AllPatch.mg30patch', 'rb') as f:
        data = f.read()
    
    start = index * 9902
    preset_data = data[start:start+9902]
    
    pid = struct.unpack('<I', preset_data[0:4])[0]
    print(f"Preset {index} ID: {pid}")
    
    # Let's see the 3 scenes
    for i in range(3):
        scene_start = 4 + i * 142
        scene_data = preset_data[scene_start : scene_start + 142]
        name = scene_data[104:136].replace(b'\x00', b'').decode('ascii', errors='ignore')
        print(f"  Scene {i+1} Name: {name}")
        # print hex
        print(f"    {scene_data[:16].hex()}")

dump_preset(0)
dump_preset(1)
