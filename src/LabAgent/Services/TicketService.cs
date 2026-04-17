using System.Collections.Concurrent;

namespace LabAgent.Services;

public record TicketResult(int TicketId, string Category, string Priority, string Status, string Summary);

public class TicketService
{
    private static int _nextId = 1041;
    private readonly ConcurrentBag<TicketResult> _tickets = new();

    public TicketResult Open(string requester, string category, string description, string priority)
    {
        var id = Interlocked.Increment(ref _nextId);
        var ticket = new TicketResult(
            TicketId: id,
            Category: string.IsNullOrWhiteSpace(category) ? "Geral" : category,
            Priority: string.IsNullOrWhiteSpace(priority) ? "Media" : priority,
            Status: "Aberto",
            Summary: $"Chamado aberto para '{requester}': {description}");

        _tickets.Add(ticket);
        return ticket;
    }

    public IReadOnlyCollection<TicketResult> GetAll() => _tickets.ToArray();
}
