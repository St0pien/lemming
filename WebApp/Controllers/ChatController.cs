using Microsoft.AspNetCore.Mvc;
using WebApp.Client.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController(IChatService chatService) : ControllerBase
{
    [HttpPost]
    public void PostMessage([FromBody] MessagePaylod payload)
    {
        chatService.SendUserMessage(payload.Message);
    }
}
