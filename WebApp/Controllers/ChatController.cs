using Microsoft.AspNetCore.Mvc;
using Model.Payloads;

namespace WebApp.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    [HttpPost]
    public void PostMessage([FromBody] MessagePayload payload)
    {
        Console.WriteLine(payload.message);
    }
}
