# LabAgent — Global Azure 2026

Lab prático apresentado no **Global Azure 2026** demonstrando como construir um agente de suporte virtual com **Azure OpenAI** e **.NET 10**.

O agente representa a central de atendimento fictícia da **UniAnchieta** e é capaz de responder perguntas usando uma base de conhecimento local ou abrir chamados de suporte quando a situação exige intervenção humana.

> Este projeto é um **scaffold de laboratório**: o núcleo do `AgentService` contém blocos `TODO` que os participantes devem implementar passo a passo, com comentários de referência indicando o código final.

> A versão finalizada do projeto encontra-se na branch **demo-finalizada**, para refeerência.

---

## Visão Geral

```
Usuário digita mensagem no browser
  → POST ?handler=Chat (JSON)
    → AgentService.SendAsync(mensagem)
      → Azure OpenAI GPT-4o-mini
          → [ferramenta] search_knowledge_base  →  KnowledgeBaseService.FindBest()
          → [ferramenta] open_support_ticket    →  TicketService.Open()
      → ChatResponse { Reply, ToolsInvoked }
  → Exibido como bolha de resposta no chat
```

---

## Stack

| Tecnologia | Versão |
|---|---|
| .NET / ASP.NET Core (Razor Pages) | 10 |
| Azure.AI.OpenAI | 2.2.0-beta.4 |
| Microsoft.Agents.AI | 1.1.0 |
| Microsoft.Agents.AI.OpenAI | 1.1.0 |
| Microsoft.Extensions.AI | 10.4.0 |

---

## Estrutura do Projeto

```
src/
├── LabAgent/
│   ├── Program.cs                  # Configuração do host e injeção de dependências
│   ├── appsettings.json            # Configurações do Azure OpenAI (preencher antes de rodar)
│   ├── Data/
│   │   └── knowledge-base.json     # Base de conhecimento com 8 tópicos de suporte
│   ├── Models/
│   │   ├── ChatRequest.cs          # DTO da requisição de chat
│   │   └── ChatResponse.cs         # DTO da resposta (reply, ferramentas invocadas, erros)
│   ├── Pages/
│   │   └── Index.cshtml(.cs)       # UI de chat em Razor Pages com JavaScript vanilla
│   └── Services/
│       ├── AgentService.cs         # Núcleo do lab — integração com o AIAgent
│       ├── KnowledgeBaseService.cs # Busca por termos na base de conhecimento
│       └── TicketService.cs        # Armazenamento em memória de chamados de suporte
```

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Recurso de **Azure OpenAI** com um deployment do modelo `gpt-4o-mini`

---

## Como Executar

### 1. Configure as credenciais

Abra `src/LabAgent/appsettings.json` e preencha os valores do Azure OpenAI:

```json
"AzureOpenAI": {
  "Endpoint": "https://<SEU-RECURSO>.openai.azure.com/",
  "ApiKey": "<SUA-CHAVE>",
  "DeploymentName": "gpt-4o-mini"
}
```

### 2. Execute o projeto

```bash
cd src
dotnet run --project LabAgent/LabAgent.csproj
```

Abra o navegador na URL HTTPS exibida no terminal.

---

## Ferramentas do Agente

| Ferramenta | Descrição |
|---|---|
| `search_knowledge_base` | Busca a resposta mais relevante na base de conhecimento local |
| `open_support_ticket` | Abre um chamado no sistema interno quando é necessário atendimento humano |

---

## Base de Conhecimento

O arquivo `Data/knowledge-base.json` contém 8 entradas cobrindo os tópicos de suporte mais comuns:

- Redefinição de senha
- Acesso ao portal acadêmico
- Declarações de matrícula
- E-mail institucional (Outlook)
- AVA / Moodle
- Horário de atendimento
- Abertura de chamados de TI
- Consultas financeiras

---

## Licença

Este projeto foi criado para fins educacionais no contexto do Global Azure 2026.
