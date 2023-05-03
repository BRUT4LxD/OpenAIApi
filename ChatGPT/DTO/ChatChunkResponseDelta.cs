using System.Text.Json.Serialization;

namespace ChatGPT.DTO;

public class ChatChunkResponseDelta
{
    [JsonPropertyName("content")]
    public string Content { get; set; }
}