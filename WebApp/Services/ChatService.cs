using System.Security.Cryptography;
using Model;
using OllamaSharp;
using WebApp.Client.Services;

namespace WebApp.Services;

public class ChatService : IChatService, IChatStatusService
{
    private const string SystemPrompt = "You are just friendly AI bot to chat about some stupid things";

    private List<Message> messages = [];
    public event Action? OnChatUpdate;
    private ChatState _chatState = ChatState.NotInitiliazed;
    public ChatState ChatState
    {
        private set
        {
            _chatState = value;
            OnStateChanged?.Invoke(this, value);
        }
        get => _chatState;
    }
    public event EventHandler<ChatState>? OnStateChanged = null;
    private Chat? chat = null;
    private CancellationTokenSource cancellation = new CancellationTokenSource();

    public ChatService()
    {
        Task.Run(Init);
    }

    private async Task Init()
    {
        Console.WriteLine("[Chat] Initializing new chat");
        ChatState = ChatState.NotInitiliazed;
        var client = new OllamaApiClient("http://localhost:11434", "llama3.2");
        chat = new Chat(client, SystemPrompt);
        var firstResposne = chat.SendAsync("", cancellation.Token);
        messages.Add(new(MessageAuthor.Bot, ""));
        await foreach (var chunk in firstResposne)
        {
            var last = messages.Last();
            messages.RemoveAt(messages.Count - 1);
            messages.Add(new(MessageAuthor.Bot, last.Content += chunk));
            OnChatUpdate?.Invoke();
        }
        ChatState = ChatState.Ready;
    }

    private async Task AskLLM(string message)
    {
        if (chat == null)
        {
            throw new InvalidOperationException("LLM chat not initialized");
        }

        ChatState = ChatState.Processing;

        var response = chat.SendAsync(message, cancellation.Token);
        messages.Add(new(MessageAuthor.Bot, ""));
        await foreach (var chunk in response)
        {
            var last = messages.Last();
            messages.RemoveAt(messages.Count - 1);
            messages.Add(new(MessageAuthor.Bot, last.Content += chunk));
            OnChatUpdate?.Invoke();
        }
        ChatState = ChatState.Ready;
    }

    public void SendUserMessage(string message)
    {
        cancellation.Cancel();
        cancellation = new CancellationTokenSource();
        messages.Add(new(MessageAuthor.User, message));
        OnChatUpdate?.Invoke();

        Task.Run(() => AskLLM(message));
    }

    public List<Message> GetMessages()
    {
        return messages;
    }

    public void StartNewChat()
    {
        cancellation.Cancel();
        cancellation = new CancellationTokenSource();
        messages.Clear();
        OnChatUpdate?.Invoke();
        Task.Run(Init);
    }
}