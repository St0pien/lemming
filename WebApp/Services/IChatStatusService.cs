using Model;

namespace WebApp.Services;


public interface IChatStatusService
{
    public event Action? OnChatUpdate;
    public List<Message> GetMessages();
    public ChatState ChatState { get; }
    public event EventHandler<ChatState> OnStateChanged;
}