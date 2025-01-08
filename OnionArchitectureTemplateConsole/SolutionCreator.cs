using System.Diagnostics;

namespace OnionArchitectureTemplateConsole;

public static class SolutionCreator
{
    public static void CreateSolution(string solutionName, bool isWindows)
    {
        RunCommand($"dotnet new sln -n {solutionName}", isWindows);

        string[] projects = { "Domain", "Application", "Infrastructure", "Shared" };
        foreach (var project in projects)
        {
            RunCommand($"dotnet new classlib -n {solutionName}.{project}", isWindows);
            RunCommand($"dotnet sln {solutionName}.sln add {solutionName}.{project}/{solutionName}.{project}.csproj", isWindows);
        }
    }

    public static void RunCommand(string command, bool isWindows)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = isWindows ? "cmd.exe" : "/bin/bash",
                Arguments = isWindows ? $"/c {command}" : $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        Console.WriteLine(process.StandardOutput.ReadToEnd());
        Console.WriteLine(process.StandardError.ReadToEnd());
        process.WaitForExit();
    }
}