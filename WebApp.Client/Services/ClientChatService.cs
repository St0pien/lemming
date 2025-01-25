using System.Net.Http.Json;
using Model;

namespace WebApp.Client.Services;

public record struct MessagePaylod(string Message);

public class ClientChatService(HttpClient httpClient) : IChatService
{
    public void SendUserMessage(string message)
    {
        Console.WriteLine(httpClient.BaseAddress);
        httpClient.PostAsJsonAsync("chat", new MessagePaylod
        {
            Message = message

        });
    }
}