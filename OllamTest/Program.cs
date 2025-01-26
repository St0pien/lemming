// See https://aka.ms/new-console-template for more information

using OllamaSharp;

var ollamaClient = new OllamaApiClient(new Uri("http://localhost:11434/"));
ollamaClient.SelectedModel = "llama3.2";

Console.WriteLine(ollamaClient.ListLocalModelsAsync().Result);

var chat = new Chat(ollamaClient, "You are a helpful ai browser automation bot");
var cnc = new CancellationTokenSource();
var answer = chat.SendAsync("Hi there!", cnc.Token);


Thread.Sleep(1000);
cnc.Cancel();

Console.WriteLine("yea am blocked here");
while (true)
{
    foreach (var x in answer.ToBlockingEnumerable())
    {
        Console.Write(x);
    }
    Console.WriteLine("");

    var line = Console.ReadLine() ?? "";
    Console.WriteLine(line);
    answer = chat.SendAsync(line);
}