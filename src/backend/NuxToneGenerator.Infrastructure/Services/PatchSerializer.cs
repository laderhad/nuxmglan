namespace NuxToneGenerator.Infrastructure.Services;

using NuxToneGenerator.Core.Interfaces;
using NuxToneGenerator.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class PatchSerializer : IPatchSerializer
{
    private const int PresetSize = 9902;
    private const int SceneSize = 140;
    private const int NameOffset = 0x6A;  // Offset of name within a scene block (106 decimal)
    private const int NameLength = 16;
    private const int ChainOffset = 0x5E; // Signal chain routing offset within scene
    private const int ChainLength = 12;   // 12 blocks in the routing chain
    private const int CabNameOffset = 0x1B2; // Absolute offset of cabinet name in preset
    private const int CabNameLength = 32;
    private const int IrDataOffset = 0x1D2;  // Start of WAV IR data
    private const int IrDataSize = 8236;     // RIFF WAV size
    private const int FreqDataSize = 1200;   // Frequency response data

    public byte[] Serialize(Preset preset)
    {
        var data = new byte[PresetSize];
        using var ms = new MemoryStream(data);
        using var writer = new BinaryWriter(ms);

        // Write preset index
        writer.Write(preset.Index);

        // Write Scene A
        WriteScene(writer, preset.SceneA, preset.Name);

        // Write Scene B
        WriteScene(writer, preset.SceneB, preset.Name);

        // Write 4-byte padding between Scene B and Scene C
        writer.Write(new byte[4]);

        // Write Scene C
        WriteScene(writer, preset.SceneC, preset.Name);

        // Write cabinet metadata (6 bytes flags + 32 bytes name)
        var cabFlags = new byte[6];
        if (!string.IsNullOrEmpty(preset.Cabinet.Name))
        {
            cabFlags[4] = 0x01; // IR active flag
        }
        writer.Write(cabFlags);

        var cabNameBytes = new byte[CabNameLength];
        if (!string.IsNullOrEmpty(preset.Cabinet.Name))
        {
            var nameBytes = Encoding.ASCII.GetBytes(preset.Cabinet.Name);
            Array.Copy(nameBytes, cabNameBytes, Math.Min(nameBytes.Length, CabNameLength));
        }
        writer.Write(cabNameBytes);

        // Write IR WAV data
        if (preset.ImpulseResponseData.Length > 0)
            writer.Write(preset.ImpulseResponseData);
        else
            writer.Write(new byte[IrDataSize]);

        // Write frequency response data
        if (preset.FrequencyResponseData.Length > 0)
            writer.Write(preset.FrequencyResponseData);
        else
            writer.Write(new byte[FreqDataSize]);

        return data;
    }

    private static void WriteScene(BinaryWriter writer, Scene scene, string presetName)
    {
        if (scene.RawData.Length == SceneSize)
        {
            // Round-trip: use the raw data but update the name
            var raw = (byte[])scene.RawData.Clone();
            var nameBytes = new byte[NameLength];
            var encoded = Encoding.ASCII.GetBytes(presetName ?? "");
            Array.Copy(encoded, nameBytes, Math.Min(encoded.Length, NameLength));
            Array.Copy(nameBytes, 0, raw, NameOffset, NameLength);
            writer.Write(raw);
        }
        else
        {
            // Generate a minimal scene block
            var raw = new byte[SceneSize];

            // Write signal chain at offset 0x5E
            var chain = scene.SignalChain;
            if (chain.Length >= ChainLength)
            {
                for (int i = 0; i < ChainLength; i++)
                    raw[ChainOffset + i] = (byte)chain[i];
            }
            else
            {
                // Default signal chain
                byte[] defaultChain = { 0x01, 0x05, 0x02, 0x03, 0x09, 0x04, 0x0A, 0x06, 0x07, 0x08, 0x0B, 0x00 };
                Array.Copy(defaultChain, 0, raw, ChainOffset, ChainLength);
            }

            // Write preset name at offset 0x6A
            var nameBytes = new byte[NameLength];
            var encoded = Encoding.ASCII.GetBytes(presetName ?? "");
            Array.Copy(encoded, nameBytes, Math.Min(encoded.Length, NameLength));
            Array.Copy(nameBytes, 0, raw, NameOffset, NameLength);

            writer.Write(raw);
        }
    }

    public Preset Deserialize(byte[] data, int presetIndex = 0)
    {
        if (data.Length < PresetSize)
            throw new ArgumentException("Invalid data length");

        var offset = presetIndex * PresetSize;
        if (offset + PresetSize > data.Length)
            throw new ArgumentException("Preset index out of bounds");

        using var ms = new MemoryStream(data, offset, PresetSize);
        using var reader = new BinaryReader(ms);

        var pIndex = reader.ReadInt32();
        var sceneARaw = reader.ReadBytes(SceneSize);
        var sceneBRaw = reader.ReadBytes(SceneSize);
        var padding = reader.ReadBytes(4);
        var sceneCRaw = reader.ReadBytes(SceneSize);

        var cabFlags = reader.ReadBytes(6);
        var cabNameBytes = reader.ReadBytes(CabNameLength);
        var cabName = Encoding.ASCII.GetString(cabNameBytes).TrimEnd('\0');

        var irData = reader.ReadBytes(IrDataSize);
        var freqData = reader.ReadBytes(FreqDataSize);

        // Parse name from Scene A at offset 0x6A
        var presetName = ExtractString(sceneARaw, NameOffset, NameLength);

        // Parse signal chain from Scene A at offset 0x5E
        var signalChain = new int[ChainLength];
        for (int i = 0; i < ChainLength; i++)
            signalChain[i] = sceneARaw[ChainOffset + i];

        return new Preset
        {
            Index = pIndex,
            Name = presetName,
            SceneA = new Scene
            {
                RawData = sceneARaw,
                PresetName = presetName,
                SignalChain = signalChain
            },
            SceneB = new Scene
            {
                RawData = sceneBRaw,
                PresetName = ExtractString(sceneBRaw, NameOffset, NameLength),
                SignalChain = ExtractSignalChain(sceneBRaw)
            },
            SceneC = new Scene
            {
                RawData = sceneCRaw,
                PresetName = ExtractString(sceneCRaw, NameOffset, NameLength),
                SignalChain = ExtractSignalChain(sceneCRaw)
            },
            Cabinet = new CabinetIR { Name = cabName },
            ImpulseResponseData = irData,
            FrequencyResponseData = freqData
        };
    }

    private static string ExtractString(byte[] data, int offset, int length)
    {
        if (offset + length > data.Length) return string.Empty;
        var bytes = new byte[length];
        Array.Copy(data, offset, bytes, 0, length);
        var str = Encoding.ASCII.GetString(bytes);
        var nullIdx = str.IndexOf('\0');
        return nullIdx >= 0 ? str[..nullIdx] : str;
    }

    private static int[] ExtractSignalChain(byte[] sceneData)
    {
        var chain = new int[ChainLength];
        for (int i = 0; i < ChainLength; i++)
            chain[i] = sceneData[ChainOffset + i];
        return chain;
    }

    public IReadOnlyList<Preset> DeserializeAll(byte[] data)
    {
        if (!ValidatePatch(data))
            throw new ArgumentException("Invalid patch file size");

        var count = data.Length / PresetSize;
        var list = new List<Preset>();
        for (int i = 0; i < count; i++)
        {
            list.Add(Deserialize(data, i));
        }
        return list;
    }

    public bool ValidatePatch(byte[] data)
    {
        return data != null && data.Length % PresetSize == 0 && data.Length > 0;
    }
}
