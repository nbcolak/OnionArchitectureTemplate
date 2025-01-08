namespace OnionArchitectureTemplateConsole;

using System.IO;

public static class InfrastructureLayerCreator
{
    public static void CreateInfrastructureLayer(string solutionName, bool isWindows)
    {
        string infrastructurePath = Path.Combine(Directory.GetCurrentDirectory(), $"{solutionName}.Infrastructure", "Persistence");
        Directory.CreateDirectory(infrastructurePath);

        // AppDbContext oluşturuluyor
        File.WriteAllText(Path.Combine(infrastructurePath, "AppDbContext.cs"),
            @$"using Microsoft.EntityFrameworkCore;
using {solutionName}.Domain.Entities;

namespace {solutionName}.Infrastructure.Persistence
{{
    public class AppDbContext : DbContext
    {{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {{ }}

        public DbSet<Product> Products {{ get; set; }}
    }}
}}");

        // Entity Framework 
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Infrastructure/{solutionName}.Infrastructure.csproj package Microsoft.EntityFrameworkCore", isWindows);
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Infrastructure/{solutionName}.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer", isWindows);

        // **Domain** 
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Infrastructure/{solutionName}.Infrastructure.csproj reference {solutionName}.Domain/{solutionName}.Domain.csproj", isWindows);
    }
}