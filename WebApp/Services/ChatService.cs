using Model;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using WebApp.Client.Services;

namespace WebApp.Services;

public class ChatService : IChatService, IChatStatusService
{
    private const string SystemPrompt = "You are helpful bot with ability to use embedded web browser for automated browser tasks and scrapign activities. Use multiple tools at once if it'll help you achieve the task";

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

        var response = chat.SendAsync(message, [new NavigateTool(), new GetPageContentTool()], cancellationToken: cancellation.Token);
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
                    var last = messages.Last();
                    if (tool.Function!.Name == "get_page_content")
                    {
                        messages.RemoveAt(messages.Count - 1);
                        messages.Add(new(MessageAuthor.Bot, "[Browser action successfull]\nPageContent:\n<!DOCTYPE html><html><h1>x.com</html>"));
                        messages.Add(last);
                        Console.WriteLine("Hooray");
                        response = chat.SendAsAsync(ChatRole.Tool, "[Browser action successfull]\nPageContent:\n<!DOCTYPE html><html><h1>x.com</html>", cancellation.Token);
                    }
                    else
                    {
                        messages.RemoveAt(messages.Count - 1);
                        messages.Add(new(MessageAuthor.Bot, "[Browser action successfull]"));
                        messages.Add(last);
                        response = chat.SendAsAsync(ChatRole.Tool, "[Browser action successfull]", cancellation.Token);
                    }
                }
            }
        } while (!toolsResolved);

        ChatState = ChatState.Ready;
    }

    public void SendUserMessage(string message)
    {
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


public sealed class NavigateTool : Tool
{
    public NavigateTool()
    {
        Function = new Function
        {
            Description = "Navigate to the url within your browser",
            Name = "navigate",
            Parameters = new Parameters
            {
                Properties = new Dictionary<string, Property>
                {
                    ["url"] = new() { Type = "string", Description = "url you want to navigate to" }
                }

            }
        };

        Type = "function";
    }
}

public sealed class GotoTool : Tool
{
    public GotoTool()
    {
        Function = new Function
        {
            Description = "navigate to a given url using embedded browser",
            Name = "navigate",
            Parameters = new Parameters
            {
                Properties = new Dictionary<string, Property>
                {
                    ["url"] = new() { Type = "string", Description = "The url where embedded browser should navigate to" },
                },
                Required = ["url"],
            }
        };
        Type = "function";
    }
}


public sealed class GetPageContentTool : Tool
{
    public GetPageContentTool()
    {
        Function = new Function
        {
            Description = "Fetch html content from current opened page in embedded browser",
            Name = "get_page_content",
        };
        Type = "function";
    }
}