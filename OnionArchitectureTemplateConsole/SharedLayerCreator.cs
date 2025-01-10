namespace OnionArchitectureTemplateConsole;

public static class SharedLayerCreator
{
    public static void CreateSharedLayer(string solutionName)
    {
        string sharedPath = Path.Combine(Directory.GetCurrentDirectory(), $"{solutionName}.Shared");
        string responsesPath = Path.Combine(sharedPath, "Responses");
        string interfacesPath = Path.Combine(sharedPath, "Interfaces");

        // Klasörleri oluştur
        Directory.CreateDirectory(responsesPath);
        Directory.CreateDirectory(interfacesPath);

        // Response<T> sınıfını oluştur
        File.WriteAllText(Path.Combine(responsesPath, "Response.cs"),
            @$"namespace {solutionName}.Shared.Responses
{{
    public class Response<T>
    {{
        public bool Success {{ get; set; }}
        public T Data {{ get; set; }}
        public List<string> Errors {{ get; set; }} = new List<string>();
        public int StatusCode {{ get; set; }}
        public string Message {{ get; set; }}

        public Response(T data, bool success, int statusCode, string message = null)
        {{
            Data = data;
            Success = success;
            StatusCode = statusCode;
            Message = message;
        }}

        public Response(List<string> errors, bool success, int statusCode, string message = null)
        {{
            Errors = errors;
            Success = success;
            StatusCode = statusCode;
            Message = message;
        }}
    }}
}}");

        // IRepository<T> interface'ini oluştur
        File.WriteAllText(Path.Combine(interfacesPath, "IRepository.cs"),
            @$"using System.Collections.Generic;
using System.Threading.Tasks;

namespace {solutionName}.Shared.Interfaces
{{
    public interface IRepository<T> where T : class
    {{
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }}
}}");

        // IUnitOfWork interface'ini oluştur
        File.WriteAllText(Path.Combine(interfacesPath, "IUnitOfWork.cs"),
            @$"using System;
using System.Threading.Tasks;

namespace {solutionName}.Shared.Interfaces
{{
    public interface IUnitOfWork : IDisposable
    {{
        IRepository<T> Repository<T>() where T : class;
        Task SaveChangesAsync();
    }}
}}");
    }
}