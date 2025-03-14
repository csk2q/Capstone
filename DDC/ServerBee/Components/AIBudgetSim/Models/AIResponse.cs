using System.Text.Json.Serialization;

namespace ServerBee.AIBudgetSim.Models
{
    public class AIResponse
    {
        [JsonPropertyName("choices")]
        public Choice[] Choices { get; set; } = new Choice[0]; // ✅ Ensures Choices is initialized
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public Message? Message { get; set; } // ✅ Made nullable to prevent errors
    }

    public class Message
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; } // ✅ Made nullable to prevent null reference errors
    }
}
