using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.CommandLine;
using System.Threading;

namespace netwise_task;

class Program
{
    //Create httpClient instance, static per recommended documentation
    private static readonly HttpClient httpClient = new()
    {
        BaseAddress = new Uri("https://catfact.ninja"),
    };
    //Successful command parsing action
    static async Task<int> FetchFact(FileInfo file, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync("fact",cancellationToken);
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
        if(!jsonResponse.EndsWith('\n'))
        {
            jsonResponse = $"{jsonResponse}\n";
        }
        File.AppendAllText(file.FullName,jsonResponse);
        return 0;
    }

    static async Task<int> Main(string[] args)
    {
        RootCommand rootCommand = new("Utility for getting random cat facts from catfact.ninja");
        Option<FileInfo> fileOption = new("--output", "-o")
        {
            Description = "File to write fact into",
            Required = true
        };
        rootCommand.Options.Add(fileOption);
        //Handle action of root command
        rootCommand.SetAction(async (parseResult,cancellationToken)=>
        {
            FileInfo? file = parseResult.GetValue(fileOption);
            if (file is null)
            {
                return 10;
            }
            return await FetchFact(file,cancellationToken);

        });

        //Parse arguments and run program
        ParseResult parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}
