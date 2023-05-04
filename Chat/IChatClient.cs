using Chat.DTO;

namespace Chat
{
    public interface IChatClient
    {
        Task<string> AskAsync(ChatMessage message, List<ChatMessage> historyMessages = null);

        Task<string> AskStreamAsync(ChatMessage message, Action<string> callback, List<ChatMessage> historyMessages = null);
    }
}