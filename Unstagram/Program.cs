using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System.Diagnostics;
using Unstagram.Helper;

ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddJsonFile("appsettings.json");
IConfigurationRoot configuration = configurationBuilder.Build();

AnsiConsole.Write(new FigletText("Welcome to \n Unstagram!"));
Console.WriteLine("Step 1: Copy your instagram 'followers_1.json' and 'follower.json' to this folder.");
Console.WriteLine("Step 2: Check file names in Appsettings.json");
Console.WriteLine("Step 3: Press Enter key to start process.");

ReadAndGenerateResults(configuration);



static void ReadAndGenerateResults(IConfigurationRoot configuration)
{
    do
    {
        ConsoleKeyInfo keyResult = Console.ReadKey();
        if (keyResult.Key == ConsoleKey.Enter)
        {
            AnsiConsole.Write(new FigletText("*-Processing-*"));
            string? followingFileName = configuration.GetValue<string>("FollowingFileName");
            string? followerFileName = configuration.GetValue<string>("FollowerFileName");

            bool result = InstagramCalculator.GenerateInformation(followingFileName, followerFileName);
            if (result)
            {
                for (int i = 1; i <= 10; i++)
                {
                    Console.WriteLine($"%{i * 10}");
                    Thread.Sleep(100);
                }
                AnsiConsole.Write(new FigletText("*-Done-*"));
                DoExitProcess();
            }
            else
            {
                AnsiConsole.Write(new FigletText("*-Error-*"));
                Console.WriteLine("Press any button to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            break;
        }
        else if (keyResult.Key == ConsoleKey.Escape)
        {
            Environment.Exit(0);
        }
        ;
    } while (true);
}

static void DoExitProcess()
{
    Console.WriteLine("Do you want to open result files? (Y/N)");
    var key = Console.ReadLine();
    key = !string.IsNullOrWhiteSpace(key) ? key.ToLower() : string.Empty;
    if (key.Equals("y"))
    {
        var path = $"{AppDomain.CurrentDomain.BaseDirectory}Results";
        if (Directory.Exists(path))
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = path
            });
            Environment.Exit(0);
        }
        else
        {
            Console.WriteLine("Directory could not found.");
            Environment.Exit(0);
        }
    }
    else
    {
        Console.WriteLine("Press any button to exit.");
        Console.ReadKey();
        Environment.Exit(0);
    }
}