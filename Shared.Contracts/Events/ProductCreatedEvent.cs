namespace Shared.Contracts.Events; // 👈 Novo namespace corporativo padronizado

public record ProductCreatedEvent(
    string Id,
    string Name,
    string Description
);
