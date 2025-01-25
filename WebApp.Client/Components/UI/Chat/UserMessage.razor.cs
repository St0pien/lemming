using Microsoft.AspNetCore.Components;

namespace WebApp.Client.Components.UI.Chat;

public partial class UserMessage
{
    [Parameter]
    public string Message { get; set; } = "";
}