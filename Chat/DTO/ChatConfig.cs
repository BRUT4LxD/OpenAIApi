namespace Chat.DTO;

public class ChatConfig
{
    public string Model { get; set; } = ChatConstants.ChatModels.GPT35Turbo;

    public double Temperature { get; set; } = 0.7;

    public double TopP { get; set; } = 0.3;

    public long MaxTokens { get; set; } = 20;

    public string[] Stop { get; set; }

    public double PresencePenalty { get; set; }

    public double FrequencyPenalty { get; set; }
}
