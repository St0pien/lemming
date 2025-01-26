using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WebApp.Client.Services;

namespace WebApp.Client.Components.UI.Forms;

public partial class UserMessageForm(IChatService chatService)
{
    private IChatService chatService = chatService;
    private string text = "";
    private string err = "";
    private bool enterPressed = false;
    private ElementReference userMessageInput;

    private void OnSubmit()
    {
        chatService.SendUserMessage(text);
        text = "";
    }

    private void ResetChat()
    {
        chatService.StartNewChat();
    }

    private void OnKeyPress(KeyboardEventArgs eventArgs)
    {
        enterPressed = false;
        if (eventArgs.Code == "Enter" && !eventArgs.ShiftKey)
        {
            enterPressed = true;
            OnSubmit();
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        userMessageInput.FocusAsync();
    }
}