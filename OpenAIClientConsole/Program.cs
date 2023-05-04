using Chat;
using Chat.DTO;

const string apiKey = "";
var config = new ChatConfig
{
    Model = ChatConstants.ChatModels.GPT35Turbo,
    MaxTokens = 100,
};
ChatClient client = new(apiKey, config, new HttpClient());

var message = new ChatMessage
{
    Content = "What is the difference between a cow and a fly?",
    Name = "George",
    Role = "user"
};

string r = await client.AskAsync(message);

Console.WriteLine(r);
