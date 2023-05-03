using System.Text.Json.Serialization;

namespace ChatGPT.DTO;

public class ChatMessage
{
    /// <summary>
    /// The role of the author of this message. One of system, user, or assistant.
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; } = "user";

    /// <summary>
    /// The contents of the message.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }

    /// <summary>
    /// The name of the author of this message. May contain a-z, A-Z, 0-9, and underscores, with a maximum length of 64 characters.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
