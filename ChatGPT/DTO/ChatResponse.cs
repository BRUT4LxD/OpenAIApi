using System.Text.Json.Serialization;

namespace ChatGPT.DTO;

public class ChatResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("usage")]
    public ChatResponseUsage Usage { get; set; }

    [JsonPropertyName("choices")]
    public List<ChatResponseChoice> Choices { get; set; }

    [JsonPropertyName("error")]
    public ChatResponseError Error { get; set; }

    public static ChatResponse Create(ChatStreamChunkResponse chunkResponse, string currentMessage)
    {
        if (chunkResponse is null) throw new ArgumentNullException(nameof(chunkResponse));

        return new ChatResponse
        {
            Id = chunkResponse.Id,
            Model = chunkResponse.Model,
            Created = chunkResponse.Created,
            Choices = new List<ChatResponseChoice>
                {
                    new()
                    {
                        Message = new ChatMessage
                        {
                            Content = currentMessage
                        }
                    }
                }
        };
    }
}
