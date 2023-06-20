using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Unstagram.Helper;

ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddJsonFile("appsettings.json");
IConfigurationRoot configuration = configurationBuilder.Build();

AnsiConsole.Write(new FigletText("Welcome to \n Unstagram!"));
Console.WriteLine("Step 1: Copy your instagram 'followers_1.json' and 'follower.json' to this folder.");
Console.WriteLine("Step 2: Check file names in Appsettings.json");
Console.WriteLine("Step 3: Press Enter key to start process.");
do
{
    ConsoleKeyInfo keyResult = Console.ReadKey();
    if (keyResult.Key == ConsoleKey.Enter)
    {
        string? followingFileName = configuration.GetValue<string>("FollowingFileName");
        string? followerFileName = configuration.GetValue<string>("FollowerFileName");

        InstagramCalculator.GenerateInformation(followingFileName, followerFileName);
        break;
    }
    else if (keyResult.Key == ConsoleKey.Escape)
    {
        Environment.Exit(0);
    }
    ;
} while (true);
AnsiConsole.Write(new FigletText("*-Processing-*"));
for (int i = 1; i <= 10; i++)
{
    Console.WriteLine($"%{i*10}");
    Thread.Sleep(100);
}
AnsiConsole.Write(new FigletText("Done"));
Console.WriteLine("Press any button to exit.");
Console.ReadKey();
Environment.Exit(0);


