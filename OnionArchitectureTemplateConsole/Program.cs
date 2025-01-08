using System;
using System.Runtime.InteropServices;
using OnionArchitectureTemplateConsole;

abstract class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Lütfen  (solution) adını girin:");
        string solutionName = Console.ReadLine();

        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

 
        SolutionCreator.CreateSolution(solutionName, isWindows);
        DomainLayerCreator.CreateDomainLayer(solutionName);
        ApplicationLayerCreator.CreateApplicationLayer(solutionName, isWindows);
        InfrastructureLayerCreator.CreateInfrastructureLayer(solutionName, isWindows);
        ApiLayerCreator.CreateApiLayer(solutionName, isWindows);
        SharedLayerCreator.CreateSharedLayer(solutionName);

        Console.WriteLine("Çözüm yapısı başarıyla oluşturuldu!");
    }
}