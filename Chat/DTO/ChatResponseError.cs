using System.Text.Json.Serialization;

namespace Chat.DTO;

public class ChatResponseError
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("param")]
    public object Param { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }
}