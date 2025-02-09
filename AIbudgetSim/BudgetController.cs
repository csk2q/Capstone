using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/budget-advice")]
public class BudgetController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public BudgetController(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"];
    }

    [HttpPost]
    public async Task<IActionResult> GetBudgetAdvice([FromBody] BudgetInput budget)
    {
        // Format the user's budget into a readable message for AI
        string userBudgetSummary = $@"
            Monthly Income: ${budget.Income}
            Housing: ${budget.Housing}
            Electric: ${budget.Electric}
            Internet: ${budget.Internet}
            Gas: ${budget.Gas}
            Water: ${budget.Water}
            Trash: ${budget.Trash}
            Food: ${budget.Food}
            Leisure: ${budget.Leisure}
            Savings: ${budget.Savings}
            Investments: ${budget.Investments}

            Based on this budget, please provide recommendations on how to reduce spending and save more money.
        ";

        // Create the AI request payload
        var aiRequest = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new { role = "system", content = "You are a financial expert providing budgeting advice." },
                new { role = "user", content = userBudgetSummary }
            }
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(aiRequest), Encoding.UTF8, "application/json");

        // Set OpenAI authorization header
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        // Send request to OpenAI API
        HttpResponseMessage response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest("Failed to get AI response.");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var aiResponse = JsonSerializer.Deserialize<AIResponse>(responseBody);

        return Ok(new { advice = aiResponse.Choices[0].Message.Content });
    }
}

// Define the budget input structure
public class BudgetInput
{
    public int Income { get; set; }
    public int Housing { get; set; }
    public int Electric { get; set; }
    public int Internet { get; set; }
    public int Gas { get; set; }
    public int Water { get; set; }
    public int Trash { get; set; }
    public int Food { get; set; }
    public int Leisure { get; set; }
    public int Savings { get; set; }
    public int Investments { get; set; }
}

// Define AI response structure
public class AIResponse
{
    public Choice[] Choices { get; set; }
}

public class Choice
{
    public Message Message { get; set; }
}

public class Message
{
    public string Content { get; set; }
}
