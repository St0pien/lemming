using Model;
using OllamaSharp;
using WebApp.Client.Services;

namespace WebApp.Services;

public class ChatService : IChatService, IMessageRelayService
{
    private List<Message> messages = [];

    public event EventHandler<Message>? OnMessage;

    public void SendUserMessage(string message)
    {
        messages.Add(new(MessageAuthor.User, message));
        Console.WriteLine(message);
        OnMessage?.Invoke(this, messages.Last());

        messages.Add(new(MessageAuthor.Bot, "Papuga:sfdsldkfsldf;asklasdfkj;lasdfkjl;afdskjl;asfkjfalsadlkfkljsfdljksfdjklsdfjklsfdjklsfdjklsfdjklsfdjklsdfjklsfdjklkasdjkldfsjkladfljkasd; " + message));
        OnMessage?.Invoke(this, messages.Last());
    }

    public List<Message> GetMessages()
    {
        return messages;
    }
}