using System.Text.Json;

namespace LabAgent.Services;

public class KnowledgeBaseService
{
    private readonly List<KnowledgeEntry> _entries;

    public KnowledgeBaseService(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "knowledge-base.json");
        using var stream = File.OpenRead(path);
        var doc = JsonSerializer.Deserialize<KnowledgeBaseFile>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Falha ao ler knowledge-base.json");
        _entries = doc.Entries;
    }

    public IReadOnlyList<KnowledgeEntry> All() => _entries;

    public KnowledgeEntry? FindBest(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return null;

        var terms = query.ToLowerInvariant()
            .Split(new[] { ' ', ',', '.', '?', '!', ';', ':' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length > 2)
            .ToArray();

        KnowledgeEntry? best = null;
        int bestScore = 0;

        foreach (var entry in _entries)
        {
            var haystack = (entry.Topic + " " + string.Join(" ", entry.Keywords) + " " + entry.Content)
                .ToLowerInvariant();

            int score = terms.Count(t => haystack.Contains(t));
            foreach (var kw in entry.Keywords)
            {
                if (terms.Contains(kw.ToLowerInvariant())) score += 2;
            }

            if (score > bestScore)
            {
                bestScore = score;
                best = entry;
            }
        }

        return bestScore > 0 ? best : null;
    }

    private class KnowledgeBaseFile
    {
        public List<KnowledgeEntry> Entries { get; set; } = new();
    }
}

public class KnowledgeEntry
{
    public string Id { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = new();
    public string Content { get; set; } = string.Empty;
}
