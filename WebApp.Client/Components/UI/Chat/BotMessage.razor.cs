using Microsoft.AspNetCore.Components;

namespace WebApp.Client.Components.UI.Chat;

public partial class BotMessage
{
    [Parameter]
    public string Message { get; set; } = "";
}