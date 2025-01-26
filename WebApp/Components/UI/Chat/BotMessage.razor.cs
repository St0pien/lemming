using Microsoft.AspNetCore.Components;

namespace WebApp.Components.UI.Chat;

public partial class BotMessage
{
    [Parameter]
    public string Message { get; set; } = "";
}