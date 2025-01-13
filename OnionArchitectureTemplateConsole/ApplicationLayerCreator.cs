namespace OnionArchitectureTemplateConsole;

public static class ApplicationLayerCreator
{
    public static void CreateApplicationLayer(string solutionName, bool isWindows)
    {
        string applicationPath = Path.Combine(Directory.GetCurrentDirectory(), $"{solutionName}.Application", "Features", "Products");
        Directory.CreateDirectory(Path.Combine(applicationPath, "Commands"));
        Directory.CreateDirectory(Path.Combine(applicationPath, "Queries"));
        Directory.CreateDirectory(Path.Combine(applicationPath, "DTOs"));
        Directory.CreateDirectory(Path.Combine(applicationPath, "Mappings"));
        Directory.CreateDirectory(Path.Combine(applicationPath, "Validators"));
        Directory.CreateDirectory(Path.Combine(applicationPath, "Tests"));

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

        // Command: CreateProductCommand.cs
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

        // Command Handler: CreateProductCommandHandler.cs
        File.WriteAllText(Path.Combine(applicationPath, "Commands", "CreateProductCommandHandler.cs"),
            @$"using MediatR;
using AutoMapper;
using FluentValidation;
using {solutionName}.Domain.Entities;
using {solutionName}.Shared.Interfaces;
using {solutionName}.Shared.Responses;

namespace {solutionName}.Application.Features.Products.Commands
{{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Response<int>>
    {{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductCommand> _validator;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateProductCommand> validator)
        {{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }}

        public async Task<Response<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {{
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {{
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return new Response<int>(errors, 400, ""Validation failed"");
            }}

            var product = _mapper.Map<Product>(request);

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return new Response<int>(product.Id, true, 201, ""Product created successfully"");
        }}
    }}
}}");

        // Query: GetProductQuery.cs
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

        // Query Handler: GetProductQueryHandler.cs
        File.WriteAllText(Path.Combine(applicationPath, "Queries", "GetProductQueryHandler.cs"),
            @$"using MediatR;
using AutoMapper;
using {solutionName}.Domain.Entities;
using {solutionName}.Shared.Interfaces;
using {solutionName}.Shared.Responses;
using {solutionName}.Application.Features.Products.DTOs;

namespace {solutionName}.Application.Features.Products.Queries
{{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Response<ProductDto>>
    {{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }}

        public async Task<Response<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {{
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);

            if (product == null)
                return new Response<ProductDto>(null, false, 404, ""Product not found"");

            var productDto = _mapper.Map<ProductDto>(product);

            return new Response<ProductDto>(productDto, true, 200, ""Product retrieved successfully"");
        }}
    }}
}}");

        // FluentValidation Validator: ProductValidator.cs
        File.WriteAllText(Path.Combine(applicationPath, "Validators", "ProductValidator.cs"),
            @$"using FluentValidation;

namespace {solutionName}.Application.Features.Products.Validators
{{
    public class ProductValidator : AbstractValidator<{solutionName}.Application.Features.Products.DTOs.ProductDto>
    {{
        public ProductValidator()
        {{
            RuleFor(product => product.Name)
                .NotEmpty().WithMessage(""Product name cannot be empty"")
                .MaximumLength(250).WithMessage(""Product name cannot exceed 250 characters"");

            RuleFor(product => product.Price)
                .GreaterThan(0).WithMessage(""Product price must be greater than zero"");
        }}
    }}
}}");

        // Unit Test: CreateProductCommandHandlerTests.cs
        File.WriteAllText(Path.Combine(applicationPath, "Tests", "CreateProductCommandHandlerTests.cs"),
            @$"using Xunit;
using Moq;
using FluentValidation;
using FluentValidation.Results;
using AutoMapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using {solutionName}.Domain.Entities;
using {solutionName}.Shared.Interfaces;
using {solutionName}.Shared.Responses;
using {solutionName}.Application.Features.Products.Commands;

namespace {solutionName}.Application.Tests.Features.Products.Commands
{{
    public class CreateProductCommandHandlerTests
    {{
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<CreateProductCommand>> _validatorMock;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {{
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<CreateProductCommand>>();
            _handler = new CreateProductCommandHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _validatorMock.Object
            );
        }}

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResponse()
        {{
            // Arrange
            var command = new CreateProductCommand
            {{
                Name = ""Test Product"",
                Price = 100
            }};

            var product = new Product
            {{
                Id = 1,
                Name = command.Name,
                Price = command.Price
            }};

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mapperMock
                .Setup(m => m.Map<Product>(command))
                .Returns(product);

            _unitOfWorkMock
                .Setup(u => u.Repository<Product>().AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal(""Product created successfully"", result.Message);
            Assert.Equal(product.Id, result.Data);
        }}
    }}
}}");

        // Gerekli NuGet Paketlerini Ekle
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj package FluentValidation", isWindows);
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj package FluentValidation.DependencyInjectionExtensions", isWindows);
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj package AutoMapper", isWindows);
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj package MediatR", isWindows);
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj package Moq", isWindows);
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj package xunit", isWindows);

        // **Domain** ve **Shared** referanslarını ekle
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj reference {solutionName}.Domain/{solutionName}.Domain.csproj", isWindows);
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Application/{solutionName}.Application.csproj reference {solutionName}.Shared/{solutionName}.Shared.csproj", isWindows);
    }
}