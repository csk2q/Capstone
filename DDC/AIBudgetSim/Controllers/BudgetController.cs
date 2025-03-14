using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AIBudgetSimTesting.Models;

namespace AIBudgetSimTesting.Controllers
{
    [ApiController]
    [Route("api/budget-advice")]
    public class BudgetController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BudgetController> _logger;
private readonly string? _apiKey;
private readonly IConfiguration _configuration;

public BudgetController(ILogger<BudgetController> logger, IConfiguration configuration)
{
    _httpClient = new HttpClient();
    _logger = logger;
    _configuration = configuration;

    // Try getting API key from appsettings.json first
    _apiKey = _configuration["OpenAI:ApiKey"];

    // If not found, try the environment variable as a fallback
    if (string.IsNullOrEmpty(_apiKey))
    {
        _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    }

    if (string.IsNullOrEmpty(_apiKey))
    {
        _logger.LogError("❌ OpenAI API Key is missing! Ensure it's set in environment variables or appsettings.json.");
    }
    else
    {
        _logger.LogInformation("✅ OpenAI API Key: Loaded successfully");
    }
}


        [HttpPost]
        public async Task<IActionResult> GetBudgetAdvice([FromBody] BudgetInput input)
        {
            try
            {
                _logger.LogInformation("✅ Received Budget Input: {@Input}", input);

                if (input == null)
                {
                    _logger.LogWarning("⚠️ Received null input for budget advice.");
                    return BadRequest(new { advice = "Invalid input data." });
                }

                var aiResponse = await GetAIResponse(input);

                // ✅ Fixing possible null reference issue (CS8602)
                string finalAdvice = aiResponse?.Choices?[0]?.Message?.Content ?? "AI response was empty.";

                _logger.LogInformation("📤 Final Advice Sent to User: {FinalAdvice}", finalAdvice);

                return Ok(new { advice = finalAdvice });
            }
            catch (Exception ex)
            {
                _logger.LogError("❌ Error in GetBudgetAdvice: {Error}", ex.Message);
                return StatusCode(500, new { advice = "Error generating AI response." });
            }
        }

        private async Task<AIResponse?> GetAIResponse(BudgetInput input)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogError("❌ API Key is missing. Cannot send request.");
                return null;
            }

            var requestPayload = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = "You are a financial advisor helping people save money." },
                    new { role = "user", content = GeneratePrompt(input) }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(requestPayload);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Headers = { { "Authorization", $"Bearer {_apiKey}" } },
                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };

            _logger.LogInformation("📡 Sending request to OpenAI...");
            _logger.LogDebug("📨 Request Body: {Payload}", jsonPayload);

            var response = await _httpClient.SendAsync(requestMessage);

            _logger.LogInformation("📩 OpenAI Response Status Code: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("❌ OpenAI API Request Failed: {Status}", response.StatusCode);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("📥 OpenAI Response Body: {ResponseContent}", responseContent);

            return JsonSerializer.Deserialize<AIResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private string GeneratePrompt(BudgetInput input)
        {
            return $@"
                Monthly Income: ${input.Income}
                Housing: ${input.Housing}
                Electric: ${input.Electric}
                Internet: ${input.Internet}
                Gas: ${input.Gas}
                Water: ${input.Water}
                Trash: ${input.Trash}
                Food: ${input.Food}
                Leisure: ${input.Leisure}
                Savings: ${input.Savings}
                Investments: ${input.Investments}

                Based on this budget, how can I save more money while covering all necessary expenses?
            ";
        }
    }
}
