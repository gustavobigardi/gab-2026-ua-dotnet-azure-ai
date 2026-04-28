# Guia do Participante — Criando seu primeiro AI Agent com Azure OpenAI

> **Global Azure 2026 / UniAnchieta**
> Duração alvo: **2 horas úteis**.
> Stack: **.NET 10 · Razor Pages · Microsoft Agent Framework · Azure OpenAI**.

---

## 0. Pré-requisitos

- .NET 10 SDK (`dotnet --version` ≥ 10.0).
- VS Code ou Visual Studio 2026.
- Acesso a um **Azure OpenAI** com um deployment de modelo de chat (ex.: `gpt-4o-mini`).
  - Caminho A: conta/voucher Azure próprio.
  - Caminho B: recurso compartilhado do instrutor (endpoint + key + deployment name).
- Clone do repositório do lab.

Estrutura entregue:
```
src/
  LabAgent.Starter/   ← projeto base (com TODOs — aqui você vai trabalhar)
  LabAgent.Final/     ← projeto completo (referência e fallback)
samples/knowledge-base.json
```

---

## Bloco 1 — Setup Azure e configuração local

### Caminho A — Participante com conta Azure

1. Portal Azure → criar `Resource Group` (ex.: `rg-lab-ai-agent`).
2. Criar recurso **Azure OpenAI** em região disponível.
3. Abrir o recurso → **Model deployments** → criar deployment de modelo de chat (`gpt-4o-mini`).
4. Copiar `Endpoint`, `Key` e `Deployment name`.

### Caminho B — Recurso compartilhado

O instrutor fornece as 3 variáveis pelo canal combinado (ex.: QR code ou slide de apoio).

### Configuração local (recomendado: **User Secrets**)

```bash
cd src/LabAgent.Starter
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:Endpoint"       "https://SEU-RECURSO.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey"         "SUA_CHAVE"
dotnet user-secrets set "AzureOpenAI:DeploymentName" "gpt-4o-mini"
```

> Alternativa rápida: editar `appsettings.Development.json` — lembre de **não commitar** a chave.

**Checkpoint 1** ✔️ Todos conseguiram `dotnet run` e ver a UI em `https://localhost:5001`.

---

## Bloco 2 — Hands-on: implementando o AgentService

### Etapa 2.1 — Abrir o projeto base

```bash
cd src/LabAgent.Starter
dotnet run
```

A interface de chat abre. Qualquer mensagem devolve o stub *"[Starter] AgentService ainda não implementado…"*.

Abra `Services/AgentService.cs` — é nele que vamos trabalhar.

---

### Etapa 2.2 — Configurar credenciais

No construtor de `AgentService`, localize o TODO da Etapa 5.2 e adicione:

```csharp
var endpoint   = configuration["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("AzureOpenAI:Endpoint não configurado.");
var apiKey     = configuration["AzureOpenAI:ApiKey"]
    ?? throw new InvalidOperationException("AzureOpenAI:ApiKey não configurado.");
var deployment = configuration["AzureOpenAI:DeploymentName"]
    ?? throw new InvalidOperationException("AzureOpenAI:DeploymentName não configurado.");
```

**Checkpoint 2** ✔️ Endpoint, key e deployment lidos sem exceção.

---

### Etapa 2.3 — Escrever as instruções do agente

Substitua o `AgentInstructions` pela versão orientada: (localize o TODO da Etapa 5.3)

```text
Você é o assistente de triagem e atendimento interno da UniAnchieta.
Sempre que possível, use a ferramenta `search_knowledge_base` antes de responder.
Quando o caso exigir ação humana (problema persistente, acesso bloqueado), use `open_support_ticket`.
Se a pergunta estiver ambígua, peça mais contexto.
Não invente políticas, prazos ou URLs.
Responda em português, em no máximo 4 parágrafos curtos.
```

> A instrução molda o comportamento do agente, mas não substitui dados reais — é só orientação de como agir.

---

### Etapa 2.4 — Chamar o Azure OpenAI

Ainda no construtor, depois das credenciais: (localize o TODO da Etapa 5.4)

