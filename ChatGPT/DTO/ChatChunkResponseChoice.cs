using System.Text.Json.Serialization;

namespace ChatGPT.DTO;

public class ChatChunkResponseChoice
{
    [JsonPropertyName("delta")]
    public ChatChunkResponseDelta Delta { get; set; }

    [JsonPropertyName("index")]
    public long Index { get; set; }

    [JsonPropertyName("finish_reason")]
    public object FinishReason { get; set; }
}
