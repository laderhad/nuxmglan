namespace NuxToneGenerator.Tests;

using NuxToneGenerator.Infrastructure.Services;
using NuxToneGenerator.Core.Models;

public class EquipmentCatalogServiceTests
{
    private readonly EquipmentCatalogService _service = new();

    [Fact]
    public void GetCatalog_ReturnsNonEmptyCatalog()
    {
        var catalog = _service.GetCatalog();
        Assert.NotNull(catalog);
        Assert.NotEmpty(catalog.Amplifiers);
        Assert.NotEmpty(catalog.Cabinets);
        Assert.NotEmpty(catalog.Overdrives);
        Assert.NotEmpty(catalog.Modulations);
        Assert.NotEmpty(catalog.Delays);
        Assert.NotEmpty(catalog.Reverbs);
        Assert.NotEmpty(catalog.Compressions);
    }

    [Fact]
    public void GetCatalog_AmplifiersHaveCorrectParameters()
    {
        var catalog = _service.GetCatalog();
        var amp = catalog.Amplifiers[0];

        Assert.Equal(6, amp.Parameters.Count);
        Assert.Contains(amp.Parameters, p => p.Name == "Gain");
        Assert.Contains(amp.Parameters, p => p.Name == "Bass");
        Assert.Contains(amp.Parameters, p => p.Name == "Middle");
        Assert.Contains(amp.Parameters, p => p.Name == "Treble");
        Assert.Contains(amp.Parameters, p => p.Name == "Volume");
        Assert.Contains(amp.Parameters, p => p.Name == "Presence");
    }

    [Theory]
    [InlineData("Jazz Clean")]
    [InlineData("Brit 800")]
    [InlineData("Dual Rect")]
    [InlineData("Slo 100")]
    [InlineData("Class A 30")]
    public void FindAmplifier_FindsByName(string name)
    {
        var amp = _service.FindAmplifier(name);
        Assert.NotNull(amp);
        Assert.Equal(name, amp.Name);
    }

    [Theory]
    [InlineData("JazzClean")]
    [InlineData("Brit800")]
    [InlineData("DualRect")]
    public void FindAmplifier_FindsById(string id)
    {
        var amp = _service.FindAmplifier(id);
        Assert.NotNull(amp);
        Assert.Equal(id, amp.Id);
    }

    [Fact]
    public void FindAmplifier_ReturnsNullForUnknown()
    {
        var amp = _service.FindAmplifier("Nonexistent Amp");
        Assert.Null(amp);
    }

    [Theory]
    [InlineData("Overdrive", "T Scream")]
    [InlineData("Modulation", "CE-1")]
    [InlineData("Delay", "Tape Echo")]
    [InlineData("Reverb", "Hall")]
    [InlineData("Compression", "Rose Comp")]
    public void FindEffect_FindsByName(string category, string name)
    {
        var effect = _service.FindEffect(category, name);
        Assert.NotNull(effect);
        Assert.Equal(name, effect.Name);
    }

    [Theory]
    [InlineData("Amplifier")]
    [InlineData("Cabinet")]
    [InlineData("Overdrive")]
    [InlineData("Modulation")]
    [InlineData("Delay")]
    [InlineData("Reverb")]
    [InlineData("Compression")]
    public void GetEffectsByCategory_ReturnsNonEmpty(string category)
    {
        var effects = _service.GetEffectsByCategory(category);
        Assert.NotEmpty(effects);
    }

    [Fact]
    public void GetEffectsByCategory_UnknownReturnsEmpty()
    {
        var effects = _service.GetEffectsByCategory("unknown");
        Assert.Empty(effects);
    }

    [Fact]
    public void AllAmplifiers_HaveBasedOnProperty()
    {
        var catalog = _service.GetCatalog();
        foreach (var amp in catalog.Amplifiers)
        {
            Assert.False(string.IsNullOrEmpty(amp.BasedOn),
                $"Amplifier '{amp.Name}' is missing BasedOn");
        }
    }

    [Fact]
    public void AllOverdrives_HaveParameters()
    {
        var catalog = _service.GetCatalog();
        foreach (var od in catalog.Overdrives)
        {
            Assert.NotEmpty(od.Parameters);
            Assert.Contains(od.Parameters, p => p.Name == "Level");
            Assert.Contains(od.Parameters, p => p.Name == "Drive");
            Assert.Contains(od.Parameters, p => p.Name == "Tone");
        }
    }

    [Fact]
    public void AllDelays_HaveParameters()
    {
        var catalog = _service.GetCatalog();
        foreach (var delay in catalog.Delays)
        {
            Assert.NotEmpty(delay.Parameters);
            Assert.Contains(delay.Parameters, p => p.Name == "Time");
            Assert.Contains(delay.Parameters, p => p.Name == "Feedback");
            Assert.Contains(delay.Parameters, p => p.Name == "Mix");
        }
    }

    [Fact]
    public void AllParameters_HaveValidRanges()
    {
        var catalog = _service.GetCatalog();
        var allItems = catalog.Amplifiers
            .Concat(catalog.Overdrives)
            .Concat(catalog.Modulations)
            .Concat(catalog.Delays)
            .Concat(catalog.Reverbs)
            .Concat(catalog.Compressions);

        foreach (var item in allItems)
        {
            foreach (var param in item.Parameters)
            {
                Assert.True(param.MinValue >= 0, $"{item.Name}.{param.Name} min < 0");
                Assert.True(param.MaxValue <= 100, $"{item.Name}.{param.Name} max > 100");
                Assert.True(param.MinValue <= param.MaxValue, $"{item.Name}.{param.Name} min > max");
                Assert.True(param.DefaultValue >= param.MinValue && param.DefaultValue <= param.MaxValue,
                    $"{item.Name}.{param.Name} default out of range");
            }
        }
    }
}
