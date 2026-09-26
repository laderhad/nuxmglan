namespace NuxToneGenerator.Infrastructure.Services;

using NuxToneGenerator.Core.Interfaces;
using NuxToneGenerator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class EquipmentCatalogService : IEquipmentCatalogService
{
    private readonly EquipmentCatalog _catalog;

    public EquipmentCatalogService()
    {
        var ampParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Gain", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Bass", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Middle", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Treble", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Volume", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Presence", MinValue = 0, MaxValue = 100, DefaultValue = 50 }
        };

        var driveParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Level", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Drive", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Tone", MinValue = 0, MaxValue = 100, DefaultValue = 50 }
        };

        var modParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Rate", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Depth", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Mix", MinValue = 0, MaxValue = 100, DefaultValue = 50 }
        };

        var delayParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Time", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Feedback", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Mix", MinValue = 0, MaxValue = 100, DefaultValue = 30 }
        };

        var reverbParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Decay", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "PreDelay", MinValue = 0, MaxValue = 100, DefaultValue = 20 },
            new ParameterDefinition { Name = "Mix", MinValue = 0, MaxValue = 100, DefaultValue = 30 }
        };

        var compParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Level", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Tone", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Attack", MinValue = 0, MaxValue = 100, DefaultValue = 50 }
        };

        var gateParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Threshold", MinValue = 0, MaxValue = 100, DefaultValue = 30 }
        };

        // Using exact model names from MG-30 PDF documentation
        _catalog = new EquipmentCatalog
        {
            Amplifiers = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "JazzClean", Name = "Jazz Clean", BasedOn = "Roland JC-120", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "DeluxeRvb", Name = "Deluxe Rvb", BasedOn = "Fender Deluxe Reverb", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "FPrinceton", Name = "F Princeton", BasedOn = "Fender Princeton Reverb", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "BassMate", Name = "Bass Mate", BasedOn = "Fender Bassman", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Tweedy", Name = "Tweedy", BasedOn = "Fender Tweed Deluxe", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "TwinRvb", Name = "Twin Rvb", BasedOn = "Fender Twin Reverb", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Hiwire", Name = "Hiwire", BasedOn = "Hiwatt DR103", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "CaliCrunch", Name = "Cali Crunch", BasedOn = "Mesa Boogie Mark I", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "ClassA15", Name = "Class A 15", BasedOn = "Vox AC15", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "ClassA30", Name = "Class A 30", BasedOn = "Vox AC30", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Plexi100W", Name = "Plexi 100W", BasedOn = "Marshall JTM45/100", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Plexi45", Name = "Plexi 45", BasedOn = "Marshall JTM45", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Brit800", Name = "Brit 800", BasedOn = "Marshall JCM800", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "1987X50W", Name = "1987 X 50W", BasedOn = "Marshall 1987x", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Slo100", Name = "Slo 100", BasedOn = "Soldano SLO100", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "FiremanHBE", Name = "Fireman HBE", BasedOn = "Friedman HBE-100", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "DualRect", Name = "Dual Rect", BasedOn = "Mesa Boogie Dual Rectifier", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "DieVH4", Name = "Die VH4", BasedOn = "Diezel VH4", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "VibroKing", Name = "Vibro King", BasedOn = "Fender Vibro King", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Budda", Name = "Budda", BasedOn = "Budda Superdrive", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "MrZ38", Name = "Mr Z 38", BasedOn = "Dr.Z Maz 38", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "SuperRvb", Name = "Super Rvb", BasedOn = "Fender Super-Sonic", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "BritBlues", Name = "Brit Blues", BasedOn = "Marshall Bluesbreaker", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Match", Name = "Match", BasedOn = "Matchless DC30", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Brit2000", Name = "Brit 2000", BasedOn = "Marshall JCM2000", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Uber", Name = "Uber", BasedOn = "Bogner Uberschall", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Vivo", Name = "Vivo", BasedOn = "Peavey 5150", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "OptimaAir", Name = "Optima Air", BasedOn = "NUX Optima Air", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Stageman", Name = "Stageman", BasedOn = "NUX Stageman AC-50", Category = "Amplifier", Parameters = ampParams },
            },
            Cabinets = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "JZ120", Name = "JZ120", BasedOn = "2x12 Roland JC-120", Category = "Cabinet" },
                new EquipmentItem { Id = "DR112", Name = "DR112", BasedOn = "1x12 Fender Deluxe Reverb", Category = "Cabinet" },
                new EquipmentItem { Id = "BS410", Name = "BS410", BasedOn = "4x10 Fender Bassman", Category = "Cabinet" },
                new EquipmentItem { Id = "A212", Name = "A212", BasedOn = "2x12 Vox AC30", Category = "Cabinet" },
                new EquipmentItem { Id = "TR212", Name = "TR212", BasedOn = "2x12 Fender Twin Reverb", Category = "Cabinet" },
                new EquipmentItem { Id = "1960", Name = "1960", BasedOn = "4x12 Marshall 1960", Category = "Cabinet" },
                new EquipmentItem { Id = "GB412", Name = "GB412", BasedOn = "4x12 Marshall Greenback", Category = "Cabinet" },
                new EquipmentItem { Id = "V412", Name = "V412", BasedOn = "4x12 Marshall V30", Category = "Cabinet" },
            },
            Overdrives = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "DistortionPlus", Name = "Distortion +", BasedOn = "MXR Distortion +", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "RCBoost", Name = "RC Boost", BasedOn = "Xotic RC Booster", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "ACBoost", Name = "AC Boost", BasedOn = "Xotic AC Booster", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "DistOne", Name = "Dist One", BasedOn = "Boss DS-1", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "TScream", Name = "T Scream", BasedOn = "Ibanez TS808 Tube Screamer", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "BluesDrv", Name = "Blues Drv", BasedOn = "Boss BD-2", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "MorningDrv", Name = "Morning Drv", BasedOn = "JHS Morning Glory", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "EatDist", Name = "Eat Dist", BasedOn = "Pro Co RAT", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "RedDirt", Name = "Red Dirt", BasedOn = "Keeley Red Dirt", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "Crunch", Name = "Crunch", BasedOn = "JHS Angry Charlie", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "MuffFuzz", Name = "Muff Fuzz", BasedOn = "Electro-Harmonix Big Muff", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "Katana", Name = "Katana", BasedOn = "Keeley Katana Clean Boost", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "STSinger", Name = "ST Singer", BasedOn = "NUX Steel Singer Drive", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "RedFuzz", Name = "Red Fuzz", BasedOn = "Arbiter Fuzz Face", Category = "Overdrive", Parameters = driveParams },
                new EquipmentItem { Id = "TouchWah", Name = "Touch Wah", BasedOn = "Musitronics Mu-Tron III", Category = "Overdrive", Parameters = driveParams },
            },
            Modulations = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "CE1", Name = "CE-1", BasedOn = "Boss CE-1", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "CE2", Name = "CE-2", BasedOn = "Boss CE-2", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "StChorus", Name = "St. Chorus", BasedOn = "MXR M134 Stereo Chorus", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "SCH1", Name = "SCH-1", BasedOn = "Arion SCH-1 Stereo Chorus", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Vibrator", Name = "Vibrator", BasedOn = "Boss VB-2", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Detune", Name = "Detune", BasedOn = "NUX Original", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Flanger", Name = "Flanger", BasedOn = "Boss BF-3", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Phase90", Name = "Phase 90", BasedOn = "MXR Phase 90", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Phase100", Name = "Phase 100", BasedOn = "MXR Phase 100", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "SCF", Name = "SCF", BasedOn = "TC Electronic SCF", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "UVibe", Name = "U-Vibe", BasedOn = "Dunlop UV-1 Uni-Vibe", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Tremolo", Name = "Tremolo", BasedOn = "Boss TR-2", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Rotary", Name = "Rotary", BasedOn = "Strymon Lex", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Harmonist", Name = "Harmonist", BasedOn = "Digitech Harmony Man", Category = "Modulation", Parameters = modParams },
            },
            Delays = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "AnalogDelay", Name = "Analog Delay", BasedOn = "Boss DM-2", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "DigiDelay", Name = "Digi Delay", BasedOn = "Boss DD-7", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "ModDelay", Name = "Modulation", BasedOn = "Ibanez DML 10", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "TapeEcho", Name = "Tape Echo", BasedOn = "Roland RE-101", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "Reverse", Name = "Reverse", BasedOn = "TC Electronic Alter Ego", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "PanDelay", Name = "Pan Delay", BasedOn = "Ibanez DPL-10", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "Duotime", Name = "Duotime", BasedOn = "NUX DuoTime", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "PhiDelay", Name = "Phi Delay", BasedOn = "NUX Original", Category = "Delay", Parameters = delayParams },
            },
            Reverbs = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "Room", Name = "Room", BasedOn = "MXR M300 Reverb", Category = "Reverb", Parameters = reverbParams },
                new EquipmentItem { Id = "Hall", Name = "Hall", BasedOn = "Lexicon 480L", Category = "Reverb", Parameters = reverbParams },
                new EquipmentItem { Id = "Plate", Name = "Plate", BasedOn = "EMT 140", Category = "Reverb", Parameters = reverbParams },
                new EquipmentItem { Id = "Spring", Name = "Spring", BasedOn = "Boss FRV-1", Category = "Reverb", Parameters = reverbParams },
                new EquipmentItem { Id = "Shimmer", Name = "Shimmer", BasedOn = "Neunaber Seraphim", Category = "Reverb", Parameters = reverbParams },
                new EquipmentItem { Id = "Damp", Name = "Damp", BasedOn = "Neunaber Wet", Category = "Reverb", Parameters = reverbParams },
            },
            Compressions = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "RoseComp", Name = "Rose Comp", BasedOn = "Ross Compressor", Category = "Compression", Parameters = compParams },
                new EquipmentItem { Id = "KComp", Name = "K Comp", BasedOn = "Keeley C4 4-Knob", Category = "Compression", Parameters = compParams },
                new EquipmentItem { Id = "StudioComp", Name = "Studio Comp", BasedOn = "Studio Compressor", Category = "Compression", Parameters = compParams },
            },
            EQs = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "6BandEq", Name = "6-Band Eq", BasedOn = "Boss GE-6", Category = "EQ", Parameters = new List<ParameterDefinition>
                {
                    new ParameterDefinition { Name = "100Hz", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
                    new ParameterDefinition { Name = "250Hz", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
                    new ParameterDefinition { Name = "630Hz", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
                    new ParameterDefinition { Name = "1.6kHz", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
                    new ParameterDefinition { Name = "4kHz", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
                    new ParameterDefinition { Name = "10kHz", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
                }},
                new EquipmentItem { Id = "AlignEq", Name = "Align Eq", BasedOn = "LR Baggs Align EQ", Category = "EQ", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "10BandEq", Name = "10-Band Eq", BasedOn = "MXR 10 Band Graphic EQ", Category = "EQ", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "ParaEq", Name = "Para Eq", BasedOn = "3 Band Parametric EQ", Category = "EQ", Parameters = new List<ParameterDefinition>() },
            },
            NoiseGates = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "NoiseGate", Name = "Noise Gate", BasedOn = "Boss NF-1", Category = "NoiseGate", Parameters = gateParams },
            },
            WahVolumes = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "Clyde", Name = "Clyde", BasedOn = "Fulltone Clyde Wah", Category = "WahVolume", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "CryBB", Name = "Cry BB", BasedOn = "Dunlop Cry Baby", Category = "WahVolume", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "V847", Name = "V847", BasedOn = "Vox V847", Category = "WahVolume", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "HorseWah", Name = "Horse Wah", BasedOn = "Morley Bad Horsie", Category = "WahVolume", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "OctaveShift", Name = "Octave Shift", BasedOn = "Digitech Whammy", Category = "WahVolume", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "Volume", Name = "Volume", BasedOn = "Volume Pedal", Category = "WahVolume", Parameters = new List<ParameterDefinition>() },
            },
            EFXs = new List<EquipmentItem>()
        };
    }

    public EquipmentCatalog GetCatalog() => _catalog;

    public EquipmentItem? FindAmplifier(string nameOrId)
    {
        return _catalog.Amplifiers.FirstOrDefault(a =>
            a.Id.Equals(nameOrId, StringComparison.OrdinalIgnoreCase) ||
            a.Name.Equals(nameOrId, StringComparison.OrdinalIgnoreCase));
    }

    public EquipmentItem? FindEffect(string category, string nameOrId)
    {
        var effects = GetEffectsByCategory(category);
        return effects.FirstOrDefault(e =>
            e.Id.Equals(nameOrId, StringComparison.OrdinalIgnoreCase) ||
            e.Name.Equals(nameOrId, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<EquipmentItem> GetEffectsByCategory(string category)
    {
        return category.ToLowerInvariant() switch
        {
            "amplifier" => _catalog.Amplifiers,
            "cabinet" => _catalog.Cabinets,
            "overdrive" => _catalog.Overdrives,
            "modulation" => _catalog.Modulations,
            "delay" => _catalog.Delays,
            "reverb" => _catalog.Reverbs,
            "compression" => _catalog.Compressions,
            "eq" => _catalog.EQs,
            "noisegate" => _catalog.NoiseGates,
            "wahvolume" => _catalog.WahVolumes,
            "efx" => _catalog.EFXs,
            _ => Array.Empty<EquipmentItem>()
        };
    }
}
