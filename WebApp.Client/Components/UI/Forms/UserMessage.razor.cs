using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Model.Payloads;

namespace WebApp.Client.Components.UI.Forms;

public partial class UserMessage(HttpClient httpClient)
{
    private HttpClient httpClient = httpClient;
    private string text = "";
    private string err = "";
    private bool enterPressed = false;
    private ElementReference userMessageInput;

    private void OnSubmit()
    {
        Console.WriteLine($"Submitted with {text}");
        httpClient.PostAsJsonAsync("api/chat", new MessagePayload
        {
            message = text
        });

        text = "";
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
        base.OnAfterRender(firstRender);
        userMessageInput.FocusAsync();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Console.WriteLine(OperatingSystem.IsBrowser());
    }
}