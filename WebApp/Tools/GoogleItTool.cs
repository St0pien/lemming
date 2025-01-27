using OllamaSharp.Models.Chat;

namespace WebApp.Tools;

class GoogleItTool : Tool
{
    public GoogleItTool()
    {
        Type = "function";
        Function = new Function
        {
            Name = "google_it",
            Description = "Search the web for new relevant information",
            Parameters = new Parameters
            {
                Properties = new Dictionary<string, Property>
                {
                    ["search"] = new()
                    {
                        Type = "string",
                        Description = "Search phrase"
                    }
                },
                Required = ["search"]
            }
        };
    }
}