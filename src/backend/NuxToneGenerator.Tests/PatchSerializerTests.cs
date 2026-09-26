namespace NuxToneGenerator.Tests;

using NuxToneGenerator.Infrastructure.Services;
using NuxToneGenerator.Core.Models;
using System.IO;
using System.Text;

public class PatchSerializerTests
{
    private readonly PatchSerializer _serializer = new();

    [Fact]
    public void ValidatePatch_ValidFile_ReturnsTrue()
    {
        var data = new byte[9902]; // One preset
        Assert.True(_serializer.ValidatePatch(data));
    }

    [Fact]
    public void ValidatePatch_MultiplePresets_ReturnsTrue()
    {
        var data = new byte[9902 * 128]; // 128 presets
        Assert.True(_serializer.ValidatePatch(data));
    }

    [Fact]
    public void ValidatePatch_InvalidSize_ReturnsFalse()
    {
        var data = new byte[1000]; // Not a multiple of 9902
        Assert.False(_serializer.ValidatePatch(data));
    }

    [Fact]
    public void ValidatePatch_EmptyData_ReturnsFalse()
    {
        var data = Array.Empty<byte>();
        Assert.False(_serializer.ValidatePatch(data));
    }

    [Fact]
    public void Deserialize_ExtractsPresetIndex()
    {
        var data = new byte[9902];
        // Write index 42 as little-endian int32
        data[0] = 42;
        data[1] = 0;
        data[2] = 0;
        data[3] = 0;

        var preset = _serializer.Deserialize(data);
        Assert.Equal(42, preset.Index);
    }

    [Fact]
    public void Deserialize_ExtractsPresetName()
    {
        var data = new byte[9902];
        // Name is at preset offset 4 (scene start) + 0x6A = 0x6E
        var name = Encoding.ASCII.GetBytes("My Test Preset");
        Array.Copy(name, 0, data, 4 + 0x6A, name.Length);

        var preset = _serializer.Deserialize(data);
        Assert.Equal("My Test Preset", preset.Name);
    }

    [Fact]
    public void Deserialize_ExtractsCabinetName()
    {
        var data = new byte[9902];
        var cabName = Encoding.ASCII.GetBytes("ML-MEGA-GREEN-MIX-CL");
        Array.Copy(cabName, 0, data, 0x1B2, cabName.Length);

        var preset = _serializer.Deserialize(data);
        Assert.Equal("ML-MEGA-GREEN-MIX-CL", preset.Cabinet.Name);
    }

    [Fact]
    public void Deserialize_ExtractsSignalChain()
    {
        var data = new byte[9902];
        // Signal chain at scene offset 0x5E (within scene A starting at byte 4)
        byte[] chain = { 1, 5, 2, 3, 9, 4, 10, 6, 7, 8, 11, 0 };
        Array.Copy(chain, 0, data, 4 + 0x5E, chain.Length);

        var preset = _serializer.Deserialize(data);
        Assert.Equal(new[] { 1, 5, 2, 3, 9, 4, 10, 6, 7, 8, 11, 0 }, preset.SceneA.SignalChain);
    }

    [Fact]
    public void Serialize_ProducesCorrectSize()
    {
        var preset = new Preset { Index = 0, Name = "Test" };
        var data = _serializer.Serialize(preset);
        Assert.Equal(9902, data.Length);
    }

    [Fact]
    public void Serialize_WritesPresetIndex()
    {
        var preset = new Preset { Index = 7, Name = "Test" };
        var data = _serializer.Serialize(preset);
        Assert.Equal(7, BitConverter.ToInt32(data, 0));
    }

    [Fact]
    public void Serialize_WritesPresetName()
    {
        var preset = new Preset { Index = 0, Name = "My Cool Tone" };
        var data = _serializer.Serialize(preset);

        // Name should be at offset 4 + 0x6A = 0x6E
        var nameBytes = new byte[16];
        Array.Copy(data, 4 + 0x6A, nameBytes, 0, 16);
        var name = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');
        Assert.Equal("My Cool Tone", name);
    }

    [Fact]
    public void Serialize_WritesCabinetName()
    {
        var preset = new Preset
        {
            Index = 0,
            Name = "Test",
            Cabinet = new CabinetIR { Name = "V412" }
        };
        var data = _serializer.Serialize(preset);

        var cabBytes = new byte[32];
        Array.Copy(data, 0x1B2, cabBytes, 0, 32);
        var cab = Encoding.ASCII.GetString(cabBytes).TrimEnd('\0');
        Assert.Equal("V412", cab);
    }

    [Fact]
    public void RoundTrip_PreservesNameAndIndex()
    {
        var original = new Preset
        {
            Index = 5,
            Name = "Round Trip Test"
        };

        var serialized = _serializer.Serialize(original);
        var deserialized = _serializer.Deserialize(serialized);

        Assert.Equal(original.Index, deserialized.Index);
        Assert.Equal(original.Name, deserialized.Name);
    }

    [Fact]
    public void RoundTrip_PreservesCabinetName()
    {
        var original = new Preset
        {
            Index = 0,
            Name = "Test",
            Cabinet = new CabinetIR { Name = "GB412" }
        };

        var serialized = _serializer.Serialize(original);
        var deserialized = _serializer.Deserialize(serialized);

        Assert.Equal("GB412", deserialized.Cabinet.Name);
    }

    [Fact]
    public void DeserializeAll_ReturnsCorrectCount()
    {
        var data = new byte[9902 * 3]; // 3 presets
        // Set indices
        data[0] = 0; // Preset 0
        data[9902] = 1; // Preset 1
        data[9902 * 2] = 2; // Preset 2

        var presets = _serializer.DeserializeAll(data);
        Assert.Equal(3, presets.Count);
        Assert.Equal(0, presets[0].Index);
        Assert.Equal(1, presets[1].Index);
        Assert.Equal(2, presets[2].Index);
    }

    [Fact]
    public void RoundTrip_WithRawData_PreservesSceneBytes()
    {
        // Create a preset with raw scene data
        var sceneData = new byte[140];
        // Write some recognizable data
        for (int i = 0; i < 140; i++)
            sceneData[i] = (byte)(i % 256);
        // Write a name at offset 0x6A
        var nameBytes = Encoding.ASCII.GetBytes("Raw Data Test");
        Array.Copy(nameBytes, 0, sceneData, 0x6A, nameBytes.Length);

        var original = new Preset
        {
            Index = 10,
            Name = "Raw Data Test",
            SceneA = new Scene { RawData = sceneData },
        };

        var serialized = _serializer.Serialize(original);
        var deserialized = _serializer.Deserialize(serialized);

        Assert.Equal("Raw Data Test", deserialized.Name);
        // Scene A raw data should be preserved
        Assert.Equal(140, deserialized.SceneA.RawData.Length);
        // Name area should match
        for (int i = 0x6A; i < 0x6A + 13; i++)
            Assert.Equal(sceneData[i], deserialized.SceneA.RawData[i]);
    }
}
