namespace NuxToneGenerator.Core.Models;

using System.Collections.Generic;

public record ToneRequest
{
    public string Prompt { get; init; } = string.Empty;
    public Preset? CurrentPreset { get; init; }
}

public record ToneResponse
{
    public Preset Preset { get; init; } = new();
    public string Explanation { get; init; } = string.Empty;
    public List<string> Warnings { get; init; } = new();
}
