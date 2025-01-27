using OllamaSharp.Models.Chat;

namespace WebApp.Tools;


class ClickOnItTool : Tool
{
    public ClickOnItTool()
    {
        Type = "function";
        Function = new Function
        {
            Name = "click_on_it",
            Description = "Open the link with specified text content",
            Parameters = new Parameters
            {
                Properties = new Dictionary<string, Property>
                {
                    ["text"] = new()
                    {
                        Type = "string",
                        Description = "text content of a linkk you want to open"
                    }
                },
                Required = ["text"]
            }
        };
    }
}