using AElf.Playground.Runner.Services;

namespace AElf.Playground.Runner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Instantiate the PlaygroundService
            var playgroundService = new PlaygroundService();

            // Define the contract code directory
            var contractCodeDirectory = "/Users/lakshmi/Documents/aef-playground/AElf/contract/AElf.Contracts.Association";

            // Generate the DLL
            var (fileBytes, fileName) = await playgroundService.GenerateDll(contractCodeDirectory);

            // Save the DLL to a file
            var filePath = Path.Combine(contractCodeDirectory, fileName);
            await File.WriteAllBytesAsync(filePath, fileBytes);

            Console.WriteLine($"DLL saved to {filePath}");
        }
    }
}