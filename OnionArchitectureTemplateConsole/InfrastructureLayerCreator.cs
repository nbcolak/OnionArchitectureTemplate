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

        // Repository sınıfı oluşturuluyor
        File.WriteAllText(Path.Combine(infrastructurePath, "Repository.cs"),
            @$"using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using {solutionName}.Shared.Interfaces;

namespace {solutionName}.Infrastructure.Persistence
{{
    public class Repository<T> : IRepository<T> where T : class
    {{
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(DbContext context)
        {{
            _context = context;
            _dbSet = _context.Set<T>();
        }}

        public async Task<T> GetByIdAsync(int id)
        {{
            return await _dbSet.FindAsync(id);
        }}

        public async Task<IEnumerable<T>> GetAllAsync()
        {{
            return await _dbSet.ToListAsync();
        }}

        public async Task AddAsync(T entity)
        {{
            await _dbSet.AddAsync(entity);
        }}

        public async Task UpdateAsync(T entity)
        {{
            _dbSet.Update(entity);
        }}

        public async Task DeleteAsync(T entity)
        {{
            _dbSet.Remove(entity);
        }}
    }}
}}");

        // UnitOfWork sınıfı oluşturuluyor
        File.WriteAllText(Path.Combine(infrastructurePath, "UnitOfWork.cs"),
            @$"using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using {solutionName}.Shared.Interfaces;

namespace {solutionName}.Infrastructure.Persistence
{{
    public class UnitOfWork : IUnitOfWork
    {{
        private readonly DbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(DbContext context)
        {{
            _context = context;
        }}

        public IRepository<T> Repository<T>() where T : class
        {{
            if (!_repositories.ContainsKey(typeof(T)))
            {{
                var repository = new Repository<T>(_context);
                _repositories.Add(typeof(T), repository);
            }}
            return (IRepository<T>)_repositories[typeof(T)];
        }}

        public async Task SaveChangesAsync()
        {{
            await _context.SaveChangesAsync();
        }}

        public void Dispose()
        {{
            _context.Dispose();
        }}
    }}
}}");

        // Entity Framework paketlerini ekle
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Infrastructure/{solutionName}.Infrastructure.csproj package Microsoft.EntityFrameworkCore", isWindows);
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Infrastructure/{solutionName}.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer", isWindows);

        // **Domain** referansını ekle
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Infrastructure/{solutionName}.Infrastructure.csproj reference {solutionName}.Domain/{solutionName}.Domain.csproj", isWindows);

        // **Shared** referansını ekle
        SolutionCreator.RunCommand($"dotnet add {solutionName}.Infrastructure/{solutionName}.Infrastructure.csproj reference {solutionName}.Shared/{solutionName}.Shared.csproj", isWindows);
    }
}