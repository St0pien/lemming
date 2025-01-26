using Model;
using WebApp.Services;

namespace WebApp.Components.UI.Chat;

public partial class Chat(IChatStatusService chatStatus) : IDisposable
{
    private List<Message> messages = chatStatus.GetMessages();
    private ChatState chatState = chatStatus.ChatState;

    public void Dispose()
    {
        chatStatus.OnChatUpdate -= OnMessage;
        chatStatus.OnStateChanged -= OnStateChanged;
    }

    protected override void OnInitialized()
    {
        chatStatus.OnChatUpdate += OnMessage;
        chatStatus.OnStateChanged += OnStateChanged;
    }

    private void OnMessage()
    {
        messages = chatStatus.GetMessages();
        InvokeAsync(StateHasChanged);
    }

    private void OnStateChanged(object? _, ChatState chatState)
    {
        this.chatState = chatState;
        InvokeAsync(StateHasChanged);
    }
}