```csharp
var azureClient = new AzureOpenAIClient(
    new Uri(endpoint),
    new ApiKeyCredential(apiKey));

_agent = azureClient
    .GetChatClient(deployment)
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()   // habilita chamada automática das tools em C#
    .Build()
    .AsAIAgent(
        instructions: AgentInstructions,
        name: "UniAnchietaTriageAgent",
        tools:
        [
            AIFunctionFactory.Create(SearchKnowledgeBase, name: "search_knowledge_base",
                description: "Busca na base de conhecimento interna da UniAnchieta."),
            AIFunctionFactory.Create(OpenSupportTicket,   name: "open_support_ticket",
                description: "Abre um chamado de suporte interno."),
        ]);
```

E, logo abaixo no método `SendAsync`, troque o trecho comentado por: (localize o TODO da Etapa 5.4b)

```csharp
try
{
    var result = await _agent.RunAsync(userMessage, cancellationToken: ct);
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
```

Declare o campo `private readonly AIAgent _agent;`. (localize o TODO da Etapa 5.4c)

**Checkpoint 3** ✔️ Uma pergunta simples já devolve resposta do modelo (ainda sem usar tools).

---

### Etapa 2.5 — Knowledge Base Tool

Implemente `SearchKnowledgeBase`: (localize o TODO da Etapa 5.5)

```csharp
var entry = _kb.FindBest(query);
if (entry is null)
    return "NADA_ENCONTRADO: não há entrada sobre esse assunto na base.";
return $"Tópico: {entry.Topic}\nConteúdo: {entry.Content}";
```

> A tool é da **aplicação**, não do modelo. Hoje é JSON; amanhã pode ser banco, API ou busca vetorial.

**Checkpoint 4** ✔️ "Esqueci minha senha do portal" retorna conteúdo da base.

---

### Etapa 2.6 — Ticket Tool

Implemente `OpenSupportTicket`: (localize o TODO da Etapa 5.6)

```csharp
var t = _tickets.Open(requester, category, description, priority);
return
    $"Ticket {t.TicketId} criado com sucesso.\n" +
    $"Categoria: {t.Category}\nPrioridade: {t.Priority}\nStatus: {t.Status}";
```

**Checkpoint 5** ✔️ "Não consigo entrar mesmo depois de trocar a senha" → agente abre chamado.

---

### Etapa 2.7 — Fechar o fluxo na UI

A UI já faz POST para `?handler=Chat`. Fluxo completo:

```text
Usuário ─▶ Razor Page ─▶ AgentService ─▶ Azure OpenAI (modelo)
                             │             │
                             │             └─▶ decide chamar tools
                             └─▶ Tool: KnowledgeBase (JSON local)
                             └─▶ Tool: Ticket (simulado em memória)
```

---

## Bloco 3 — Testes e refinamento

| # | Entrada | Comportamento esperado |
|---|---|---|
| 1 | `Esqueci minha senha do portal.` | Usa `search_knowledge_base`. |
| 2 | `Preciso de uma declaração de matrícula.` | Usa KB, orientação objetiva. |
| 3 | `Não consigo acessar mesmo depois de redefinir a senha.` | Usa `open_support_ticket`. |
| 4 | `Não está funcionando.` | Agente pede mais contexto. |
| 5 | `Pode calcular meu imposto de renda?` | Recusa educadamente. |

**Pontos para explorar:**
- Endurecer instruções (ex.: "nunca invente URLs que não estão na base").
- Ajustar as descrições das tools para guiar melhor o modelo na escolha.
- Validar parâmetros no C# antes de acionar a ação.

---

## Próximos passos

O que você construiu hoje:
- Aplicação web .NET real integrada ao Azure OpenAI.
- Duas tools em C# transformando um chat em agente.

Caminhos naturais de evolução:
- Trocar JSON por banco ou API real.
- RAG com documentos e busca vetorial.
- Autenticação e autorização.
- Streaming de resposta.
- Múltiplos agents por domínio.
- Observabilidade com OpenTelemetry.

---

## Apêndice — Prompts de teste rápidos

**Simples:**
- `Esqueci minha senha do portal.`
- `Como emito uma declaração de matrícula?`
- `Como acesso meu e-mail institucional?`

**Intermediário:**
- `Não consigo entrar mesmo depois de trocar a senha.`
- `Meu acesso ao portal continua bloqueado.`
- `Quero abrir um chamado para suporte.`

**Ambíguo:**
- `Não está funcionando.`

**Fora de escopo:**
- `Me ajude a montar uma campanha de marketing.`
