namespace OnionArchitectureTemplateConsole;
using System.IO;

public static class SharedLayerCreator
{
    public static void CreateSharedLayer(string solutionName)
    {
        string sharedPath = Path.Combine(Directory.GetCurrentDirectory(), $"{solutionName}.Shared", "Responses");
        Directory.CreateDirectory(sharedPath);

        // Response<T> sınıfını oluştur
        File.WriteAllText(Path.Combine(sharedPath, "Response.cs"),
            @$"namespace {solutionName}.Shared.Responses
{{
    public class Response<T>
    {{
        /// <summary>
        /// İşlem sonucunun başarılı olup olmadığını belirtir.
        /// </summary>
        public bool Success {{ get; set; }}

        /// <summary>
        /// İşlem sonucu dönen veri (varsa).
        /// </summary>
        public T Data {{ get; set; }}

        /// <summary>
        /// İşlemle ilgili hata mesajları (varsa).
        /// </summary>
        public List<string> Errors {{ get; set; }} = new List<string>();

        /// <summary>
        /// HTTP durum kodu veya özel bir durum kodu.
        /// </summary>
        public int StatusCode {{ get; set; }}

        /// <summary>
        /// Genel bir mesaj (başarı veya hata için kullanılabilir).
        /// </summary>
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
    }
}