using System.Text.Json.Serialization;

namespace ChatGPT.DTO;

public class ChatResponseChoice
{
    [JsonPropertyName("message")]
    public ChatMessage Message { get; set; }

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }

    [JsonPropertyName("index")]
    public long Index { get; set; }
}
