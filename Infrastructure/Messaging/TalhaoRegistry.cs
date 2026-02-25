using System.Collections.Concurrent;

namespace SensorService.Infrastructure.Messaging;

/// <summary>
/// Guarda os talhãoIds recebidos do PropertyService.
/// Como primeira versão (iniciante), isso fica em memória.
/// </summary>
public sealed class TalhaoRegistry
{
    private readonly ConcurrentDictionary<Guid, DateTime> _known = new();

    public void Upsert(Guid talhaoId, DateTime seenAtUtc)
        => _known[talhaoId] = seenAtUtc;

    public bool Contains(Guid talhaoId)
        => _known.ContainsKey(talhaoId);

    public int Count => _known.Count;
}
