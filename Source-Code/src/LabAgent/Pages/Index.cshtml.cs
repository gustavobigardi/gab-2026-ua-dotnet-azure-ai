using System.Text.Json;
using LabAgent.Models;
using LabAgent.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabAgent.Pages;

public class IndexModel : PageModel
{
    private readonly AgentService _agent;

    public IndexModel(AgentService agent)
    {
        _agent = agent;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostChatAsync()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        var req = JsonSerializer.Deserialize<ChatRequest>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new ChatRequest();

        if (string.IsNullOrWhiteSpace(req.Message))
            return new JsonResult(new ChatResponse { HasError = true, Error = "Mensagem vazia." });

        var response = await _agent.SendAsync(req.Message, HttpContext.RequestAborted);

        return new JsonResult(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
