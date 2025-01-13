using System.IO;
using OnionArchitectureTemplateConsole;

public static class ApplicationLayerCreator
{
    public static void CreateApplicationLayer(string solutionName, bool isWindows)
    {
        string applicationPath = Path.Combine(Directory.GetCurrentDirectory(), $"{solutionName}.Application", "Features", "Products");
        Directory.CreateDirectory(Path.Combine(applicationPath, "Commands"));
        Directory.CreateDirectory(Path.Combine(applicationPath, "Queries"));
        Directory.CreateDirectory(Path.Combine(applicationPath, "DTOs"));

        // DTO: ProductDto.cs
        File.WriteAllText(Path.Combine(applicationPath, "DTOs", "ProductDto.cs"),
            @$"namespace {solutionName}.Application.Features.Products.DTOs
{{
    public class ProductDto
    {{
        public int Id {{ get; set; }}
        public string Name {{ get; set; }}
        public decimal Price {{ get; set; }}
    }}
}}");

        // CreateProductCommand.cs
        File.WriteAllText(Path.Combine(applicationPath, "Commands", "CreateProductCommand.cs"),
            @$"using MediatR;
using {solutionName}.Shared.Responses;

namespace {solutionName}.Application.Features.Products.Commands
{{
    public class CreateProductCommand : IRequest<Response<int>>
    {{
        public string Name {{ get; set; }}
        public decimal Price {{ get; set; }}
    }}
}}");

        // CreateProductCommandHandler.cs
        File.WriteAllText(Path.Combine(applicationPath, "Commands", "CreateProductCommandHandler.cs"),
            @$"using MediatR;
using {solutionName}.Domain.Entities;
using {solutionName}.Shared.Interfaces;
using {solutionName}.Shared.Responses;

namespace {solutionName}.Application.Features.Products.Commands
{{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Response<int>>
    {{
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        {{
            _unitOfWork = unitOfWork;
        }}

        public async Task<Response<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {{
            var product = new Product
            {{
                Name = request.Name,
                Price = request.Price
            }};

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return new Response<int>(product.Id, true, 201, ""Product created successfully"");
        }}
    }}
}}");

        // GetProductQuery.cs
        File.WriteAllText(Path.Combine(applicationPath, "Queries", "GetProductQuery.cs"),
            @$"using MediatR;
using {solutionName}.Shared.Responses;
using {solutionName}.Application.Features.Products.DTOs;

namespace {solutionName}.Application.Features.Products.Queries
{{
    public class GetProductQuery : IRequest<Response<ProductDto>>
    {{
        public int Id {{ get; set; }}
    }}
}}");

        // GetProductQueryHandler.cs
        File.WriteAllText(Path.Combine(applicationPath, "Queries", "GetProductQueryHandler.cs"),
            @$"using MediatR;
using {solutionName}.Domain.Entities;
using {solutionName}.Shared.Interfaces;
using {solutionName}.Shared.Responses;
using {solutionName}.Application.Features.Products.DTOs;

namespace {solutionName}.Application.Features.Products.Queries
{{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Response<ProductDto>>
    {{
        private readonly IUnitOfWork _unitOfWork;

        public GetProductQueryHandler(IUnitOfWork unitOfWork)
        {{
            _unitOfWork = unitOfWork;
        }}

        public async Task<Response<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {{
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);

            if (product == null)
                return new Response<ProductDto>(null, false, 404, ""Product not found"");

            var productDto = new ProductDto
            {{
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            }};

            return new Response<ProductDto>(productDto, true, 200, ""Product retrieved successfully"");
        }}
    }}
}}");

        // GetProductsQuery.cs
        File.WriteAllText(Path.Combine(applicationPath, "Queries", "GetProductsQuery.cs"),
            @$"using MediatR;
using {solutionName}.Shared.Responses;
using System.Collections.Generic;
using {solutionName}.Application.Features.Products.DTOs;

namespace {solutionName}.Application.Features.Products.Queries
{{
    public class GetProductsQuery : IRequest<Response<List<ProductDto>>>
    {{
    }}
}}");

        // GetProductsQueryHandler.cs
        File.WriteAllText(Path.Combine(applicationPath, "Queries", "GetProductsQueryHandler.cs"),
            @$"using MediatR;
using System.Collections.Generic;
using System.Linq;
using {solutionName}.Domain.Entities;
using {solutionName}.Shared.Interfaces;
using {solutionName}.Shared.Responses;
using {solutionName}.Application.Features.Products.DTOs;

namespace {solutionName}.Application.Features.Products.Queries
{{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Response<List<ProductDto>>>
    {{
        private readonly IUnitOfWork _unitOfWork;

        public GetProductsQueryHandler(IUnitOfWork unitOfWork)
        {{
            _unitOfWork = unitOfWork;
        }}

        public async Task<Response<List<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {{
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();

            var productDtos = products.Select(product => new ProductDto
            {{
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            }}).ToList();

            return new Response<List<ProductDto>>(productDtos, true, 200, ""Products retrieved successfully"");
        }}
    }}
}}");

        // MediatR NuGet Paketlerini Ekle
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj package MediatR", isWindows);
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj package MediatR.Extensions.Microsoft.DependencyInjection", isWindows);

        // **Domain** ve **Shared** referanslarını ekle
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj reference {solutionName}.Domain/{solutionName}.Domain.csproj", isWindows);
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj reference {solutionName}.Shared/{solutionName}.Shared.csproj", isWindows);
    }
}