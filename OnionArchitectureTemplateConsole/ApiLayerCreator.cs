using System.IO;
using OnionArchitectureTemplateConsole;

public static class ApiLayerCreator
{
    public static void CreateApiLayer(string solutionName, bool isWindows)
    {
        // Create Web API project
        RunCommand($"dotnet new webapi -n {solutionName}.API", isWindows);
        RunCommand($"dotnet sln {solutionName}.sln add {solutionName}.API/{solutionName}.API.csproj", isWindows);

        // Add dependencies
        AddDependencies(solutionName, isWindows);

        // Create Controllers directory and add ProductController
        CreateControllers(solutionName);

        // Configure Program.cs
        ConfigureProgramCs(solutionName);
    }

    private static void AddDependencies(string solutionName, bool isWindows)
    {
        // Add required NuGet packages
        var packages = new[]
        {
            "Microsoft.AspNetCore.Mvc.NewtonsoftJson",
            "Swashbuckle.AspNetCore",
            "MediatR",
            "MediatR.Extensions.Microsoft.DependencyInjection",
            "AutoMapper",
            "AutoMapper.Extensions.Microsoft.DependencyInjection",
            "FluentValidation",
            "FluentValidation.DependencyInjectionExtensions"
        };

        foreach (var package in packages)
        {
            RunCommand($"dotnet add {solutionName}.API/{solutionName}.API.csproj package {package}", isWindows);
        }

        // Add project references
        var references = new[]
        {
            $"{solutionName}.Application/{solutionName}.Application.csproj",
            $"{solutionName}.Shared/{solutionName}.Shared.csproj"
        };

        foreach (var reference in references)
        {
            RunCommand($"dotnet add {solutionName}.API/{solutionName}.API.csproj reference {reference}", isWindows);
        }
    }

    private static void CreateControllers(string solutionName)
    {
        var controllersPath = Path.Combine(Directory.GetCurrentDirectory(), $"{solutionName}.API", "Controllers");
        Directory.CreateDirectory(controllersPath);

        var productControllerPath = Path.Combine(controllersPath, "ProductController.cs");
        File.WriteAllText(productControllerPath, GenerateProductControllerCode(solutionName));
    }

    private static string GenerateProductControllerCode(string solutionName)
    {
        return @$"using Microsoft.AspNetCore.Mvc;
using MediatR;
using {solutionName}.Application.Features.Products.Commands;
using {solutionName}.Application.Features.Products.Queries;
using {solutionName}.Shared.Responses;

namespace {solutionName}.API.Controllers
{{
    [ApiController]
    [Route(""api/[controller]"")]
    public class ProductController : ControllerBase
    {{
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {{
            _mediator = mediator;
        }}

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {{
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetProductById), new {{ id = result.Data }}, result);
        }}

        [HttpGet(""{{id}}"")]
        public async Task<IActionResult> GetProductById(int id)
        {{
            var query = new GetProductQuery {{ Id = id }};
            var result = await _mediator.Send(query);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }}
    }}
}}";
    }

    private static void ConfigureProgramCs(string solutionName)
    {
        var programPath = Path.Combine(Directory.GetCurrentDirectory(), $"{solutionName}.API", "Program.cs");

        File.WriteAllText(programPath, @$"using {solutionName}.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Application services
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{{
    app.UseSwagger();
    app.UseSwaggerUI();
}}

//app.UseAuthorization();
app.MapControllers();
app.Run();");
    }

    private static void RunCommand(string command, bool isWindows)
    {
        SolutionCreator.RunCommand(command, isWindows);
    }
}