using Model;

namespace WebApp.Services;

public interface IMessageRelayService
{
    public event EventHandler<Message>? OnMessage;
    public List<Message> GetMessages();
}