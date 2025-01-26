namespace WebApp.Client.Services;

public interface IChatService
{
    public void SendUserMessage(string message);
    public void StartNewChat();
}