namespace Shared.Events;

public sealed record TalhaoCreatedEvent(
    Guid TalhaoId,
    Guid PropriedadeId,
    Guid ProdutorId,
    string NomeTalhao,
    decimal AreaHa,
    string Cultura,
    DateTime TimestampUtc
);
