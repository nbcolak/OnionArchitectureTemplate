namespace OnionArchitectureTemplateConsole;

using System.IO;

public static class ApplicationLayerCreator
{
    public static void CreateApplicationLayer(string solutionName, bool isWindows)
    {
        string applicationPath = Path.Combine(Directory.GetCurrentDirectory(), $"{solutionName}.Application", "Features", "Products");
        Directory.CreateDirectory(Path.Combine(applicationPath, "Commands"));
        Directory.CreateDirectory(Path.Combine(applicationPath, "Queries"));

        // CQRS Komut ve Sorgular
        File.WriteAllText(Path.Combine(applicationPath, "Commands", "CreateProductCommand.cs"),
            @$"using MediatR;

namespace {solutionName}.Application.Features.Products.Commands
{{
    public class CreateProductCommand : IRequest<int>
    {{
        public string Name {{ get; set; }}
        public decimal Price {{ get; set; }}
    }}
}}");

        File.WriteAllText(Path.Combine(applicationPath, "Commands", "UpdateProductCommand.cs"),
            @$"using MediatR;

namespace {solutionName}.Application.Features.Products.Commands
{{
    public class UpdateProductCommand : IRequest
    {{
        public int Id {{ get; set; }}
        public string Name {{ get; set; }}
        public decimal Price {{ get; set; }}
    }}
}}");

        File.WriteAllText(Path.Combine(applicationPath, "Commands", "DeleteProductCommand.cs"),
            @$"using MediatR;

namespace {solutionName}.Application.Features.Products.Commands
{{
    public class DeleteProductCommand : IRequest
    {{
        public int Id {{ get; set; }}
    }}
}}");

        File.WriteAllText(Path.Combine(applicationPath, "Queries", "GetProductQuery.cs"),
            @$"using MediatR;
using {solutionName}.Domain.Entities;

namespace {solutionName}.Application.Features.Products.Queries
{{
    public class GetProductQuery : IRequest<Product>
    {{
        public int Id {{ get; set; }}
    }}
}}");

        File.WriteAllText(Path.Combine(applicationPath, "Queries", "GetProductsQuery.cs"),
            @$"using MediatR;
using System.Collections.Generic;
using {solutionName}.Domain.Entities;

namespace {solutionName}.Application.Features.Products.Queries
{{
    public class GetProductsQuery : IRequest<List<Product>>
    {{
    }}
}}");

        // MediatR NuGet Paketleri Ekle
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj package MediatR", isWindows);
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj package MediatR.Extensions.Microsoft.DependencyInjection", isWindows);

        // **Domain** referansını ekle
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj reference {solutionName}.Domain/{solutionName}.Domain.csproj", isWindows);
    }
}