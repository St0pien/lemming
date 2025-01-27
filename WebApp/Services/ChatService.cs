using Model;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using WebApp.Client.Services;
using WebApp.Tools;

namespace WebApp.Services;

public class ChatService : IChatService, IChatStatusService
{
    // private const string SystemPrompt = "You are helpful bot with ability to use embedded web browser for automated browser tasks and scraping activities. Use multiple tools at once if it'll help you achieve the task";
    // private const string SystemPrompt = "You are browser automationt bot, you job is to orchestrate attached browser actions in order to achieve goals speciefied by user. Use you attached browser to find information, resources etc. Sequence your tool calls in order to perform task, for example: first google the resources click one of the links and retrieve content. Be proffesional and concise. Now wait for user instructions";
    // private const string SystemPrompt = "You are an automation bot with access to web browser use it to help your users";
    // private const string SystemPrompt = "You are an advanced AI which is responsible for planning and executing actions on attached to your system web browser in order to achieve tasks specified by the user. Use mix of your tools to achieve the goal. For example when use wants you to find something you can search the web for articles and then open of them and provide info about it to the user";
    // private const string SystemPrompt = "You are a helpful AI assistant with access to attached web browser allowing you to search internet for latest informations, use this ability to answer based on fresh news from the world instead of relying on pretrained data";

    private const string SystemPrompt = "You are helpful AI LLM Chat bot, but with unique ability to search internet for new information use it when your baked in knowledge is not enough";

    private List<Model.Message> messages = [];
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
    private IBrowserTools browserTools;

    public ChatService(IBrowserTools browserTools)
    {
        this.browserTools = browserTools;
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

        var response = chat.SendAsync(message, browserTools.GetTools(), cancellationToken: cancellation.Token);
        messages.Add(new(MessageAuthor.Bot, ""));

        bool toolsResolved;
        do
        {
            toolsResolved = true;
            await foreach (var chunk in response)
            {
                var last = messages.Last();
                messages.RemoveAt(messages.Count - 1);
                messages.Add(new(MessageAuthor.Bot, last.Content += chunk));
                if (!string.IsNullOrEmpty(chunk))
                {
                    OnChatUpdate?.Invoke();
                }
            }
            var toolCalls = chat.Messages.LastOrDefault()?.ToolCalls?.ToArray() ?? [];
            if (toolCalls.Any())
            {
                toolsResolved = false;
                foreach (var tool in toolCalls)
                {
                    Console.WriteLine($"Call: {tool.Function!.Name}({string.Join(", ", tool.Function.Arguments!.Select(arg => $"{arg.Key}:{arg.Value}"))})");

                    var toolResult = browserTools.ExecuteTool(tool.Function);

                    var last = messages.Last();
                    messages.RemoveAt(messages.Count - 1);
                    messages.Add(new(MessageAuthor.Bot, "[Searching web for related information]"));
                    messages.Add(last);
                    response = chat.SendAsAsync(ChatRole.Tool, toolResult, cancellation.Token);
                }
            }
        } while (!toolsResolved);

        ChatState = ChatState.Ready;
    }

    public void SendUserMessage(string message)
    {
        if (ChatState == ChatState.NotInitiliazed)
        {
            return;
        }

        if (ChatState != ChatState.Ready)
        {
            cancellation.Cancel();
            cancellation = new CancellationTokenSource();
        }
        messages.Add(new(MessageAuthor.User, message));
        OnChatUpdate?.Invoke();

        Task.Run(() => AskLLM(message));
    }

    public List<Model.Message> GetMessages()
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