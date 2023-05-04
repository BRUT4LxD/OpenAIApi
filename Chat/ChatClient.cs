using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using Chat.DTO;

namespace Chat;

public class ChatClient : IChatClient
{
    public ChatConfig Config { get; } = new();

    public string ApiKey { get; }

    private readonly HttpClient _httpClient;

    public ChatClient(string apiKey, ChatConfig config, HttpClient httpClient)
    {
        Config = config ?? new ChatConfig();
        ApiKey = apiKey;
        _httpClient = httpClient;
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
        HttpRequestMessage request = CreateRequestMessage(requestBody);

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        if (requestBody.Stream)
        {
            return await StreamResponse(callback, response);
        }

        var content = await response.Content.ReadFromJsonAsync<ChatResponse>() ?? throw new Exception("Unknown error");
        if (content.Error is not null) throw new Exception(content.Error.Message);
        return content;
    }

    private HttpRequestMessage CreateRequestMessage(ChatRequest requestBody)
    {
        return new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(ChatConstants.ChatUrl),
            Headers =
            {
                {"Authorization", $"Bearer {ApiKey}" },
                {"ContentType",  MediaTypeNames.Application.Json }
            },
            Content = JsonContent.Create(requestBody)
        };
    }

    private static async Task<ChatResponse> StreamResponse(Action<ChatStreamChunkResponse> callback, HttpResponseMessage response)
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
