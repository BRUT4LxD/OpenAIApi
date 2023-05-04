using System.Text.Json.Serialization;

namespace Chat.DTO;

public class ChatChunkResponseDelta
{
    [JsonPropertyName("content")]
    public string Content { get; set; }
}