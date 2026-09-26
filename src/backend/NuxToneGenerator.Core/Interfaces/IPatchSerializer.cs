namespace NuxToneGenerator.Core.Interfaces;

using NuxToneGenerator.Core.Models;
using System.Collections.Generic;

public interface IPatchSerializer
{
    byte[] Serialize(Preset preset);
    Preset Deserialize(byte[] data, int presetIndex = 0);
    IReadOnlyList<Preset> DeserializeAll(byte[] data);
    bool ValidatePatch(byte[] data);
}
