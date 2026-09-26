namespace NuxToneGenerator.Core.Interfaces;

using NuxToneGenerator.Core.Models;
using System.Threading;
using System.Threading.Tasks;

public interface IToneGenerationService
{
    Task<ToneResponse> GenerateToneAsync(ToneRequest request, CancellationToken ct = default);
}
