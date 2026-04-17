using System.ClientModel;
using System.ComponentModel;
using Azure.AI.OpenAI;
using LabAgent.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ChatResponse = LabAgent.Models.ChatResponse;

namespace LabAgent.Services;

public class AgentService
{
    private const string AgentInstructions = """
        Voce e o assistente virtual de triagem e atendimento interno da UniAnchieta.
        Seu objetivo e ajudar alunos, professores e funcionarios com duvidas e solicitacoes comuns,
        de forma clara, objetiva e cordial.

        Regras de comportamento:
        - Sempre que a pergunta for sobre um procedimento ou informacao institucional,
          use a ferramenta `search_knowledge_base` para buscar a resposta antes de responder.
        - Se a base de conhecimento retornar um resultado, use-o como fonte principal da resposta.
          Nao invente detalhes, URLs, prazos ou politicas que nao estejam no resultado.
        - Se o usuario descrever um problema que persiste mesmo apos tentar a solucao,
          ou que exige acao humana (acesso bloqueado, sistema fora do ar, erro recorrente),
          use a ferramenta `open_support_ticket` para abrir um chamado.
        - Quando a pergunta estiver ambigua ou faltar contexto (ex.: "nao esta funcionando"),
          pergunte educadamente o que nao esta funcionando antes de agir.
        - Se a solicitacao estiver claramente fora do escopo (ex.: imposto de renda, marketing),
          explique gentilmente que o assunto nao e atendido por este canal.
        - Responda sempre em portugues do Brasil, em no maximo 4 paragrafos curtos.
        """;

    private readonly AIAgent _agent;
    private readonly KnowledgeBaseService _kb;
    private readonly TicketService _tickets;
    private readonly ILogger<AgentService> _logger;
    private readonly List<string> _lastToolsInvoked = new();
    private readonly Lock _toolsLock = new();

    public AgentService(
        IConfiguration configuration,
        KnowledgeBaseService kb,
        TicketService tickets,
        ILogger<AgentService> logger)
    {
        _kb = kb;
        _tickets = tickets;
        _logger = logger;

        var endpoint = configuration["AzureOpenAI:Endpoint"]
            ?? throw new InvalidOperationException("AzureOpenAI:Endpoint nao configurado.");
        var apiKey = configuration["AzureOpenAI:ApiKey"]
            ?? throw new InvalidOperationException("AzureOpenAI:ApiKey nao configurado.");
        var deployment = configuration["AzureOpenAI:DeploymentName"]
            ?? throw new InvalidOperationException("AzureOpenAI:DeploymentName nao configurado.");

        var azureClient = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));

        _agent = azureClient
            .GetChatClient(deployment)
            .AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build()
            .AsAIAgent(
                instructions: AgentInstructions,
                name: "UniAnchietaTriageAgent",
                tools:
                [
                    AIFunctionFactory.Create(SearchKnowledgeBase, name: "search_knowledge_base",
                        description: "Busca na base de conhecimento interna da UniAnchieta a resposta para uma duvida do usuario."),
                    AIFunctionFactory.Create(OpenSupportTicket, name: "open_support_ticket",
                        description: "Abre um chamado de suporte interno quando o caso exige acao humana ou operacional."),
                ]);
    }

    public async Task<ChatResponse> SendAsync(string userMessage, CancellationToken ct = default)
    {
        lock (_toolsLock) _lastToolsInvoked.Clear();

        try
        {
            var result = await _agent.RunAsync(userMessage, cancellationToken: ct);

            string invoked;
            lock (_toolsLock) invoked = string.Join(",", _lastToolsInvoked);
            _logger.LogInformation("Agent concluiu. Tools: {Tools}", invoked);

            return new ChatResponse
            {
                Reply = result.Text ?? string.Empty,
                ToolsInvoked = _lastToolsInvoked.ToList(),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao executar agente.");
            return new ChatResponse { HasError = true, Error = ex.Message };
        }
    }

    [Description("Busca na base de conhecimento interna da UniAnchieta informacoes sobre procedimentos e duvidas comuns.")]
    private string SearchKnowledgeBase(
        [Description("Pergunta ou tema a ser pesquisado, em linguagem natural (ex.: redefinir senha do portal).")]
        string query)
    {
        lock (_toolsLock) _lastToolsInvoked.Add("search_knowledge_base");

        var entry = _kb.FindBest(query);
        if (entry is null)
            return "NADA_ENCONTRADO: nao ha entrada sobre esse assunto na base de conhecimento.";

        return $"Topico: {entry.Topic}\nConteudo: {entry.Content}";
    }

    [Description("Abre um chamado de suporte interno na UniAnchieta quando o caso exige intervencao humana ou operacional.")]
    private string OpenSupportTicket(
        [Description("Nome do solicitante. Se desconhecido, use 'Usuario Anonimo'.")]
        string requester,
        [Description("Categoria do chamado. Valores possiveis: Acesso, Academico, Financeiro, AVA, Email, Outros.")]
        string category,
        [Description("Descricao clara do problema, baseada no relato do usuario.")]
        string description,
        [Description("Prioridade sugerida: Baixa, Media, Alta.")]
        string priority)
    {
        lock (_toolsLock) _lastToolsInvoked.Add("open_support_ticket");

        var t = _tickets.Open(requester, category, description, priority);
        return
            $"Ticket {t.TicketId} criado com sucesso.\n" +
            $"Categoria: {t.Category}\n" +
            $"Prioridade: {t.Priority}\n" +
            $"Status: {t.Status}";
    }
}
