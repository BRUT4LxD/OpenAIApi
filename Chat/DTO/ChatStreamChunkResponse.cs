﻿using System.Text.Json.Serialization;

namespace Chat.DTO;

public class ChatStreamChunkResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("choices")]
    public List<ChatChunkResponseChoice> Choices { get; set; }
}
