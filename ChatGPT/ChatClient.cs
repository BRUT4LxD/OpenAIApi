using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Chat;
using ChatGPT.DTO;

namespace ChatGPT;

public class ChatClient
{
    private static readonly HttpClient HttpClient = new();

    public ChatConfig Config { get; set; } = new();

    public string ApiKey { get; set; }

    public ChatClient(string apiKey, ChatConfig config)
    {
        Config = config ?? new ChatConfig();
        ApiKey = apiKey;
    }

    public async Task<string> AskAsync(ChatMessage message, List<ChatMessage> historyMessages = default)
    {
        var messages = historyMessages ??= new List<ChatMessage>();
        messages.Add(message);

        var reply = await SendMessageAsync(ChatRequest.Create(messages, Config));

        return reply.Choices.FirstOrDefault()?.Message.Content ?? "";
    }

    public async Task<string> AskStreamAsync(ChatMessage message, Action<string> callback, List<ChatMessage> historyMessages = default)
    {
        var messages = historyMessages ??= new List<ChatMessage>();
        messages.Add(message);

        var reply = await SendMessageAsync(ChatRequest.Create(messages, Config, stream: true), response =>
        {
            var content = response.Choices.FirstOrDefault()?.Delta.Content;

            callback(content);
        });


        return reply.Choices.FirstOrDefault()?.Message.Content ?? "";
    }

    private async Task<ChatResponse> SendMessageAsync(ChatRequest requestBody, Action<ChatStreamChunkResponse> callback = null)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Constants.ChatUrl),
            Headers =
            {
                {"Authorization", $"Bearer {ApiKey}" }
            },
            Content = new StringContent(JsonSerializer.Serialize(requestBody))
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue("application/json")
                }
            }
        };

        var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        if (requestBody.Stream)
        {
            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType != "text/event-stream")
            {
                var error = await response.Content.ReadFromJsonAsync<ChatResponse>();
                throw new Exception(error?.Error?.Message ?? "Unknown error");
            }

            var concatMessages = string.Empty;

            ChatStreamChunkResponse reply = null;
            var stream = await response.Content.ReadAsStreamAsync();
            await foreach (var data in StreamCompletion(stream))
            {
                var jsonString = data.Replace("data: ", "");
                if (string.IsNullOrWhiteSpace(jsonString)) continue;
                if (jsonString == "[DONE]") break;
                reply = JsonSerializer.Deserialize<ChatStreamChunkResponse>(jsonString);
                if (reply is null) continue;
                concatMessages += reply.Choices.FirstOrDefault()?.Delta.Content;
                callback?.Invoke(reply);
            }

            return ChatResponse.Create(reply, concatMessages);
        }

        var content = await response.Content.ReadFromJsonAsync<ChatResponse>() ?? throw new Exception("Unknown error");
        if (content.Error is not null) throw new Exception(content.Error.Message);
        return content;
    }

    private static async IAsyncEnumerable<string> StreamCompletion(Stream stream)
    {
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line != null)
            {
                yield return line;
            }
        }
    }
}
