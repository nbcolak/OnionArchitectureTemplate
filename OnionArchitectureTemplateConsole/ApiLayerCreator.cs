namespace OnionArchitectureTemplateConsole;

public static class ApiLayerCreator
{
    public static void CreateApiLayer(string solutionName, bool isWindows)
    {
        RunCommand($"dotnet new webapi -n {solutionName}.API", isWindows);
        RunCommand($"dotnet sln {solutionName}.sln add {solutionName}.API/{solutionName}.API.csproj", isWindows);
        RunCommand($"dotnet add {solutionName}.API/{solutionName}.API.csproj package Microsoft.AspNetCore.Mvc.NewtonsoftJson", isWindows);
    }

    private static void RunCommand(string command, bool isWindows)
    {
        SolutionCreator.RunCommand(command, isWindows);
    }
}