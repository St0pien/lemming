using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiUri"] ?? "http://localhost:5115/api/")
});

builder.Services.AddScoped<IChatService, ClientChatService>();

await builder.Build().RunAsync();
