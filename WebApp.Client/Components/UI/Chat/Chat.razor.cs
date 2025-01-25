using Microsoft.AspNetCore.Components;
using Model;

namespace WebApp.Client.Components.UI.Chat;

public partial class Chat
{
    [Parameter]
    public List<Message> Messages { get; set; } = [];
}