namespace OnionArchitectureTemplateConsole;

public static class DomainLayerCreator
{
    public static void CreateDomainLayer(string solutionName)
    {
        string domainPath = Path.Combine(Directory.GetCurrentDirectory(), $"{solutionName}.Domain", "Entities");
        Directory.CreateDirectory(domainPath);
        File.WriteAllText(Path.Combine(domainPath, "Product.cs"),
            @$"namespace {solutionName}.Domain.Entities
{{
    public class Product
    {{
        public int Id {{ get; set; }}
        public string Name {{ get; set; }}
        public decimal Price {{ get; set; }}
    }}
}}");
    }
}