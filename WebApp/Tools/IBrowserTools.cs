using OllamaSharp.Models.Chat;

namespace WebApp.Tools;

public interface IBrowserTools
{
    public List<Tool> GetTools();

    public string ExecuteTool(Message.Function function);
}