using Model;
using WebApp.Services;

namespace WebApp.Components.Pages;

public partial class Home(IMessageRelayService messageRelay) : IDisposable
{
    private List<Message> messages = messageRelay.GetMessages();

    public void Dispose()
    {
        messageRelay.OnMessage -= OnMessage;
        Console.WriteLine("DeSubscribing shit");
    }

    protected override void OnInitialized()
    {
        messageRelay.OnMessage += OnMessage;
        Console.WriteLine("Subscribing shit");
    }

    private void OnMessage(object? _, Message __)
    {
        StateHasChanged();
    }
}