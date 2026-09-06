using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace netwise_task;

class Program
{
    private static readonly HttpClient httpClient = new()
    {
        BaseAddress = new Uri("https://catfact.ninja"),
    };
    private static async Task Run()
    {
        using HttpResponseMessage response = await httpClient.GetAsync("fact");
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"{jsonResponse}\n");
        if(!jsonResponse.EndsWith('\n'))
        {
            jsonResponse = $"{jsonResponse}\n";
        }
        File.AppendAllText("catfact.txt",jsonResponse);
    }

    static void Main(string[] args)
    {
        Console.WriteLine("If you can see this, the app is incomplete");
        Run().Wait();
        
    }
}
