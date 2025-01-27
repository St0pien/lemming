using System.Text.RegularExpressions;
using Microsoft.Playwright;
using OllamaSharp.Models.Chat;

namespace WebApp.Tools;

public class PlaywrightBrowserTools(IBrowserContext browser) : IBrowserTools
{
    public string ExecuteTool(Message.Function function)
    {
        return function.Name switch
        {
            "google_it" => GoogleIt(function.Arguments).Result,
            _ => "Tool not found"
        };
    }

    public List<Tool> GetTools()
    {
        return [new GoogleItTool()];
    }

    public static async Task<PlaywrightBrowserTools> Launch()
    {
        var playwright = await Playwright.CreateAsync();
        var chromium = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Headless = false
        });
        var ctx = await chromium.NewContextAsync(new()
        {
            Locale = "en-GB"
        });
        return new PlaywrightBrowserTools(ctx);
    }

    private IPage? page = null;

    private async Task<string> GoogleIt(IDictionary<string, object?>? args)
    {
        if (page == null || page.IsClosed)
        {
            page = await browser.NewPageAsync();
        }

        var search = args?["search"]?.ToString();

        page.GotoAsync("https://duckduckgo.com/").Wait();

        page.GetByRole(AriaRole.Combobox).FocusAsync().Wait();
        page.Keyboard.TypeAsync(search ?? "", new KeyboardTypeOptions() { Delay = 50 }).Wait();
        page.Keyboard.PressAsync("Enter").Wait();
        page.WaitForURLAsync(new Regex(".*q=.*")).Wait();
        var allResults = page.Locator(".react-results--main li").AllAsync().Result;
        var allLinks = allResults.SelectMany(li => li.Locator("a").AllAsync().Result.Where(a => a.GetAttributeAsync("href")?.Result?.StartsWith("http") ?? false));
        var url = allLinks.First().GetAttributeAsync("href").Result;
        allLinks.First().ClickAsync().Wait();
        Console.WriteLine($"[Browser] Grabbing content from url: {url}");
        page.WaitForLoadStateAsync().Wait();

        string? result = "";
        foreach (var l in new List<ILocator>([page.GetByRole(AriaRole.Main), page.Locator("main"), page.Locator(".main"), page.Locator("body")]))
        {
            try
            {
                result = await l.TextContentAsync(new()
                {
                    Timeout = 2000
                });
                break;
            }
            catch (Exception)
            {

            }
        }

        if (result?.Length > 20_000)
        {
            result = result.Substring(0, 20000);
        }

        return $"Useful external resources for reference:\n\n\n{result}\n\n\n------\n";
    }
}