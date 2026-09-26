namespace NuxToneGenerator.Core.Interfaces;

using NuxToneGenerator.Core.Models;
using System.Collections.Generic;

public interface IEquipmentCatalogService
{
    EquipmentCatalog GetCatalog();
    EquipmentItem? FindAmplifier(string nameOrId);
    EquipmentItem? FindEffect(string category, string nameOrId);
    IReadOnlyList<EquipmentItem> GetEffectsByCategory(string category);
}
