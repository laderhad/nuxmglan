namespace NuxToneGenerator.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using NuxToneGenerator.Core.Interfaces;
using NuxToneGenerator.Core.Models;
using OpenAI.Chat;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class ToneGenerationService : IToneGenerationService
{
    private readonly IEquipmentCatalogService _catalogService;
    private readonly string _apiKey;

    public ToneGenerationService(IEquipmentCatalogService catalogService, IConfiguration configuration)
    {
        _catalogService = catalogService;
        _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
    }

    public async Task<ToneResponse> GenerateToneAsync(ToneRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            return new ToneResponse { Warnings = new List<string> { "OpenAI API Key is not configured. Set it with 'dotnet user-secrets set OpenAI:ApiKey <your-key>' or the OpenAI__ApiKey environment variable." } };
        }

        var catalog = _catalogService.GetCatalog();
        var ampNames = catalog.Amplifiers.Select(a => $"{a.Name} (based on {a.BasedOn})");
        var cabNames = catalog.Cabinets.Select(c => $"{c.Name} ({c.BasedOn})");
        var odNames = catalog.Overdrives.Select(e => $"{e.Name} (based on {e.BasedOn})");
        var modNames = catalog.Modulations.Select(e => $"{e.Name} (based on {e.BasedOn})");
        var dlyNames = catalog.Delays.Select(e => $"{e.Name} (based on {e.BasedOn})");
        var rvbNames = catalog.Reverbs.Select(e => $"{e.Name} (based on {e.BasedOn})");
        var cmpNames = catalog.Compressions.Select(e => $"{e.Name} (based on {e.BasedOn})");

        var systemPrompt = $@"You are an expert AI guitar tone designer for the NUX MG-30 multi-effects processor.
Given a user's description of a desired guitar tone, you must select appropriate equipment and configure parameters.

IMPORTANT RULES:
- Use ONLY the exact model names listed below. Do not invent or modify names.
- All parameter values must be integers from 0 to 100.
- Think about the real-world equipment being emulated and set parameters accordingly.
- Consider the interaction between amp gain and overdrive pedals.
- Match the described tone character (warm, bright, heavy, clean, etc.) to appropriate settings.

AVAILABLE AMPLIFIERS (use exact Name):
{string.Join("\n", ampNames)}

AVAILABLE CABINETS (use exact Name):
{string.Join("\n", cabNames)}

AVAILABLE OVERDRIVES/DISTORTIONS (use exact Name):
{string.Join("\n", odNames)}

AVAILABLE COMPRESSIONS (use exact Name):
{string.Join("\n", cmpNames)}

AVAILABLE MODULATIONS (use exact Name):
{string.Join("\n", modNames)}

AVAILABLE DELAYS (use exact Name):
{string.Join("\n", dlyNames)}

AVAILABLE REVERBS (use exact Name):
{string.Join("\n", rvbNames)}

For each effect block, set parameters:
- Overdrive: Level (output), Drive (gain amount), Tone (brightness)
- Compression: Level (output), Tone (brightness), Attack (response speed)
- Modulation: Rate (speed), Depth (intensity), Mix (wet/dry)
- Delay: Time (delay time), Feedback (repeats), Mix (wet/dry)
- Reverb: Decay (tail length), PreDelay (gap before reverb), Mix (wet/dry)
- Noise Gate: Threshold (sensitivity, higher = more gating)
";

        var schemaJson = @"{
          ""type"": ""object"",
          ""properties"": {
            ""amplifier"": { ""type"": ""object"", ""properties"": { ""model"": { ""type"": ""string"" }, ""gain"": { ""type"": ""integer"" }, ""bass"": { ""type"": ""integer"" }, ""middle"": { ""type"": ""integer"" }, ""treble"": { ""type"": ""integer"" }, ""volume"": { ""type"": ""integer"" }, ""presence"": { ""type"": ""integer"" } }, ""required"": [""model"", ""gain"", ""bass"", ""middle"", ""treble"", ""volume"", ""presence""], ""additionalProperties"": false },
            ""cabinet"": { ""type"": ""object"", ""properties"": { ""name"": { ""type"": ""string"" } }, ""required"": [""name""], ""additionalProperties"": false },
            ""overdrive"": { ""type"": ""object"", ""properties"": { ""effect"": { ""type"": ""string"" }, ""enabled"": { ""type"": ""boolean"" }, ""level"": { ""type"": ""integer"" }, ""drive"": { ""type"": ""integer"" }, ""tone"": { ""type"": ""integer"" } }, ""required"": [""effect"", ""enabled"", ""level"", ""drive"", ""tone""], ""additionalProperties"": false },
            ""compression"": { ""type"": ""object"", ""properties"": { ""effect"": { ""type"": ""string"" }, ""enabled"": { ""type"": ""boolean"" }, ""level"": { ""type"": ""integer"" }, ""tone"": { ""type"": ""integer"" }, ""attack"": { ""type"": ""integer"" } }, ""required"": [""effect"", ""enabled"", ""level"", ""tone"", ""attack""], ""additionalProperties"": false },
            ""modulation"": { ""type"": ""object"", ""properties"": { ""effect"": { ""type"": ""string"" }, ""enabled"": { ""type"": ""boolean"" }, ""rate"": { ""type"": ""integer"" }, ""depth"": { ""type"": ""integer"" }, ""mix"": { ""type"": ""integer"" } }, ""required"": [""effect"", ""enabled"", ""rate"", ""depth"", ""mix""], ""additionalProperties"": false },
            ""delay"": { ""type"": ""object"", ""properties"": { ""effect"": { ""type"": ""string"" }, ""enabled"": { ""type"": ""boolean"" }, ""time"": { ""type"": ""integer"" }, ""feedback"": { ""type"": ""integer"" }, ""mix"": { ""type"": ""integer"" } }, ""required"": [""effect"", ""enabled"", ""time"", ""feedback"", ""mix""], ""additionalProperties"": false },
            ""reverb"": { ""type"": ""object"", ""properties"": { ""effect"": { ""type"": ""string"" }, ""enabled"": { ""type"": ""boolean"" }, ""decay"": { ""type"": ""integer"" }, ""predelay"": { ""type"": ""integer"" }, ""mix"": { ""type"": ""integer"" } }, ""required"": [""effect"", ""enabled"", ""decay"", ""predelay"", ""mix""], ""additionalProperties"": false },
            ""noiseGate"": { ""type"": ""object"", ""properties"": { ""enabled"": { ""type"": ""boolean"" }, ""threshold"": { ""type"": ""integer"" } }, ""required"": [""enabled"", ""threshold""], ""additionalProperties"": false },
            ""explanation"": { ""type"": ""string"" }
          },
          ""required"": [""amplifier"", ""cabinet"", ""overdrive"", ""compression"", ""modulation"", ""delay"", ""reverb"", ""noiseGate"", ""explanation""],
          ""additionalProperties"": false
        }";

        var client = new ChatClient(model: "gpt-4o", _apiKey);
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(request.Prompt)
        };
        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "preset_response",
                BinaryData.FromString(schemaJson))
        };

        var completion = await client.CompleteChatAsync(messages, options, ct);
        var responseText = completion.Value.Content[0].Text;

        using var doc = JsonDocument.Parse(responseText);
        var root = doc.RootElement;

        var warnings = new List<string>();
        var scene = new Scene();

        // Parse amplifier
        if (root.TryGetProperty("amplifier", out var ampProp))
        {
            var modelName = ampProp.GetProperty("model").GetString() ?? "";
            var found = _catalogService.FindAmplifier(modelName);
            if (found == null)
                warnings.Add($"Amplifier '{modelName}' not found in catalog. Using as-is.");

            scene = scene with
            {
                Amplifier = new AmplifierSettings
                {
                    ModelName = modelName,
                    ModelId = found?.Id ?? modelName,
                    Gain = Clamp(ampProp.GetProperty("gain").GetInt32()),
                    Bass = Clamp(ampProp.GetProperty("bass").GetInt32()),
                    Middle = Clamp(ampProp.GetProperty("middle").GetInt32()),
                    Treble = Clamp(ampProp.GetProperty("treble").GetInt32()),
                    Volume = Clamp(ampProp.GetProperty("volume").GetInt32()),
                    Presence = Clamp(ampProp.GetProperty("presence").GetInt32()),
                    Enabled = true
                }
            };
        }

        // Parse overdrive
        scene = scene with { Overdrive = ParseEffectBlock(root, "overdrive", "Overdrive", warnings) };

        // Parse compression
        scene = scene with { Compression = ParseEffectBlock(root, "compression", "Compression", warnings) };

        // Parse modulation
        scene = scene with { Modulation = ParseEffectBlock(root, "modulation", "Modulation", warnings) };

        // Parse delay
        scene = scene with { Delay = ParseEffectBlock(root, "delay", "Delay", warnings) };

        // Parse reverb
        scene = scene with { Reverb = ParseEffectBlock(root, "reverb", "Reverb", warnings) };

        // Parse noise gate
        if (root.TryGetProperty("noiseGate", out var gateProp))
        {
            scene = scene with
            {
                NoiseGate = new EffectSettings
                {
                    EffectName = "Noise Gate",
                    EffectId = "NoiseGate",
                    Enabled = gateProp.GetProperty("enabled").GetBoolean(),
                    Parameters = new Dictionary<string, int>
                    {
                        ["Threshold"] = Clamp(gateProp.GetProperty("threshold").GetInt32())
                    }
                }
            };
        }

        // Parse cabinet
        var cabinet = new CabinetIR();
        if (root.TryGetProperty("cabinet", out var cabProp))
        {
            cabinet = new CabinetIR { Name = cabProp.GetProperty("name").GetString() ?? "" };
        }

        var explanation = root.TryGetProperty("explanation", out var expProp)
            ? expProp.GetString() ?? ""
            : "";

        var preset = new Preset
        {
            Name = GeneratePresetName(request.Prompt),
            SceneA = scene,
            SceneB = scene,
            SceneC = scene,
            Cabinet = cabinet
        };

        return new ToneResponse
        {
            Preset = preset,
            Explanation = explanation,
            Warnings = warnings
        };
    }

    private EffectSettings ParseEffectBlock(JsonElement root, string propertyName, string category, List<string> warnings)
    {
        if (!root.TryGetProperty(propertyName, out var prop))
            return new EffectSettings();

        var effectName = prop.GetProperty("effect").GetString() ?? "";
        var enabled = prop.GetProperty("enabled").GetBoolean();
        var found = _catalogService.FindEffect(category, effectName);
        if (found == null && !string.IsNullOrEmpty(effectName))
            warnings.Add($"{category} effect '{effectName}' not found in catalog. Using as-is.");

        var parameters = new Dictionary<string, int>();
        foreach (var paramProp in prop.EnumerateObject())
        {
            if (paramProp.Name == "effect" || paramProp.Name == "enabled")
                continue;
            if (paramProp.Value.ValueKind == JsonValueKind.Number)
                parameters[char.ToUpper(paramProp.Name[0]) + paramProp.Name[1..]] = Clamp(paramProp.Value.GetInt32());
        }

        return new EffectSettings
        {
            EffectName = effectName,
            EffectId = found?.Id ?? effectName,
            Enabled = enabled,
            Parameters = parameters
        };
    }

    private static int Clamp(int value) => Math.Clamp(value, 0, 100);

    private static string GeneratePresetName(string prompt)
    {
        var words = prompt.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var name = string.Join(" ", words.Take(3));
        return name.Length > 16 ? name[..16] : name;
    }
}
