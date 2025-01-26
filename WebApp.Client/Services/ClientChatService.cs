using System.Net.Http.Json;

namespace WebApp.Client.Services;

public record struct MessagePaylod(string Message);

public class ClientChatService(HttpClient httpClient) : IChatService
{
    public void SendUserMessage(string message)
    {
        httpClient.PostAsJsonAsync("chat", new MessagePaylod
        {
            Message = message

        });
    }

    public void StartNewChat()
    {
        Task.Run(() => httpClient.PostAsJsonAsync("chat/init", ""));
    }
}