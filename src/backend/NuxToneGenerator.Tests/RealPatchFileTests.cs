namespace NuxToneGenerator.Tests;

using NuxToneGenerator.Infrastructure.Services;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;

/// <summary>
/// Integration tests using the actual MG30AllPatch.mg30patch reference file.
/// These tests verify the serializer against real-world data.
/// </summary>
public class RealPatchFileTests
{
    private readonly PatchSerializer _serializer = new();
    private const string PatchFilePath = "../../../../../../MG30AllPatch.mg30patch";

    private byte[]? TryLoadPatchFile()
    {
        var path = Path.GetFullPath(Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, PatchFilePath));
        if (!File.Exists(path)) return null;
        return File.ReadAllBytes(path);
    }

    [SkippableFact]
    public void RealFile_ValidatesCorrectly()
    {
        var data = TryLoadPatchFile();
        Skip.If(data == null, "Reference patch file not found");
        Assert.True(_serializer.ValidatePatch(data));
    }

    [SkippableFact]
    public void RealFile_Deserializes128Presets()
    {
        var data = TryLoadPatchFile();
        Skip.If(data == null, "Reference patch file not found");

        var presets = _serializer.DeserializeAll(data);
        Assert.Equal(128, presets.Count);
    }

    [SkippableFact]
    public void RealFile_FirstPresetHasCorrectName()
    {
        var data = TryLoadPatchFile();
        Skip.If(data == null, "Reference patch file not found");

        var presets = _serializer.DeserializeAll(data);
        Assert.Equal("Ac30 + SteelSing", presets[0].Name);
    }

    [SkippableFact]
    public void RealFile_PresetIndicesAreSequential()
    {
        var data = TryLoadPatchFile();
        Skip.If(data == null, "Reference patch file not found");

        var presets = _serializer.DeserializeAll(data);
        for (int i = 0; i < presets.Count; i++)
        {
            Assert.Equal(i, presets[i].Index);
        }
    }

    [SkippableFact]
    public void RealFile_AllPresetsHaveNonEmptyNames()
    {
        var data = TryLoadPatchFile();
        Skip.If(data == null, "Reference patch file not found");

        var presets = _serializer.DeserializeAll(data);
        foreach (var preset in presets)
        {
            Assert.False(string.IsNullOrWhiteSpace(preset.Name),
                $"Preset {preset.Index} has empty name");
        }
    }

    [SkippableFact]
    public void RealFile_KnownPresetNames()
    {
        var data = TryLoadPatchFile();
        Skip.If(data == null, "Reference patch file not found");

        var presets = _serializer.DeserializeAll(data);
        var names = presets.Select(p => p.Name).ToList();

        Assert.Contains("Izmir + Phaser", names);
        Assert.Contains("Clean + TS9", names);
        Assert.Contains("JCM + Boost", names);
        Assert.Contains("KEREM LEAD", names);
        Assert.Contains("SLASH LEAD V2", names);
    }

    [SkippableFact]
    public void RealFile_CabinetNamesExtracted()
    {
        var data = TryLoadPatchFile();
        Skip.If(data == null, "Reference patch file not found");

        var presets = _serializer.DeserializeAll(data);
        var withCabs = presets.Where(p => !string.IsNullOrEmpty(p.Cabinet.Name)).ToList();

        Assert.True(withCabs.Count > 0, "No presets have cabinet names");
        Assert.Contains(withCabs, p => p.Cabinet.Name == "ML-MEGA-GREEN-MIX-CL");
        Assert.Contains(withCabs, p => p.Cabinet.Name == "LT TV Mix");
    }

    [SkippableFact]
    public void RealFile_SignalChainsHaveValidValues()
    {
        var data = TryLoadPatchFile();
        Skip.If(data == null, "Reference patch file not found");

        var presets = _serializer.DeserializeAll(data);
        foreach (var preset in presets)
        {
            Assert.Equal(12, preset.SceneA.SignalChain.Length);
            // All values should be in range 0-11
            foreach (var block in preset.SceneA.SignalChain)
            {
                Assert.InRange(block, 0, 11);
            }
        }
    }

    [SkippableFact]
    public void RealFile_VerifiesSceneAndCabinetBoundaries()
    {
        var data = TryLoadPatchFile();
        Skip.If(data == null, "Reference patch file not found");

        const int presetSize = 9902;
        const int sceneSize = 142;
        const int sceneNameOffset = 106;
        const int cabinetOffset = 430;
        const int cabinetNameOffset = 434;

        Assert.Equal(128 * presetSize, data!.Length);
        Assert.Equal(0, BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(0, 4)));
        Assert.Equal("Ac30 + SteelSing", ReadAscii(data, 4 + sceneNameOffset, 16));
        Assert.Equal("Ac30 + SteelSing", ReadAscii(data, sceneSize + 4 + sceneNameOffset, 16));
        Assert.Equal("Ac30 + SteelSing", ReadAscii(data, 2 * sceneSize + 4 + sceneNameOffset, 16));
        Assert.Equal("ML-MEGA-GREEN-MIX-CL", ReadAscii(data, cabinetNameOffset, 32));
        Assert.Equal(new byte[] { 1, 0, 0, 0 }, data.Skip(cabinetOffset).Take(4));
    }

    [SkippableFact]
    public void RealFile_VerifiesSignalChainAndRiffBoundaries()
    {
        var data = TryLoadPatchFile();
        Skip.If(data == null, "Reference patch file not found");

        const int presetSize = 9902;
        const int sceneStart = 4;
        const int sceneSize = 142;
        const int chainOffset = 94;
        const int riffOffset = 466;
        var expectedChain = new byte[] { 5, 0, 1, 2, 3, 9, 4, 10, 6, 8, 7, 11 };
        Assert.Equal(expectedChain, data!.Skip(sceneStart + chainOffset).Take(expectedChain.Length));
        Assert.Equal("RIFF", System.Text.Encoding.ASCII.GetString(data, riffOffset, 4));
        Assert.Equal(8228, BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(riffOffset + 4, 4)));
        Assert.Equal("RIFF", System.Text.Encoding.ASCII.GetString(data, presetSize + riffOffset, 4));
        Assert.Equal(0, data[presetSize + sceneStart + 3 * sceneSize - 1]);
        Assert.Equal(presetSize, data.Length / 128);
        Assert.Equal(0, data.Length % presetSize);
    }

    [SkippableFact]
    public void RealFile_RoundTripPreservesData()
    {
        var data = TryLoadPatchFile();
        Skip.If(data == null, "Reference patch file not found");

        // Round-trip the first preset
        var original = _serializer.Deserialize(data, 0);
        var serialized = _serializer.Serialize(original);
        var deserialized = _serializer.Deserialize(serialized, 0);

        Assert.Equal(original.Index, deserialized.Index);
        Assert.Equal(original.Name, deserialized.Name);
        Assert.Equal(original.Cabinet.Name, deserialized.Cabinet.Name);

        // IR data should be preserved exactly
        Assert.Equal(original.ImpulseResponseData, deserialized.ImpulseResponseData);
        Assert.Equal(original.FrequencyResponseData, deserialized.FrequencyResponseData);
    }

    private static string ReadAscii(byte[] data, int offset, int length)
    {
        return System.Text.Encoding.ASCII.GetString(data, offset, length).TrimEnd('\0');
    }
}
