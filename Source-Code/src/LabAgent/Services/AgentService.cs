using System.ClientModel;
using System.ComponentModel;
using Azure.AI.OpenAI;
using LabAgent.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ChatResponse = LabAgent.Models.ChatResponse;

namespace LabAgent.Services;

/// <summary>
/// AgentService — construido ao vivo durante o laboratorio.
/// As responsabilidades sao:
///   1. Configurar o cliente do Azure OpenAI (Endpoint, ApiKey, DeploymentName).
///   2. Criar um AIAgent com instrucoes de comportamento e tools.
///   3. Expor SendAsync para que a pagina web envie mensagens do usuario.
///
/// Siga os TODOs abaixo. O projeto LabAgent.Final contem a implementacao completa de referencia.
/// </summary>
public class AgentService
{
    // TODO (Etapa 5.3): definir as instrucoes do agente.
    //   - O agente e o assistente de triagem da UniAnchieta.
    //   - Quando houver resposta na base, usar a tool `search_knowledge_base` antes de responder.
    //   - Quando o caso exigir suporte humano, usar `open_support_ticket`.
    //   - Se a pergunta estiver ambigua, pedir contexto.
    //   - Nao inventar politicas, URLs ou prazos.
    private const string AgentInstructions = """
        TODO: escreva aqui as instrucoes de comportamento do agente (papel, limites, tom, uso das tools).
        """;

    private readonly KnowledgeBaseService _kb;
    private readonly TicketService _tickets;
    private readonly ILogger<AgentService> _logger;
    private readonly List<string> _lastToolsInvoked = new();
    private readonly Lock _toolsLock = new();

    // TODO (Etapa 5.4c): guardar aqui o AIAgent depois de cria-lo.
    // private readonly AIAgent _agent;

    public AgentService(
        IConfiguration configuration,
        KnowledgeBaseService kb,
        TicketService tickets,
        ILogger<AgentService> logger)
    {
        _kb = kb;
        _tickets = tickets;
        _logger = logger;

        // TODO (Etapa 5.2): ler Endpoint, ApiKey e DeploymentName de appsettings.
        // var endpoint = configuration["AzureOpenAI:Endpoint"] ?? throw ...
        // var apiKey = configuration["AzureOpenAI:ApiKey"] ?? throw ...
        // var deployment = configuration["AzureOpenAI:DeploymentName"] ?? throw ...

        // TODO (Etapa 5.4): instanciar o AzureOpenAIClient com ApiKeyCredential.
        // var azureClient = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));

        // TODO (Etapa 5.4): criar o AIAgent via .GetChatClient(deployment).AsIChatClient()
        //                    .AsBuilder().UseFunctionInvocation().Build().AsAIAgent(...)
        // TODO (Etapa 5.5 e 5.6): registrar as tools `search_knowledge_base` e `open_support_ticket`.
        // _agent = azureClient.GetChatClient(deployment)
        //     .AsIChatClient()
        //     .AsBuilder()
        //     .UseFunctionInvocation()      // habilita chamada automatica de funcoes C#
        //     .Build()
        //     .AsAIAgent(
        //         instructions: AgentInstructions,
        //         name: "UniAnchietaTriageAgent",
        //         tools: [
        //             AIFunctionFactory.Create(SearchKnowledgeBase, name: "search_knowledge_base", ...),
        //             AIFunctionFactory.Create(OpenSupportTicket,   name: "open_support_ticket",   ...),
        //         ]);
    }

    public async Task<ChatResponse> SendAsync(string userMessage, CancellationToken ct = default)
    {
        lock (_toolsLock) _lastToolsInvoked.Clear();

        // TODO (Etapa 5.4b): chamar o _agent.RunAsync(userMessage) e retornar o texto.
        // try
        // {
        //     var result = await _agent.RunAsync(userMessage, cancellationToken: ct);
        //     return new ChatResponse
        //     {
        //         Reply = result.Text ?? string.Empty,
        //         ToolsInvoked = _lastToolsInvoked.ToList(),
        //     };
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Falha ao executar agente.");
        //     return new ChatResponse { HasError = true, Error = ex.Message };
        // }

        await Task.CompletedTask;
        return new ChatResponse
        {
            Reply = "[Starter] AgentService ainda nao implementado. Siga os TODOs em Services/AgentService.cs.",
            ToolsInvoked = new List<string>()
        };
    }

    // --------------------------------------------------------------------
    // TOOLS (funcoes que o modelo pode chamar)
    // --------------------------------------------------------------------

    // TODO (Etapa 5.5): implementar a KnowledgeBaseTool.
    // Ela deve usar _kb.FindBest(query) e devolver o conteudo como string.
    // Se nao encontrar, devolva uma string que o agente consiga interpretar (ex.: "NADA_ENCONTRADO").
    [Description("Busca na base de conhecimento interna da UniAnchieta informacoes sobre procedimentos e duvidas comuns.")]
    private string SearchKnowledgeBase(
        [Description("Pergunta ou tema a ser pesquisado, em linguagem natural.")]
        string query)
    {
        lock (_toolsLock) _lastToolsInvoked.Add("search_knowledge_base");

        // TODO: implementar
        throw new NotImplementedException("TODO: implementar na Etapa 5.5.");
    }

    // TODO (Etapa 5.6): implementar a TicketTool.
    // Deve chamar _tickets.Open(requester, category, description, priority)
    // e devolver uma string formatada com id, categoria, prioridade e status.
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

        // TODO: implementar
        throw new NotImplementedException("TODO: implementar na Etapa 5.6.");
    }
}
