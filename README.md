# Building Your First AI Agent with Azure OpenAI & .NET

> **Global Azure 2026 — UniAnchieta**
> Hands-on lab: build a virtual support agent powered by **Azure OpenAI** and **.NET 10**.

---

## 📁 Repository Structure

```
├── Photos/          # Event and lab photos
├── Slides/          # Presentation deck (PPTX)
├── Source-Code/     # Lab project source code
└── User-Guide/      # Step-by-step participant guide
```

### Photos
Event and lab session photos from the Global Azure 2026 gathering at UniAnchieta.

### Slides
PowerPoint presentation used during the talk: **"Slides - GlobalAzure 2026.pptx"**.

### Source-Code
ASP.NET Core (Razor Pages) application that serves as the lab scaffold.  
The agent represents the fictional **UniAnchieta** support desk and can answer questions from a local knowledge base or open support tickets when human intervention is needed.

Key technologies:

| Technology | Version |
|---|---|
| .NET / ASP.NET Core (Razor Pages) | 10 |
| Azure.AI.OpenAI | 2.2.0-beta.4 |
| Microsoft.Agents.AI | 1.1.0 |
| Microsoft.Extensions.AI | 10.4.0 |

> The `AgentService` contains `TODO` blocks for participants to implement step by step.  
> A fully completed reference version is available on the **demo-finalizada** branch.

### User-Guide
`GUIA-DO-PARTICIPANTE.md` — Portuguese-language step-by-step guide covering environment setup, Azure OpenAI configuration, and all lab implementation blocks.

---

## 🚀 Quick Start

1. Clone this repository.
2. Open `Source-Code/src/LabAgent/appsettings.json` and fill in your Azure OpenAI credentials (`Endpoint`, `Key`, `DeploymentName`).
3. Run the project:
   ```bash
   cd Source-Code/src/LabAgent
   dotnet run
   ```
4. Follow the instructions in `User-Guide/GUIA-DO-PARTICIPANTE.md`.

---

## 📋 Prerequisites

- .NET 10 SDK
- VS Code or Visual Studio 2022+
- Access to an **Azure OpenAI** resource with a chat model deployment (e.g., `gpt-4o-mini`)

---

---

# Construindo seu Primeiro AI Agent com Azure OpenAI & .NET

> **Global Azure 2026 — UniAnchieta**
> Lab prático: construa um agente de suporte virtual com **Azure OpenAI** e **.NET 10**.

---

## 📁 Estrutura do Repositório

```
├── Photos/          # Fotos do evento e das sessões de lab
├── Slides/          # Apresentação em PPTX
├── Source-Code/     # Código-fonte do projeto do lab
└── User-Guide/      # Guia do participante passo a passo
```

### Photos
Fotos do evento e das sessões de laboratório do Global Azure 2026 na UniAnchieta.

### Slides
Apresentação PowerPoint utilizada durante a palestra: **"Slides - GlobalAzure 2026.pptx"**.

### Source-Code
Aplicação ASP.NET Core (Razor Pages) que serve como scaffold do laboratório.  
O agente representa a central de atendimento fictícia da **UniAnchieta** e é capaz de responder perguntas usando uma base de conhecimento local ou abrir chamados de suporte quando a situação exige intervenção humana.

Principais tecnologias:

| Tecnologia | Versão |
|---|---|
| .NET / ASP.NET Core (Razor Pages) | 10 |
| Azure.AI.OpenAI | 2.2.0-beta.4 |
| Microsoft.Agents.AI | 1.1.0 |
| Microsoft.Extensions.AI | 10.4.0 |

> O `AgentService` contém blocos `TODO` para os participantes implementarem passo a passo.  
> A versão finalizada do projeto encontra-se na branch **demo-finalizada** para referência.

### User-Guide
`GUIA-DO-PARTICIPANTE.md` — guia em português com o passo a passo completo: configuração do ambiente, credenciais do Azure OpenAI e todos os blocos de implementação do lab.

---

## 🚀 Como Começar

1. Clone este repositório.
2. Abra `Source-Code/src/LabAgent/appsettings.json` e preencha suas credenciais do Azure OpenAI (`Endpoint`, `Key`, `DeploymentName`).
3. Execute o projeto:
   ```bash
   cd Source-Code/src/LabAgent
   dotnet run
   ```
4. Siga as instruções em `User-Guide/GUIA-DO-PARTICIPANTE.md`.

---

## 📋 Pré-requisitos

- .NET 10 SDK
- VS Code ou Visual Studio 2022+
- Acesso a um recurso **Azure OpenAI** com um deployment de modelo de chat (ex.: `gpt-4o-mini`)
