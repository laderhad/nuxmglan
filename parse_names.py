with open('/Users/kerem/Documents/nuxmglan/MG30AllPatch.mg30patch', 'rb') as f:
    for i in range(5):
        f.seek(i * 9902 + 4)
        s1 = f.read(142)
        print(f"Preset {i}:", s1[104:106].hex(), s1[106:122].decode('ascii', 'ignore'), s1[122:136].hex())
