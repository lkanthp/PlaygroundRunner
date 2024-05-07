// Services/ContractService.cs

using System.Diagnostics;

namespace AElf.Playground.Runner.Services;

public class PlaygroundService
{
    public async Task<(byte[], string)> GenerateDll(string contractCodeDirectory)
    {
        // Check if .csproj file exists in the directory
        var csprojFilePath = Path.Combine(contractCodeDirectory, "AElf.Contracts.Association.csproj");
        if (!File.Exists(csprojFilePath)) throw new Exception("The .csproj file does not exist in the directory.");

        // Get the project root directory
        var projectDirectory = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(projectDirectory, "PlaygroundRunner.csproj")))
        {
            projectDirectory = Directory.GetParent(projectDirectory).FullName;
        }

        // Define the location of the script files in your project directory
        string scriptsDirectory = Path.Combine(projectDirectory, "scripts");
        
        Console.WriteLine("scriptDirectory: "+scriptsDirectory);

        // Copy the script files to the temporary directory
        foreach (var scriptFile in Directory.GetFiles(scriptsDirectory))
        {
            var scriptFileName = Path.GetFileName(scriptFile);
            Console.WriteLine("scriptFileName: "+scriptFileName);
            File.Copy(scriptFile, Path.Combine(contractCodeDirectory, scriptFileName), true);
        }
        
        // Now, you can run the script from the temporary directory
        // var processStartInfo = new ProcessStartInfo
        // {
        //     FileName = "/bin/bash",
        //     ArgumentList = { "install_protobuf.sh", contractCodeDirectory, contractCodeDirectory },
        //     RedirectStandardOutput = true,
        //     RedirectStandardError = true,
        //     UseShellExecute = false,
        //     CreateNoWindow = true,
        //     WorkingDirectory = contractCodeDirectory // Set the working directory to the temporary directory
        // };
        //
        // var process = Process.Start(processStartInfo);
        // process.WaitForExit();
        
        // Check if the script ran successfully
        // if (process.ExitCode != 0)
        // {
        //     // Log the error message
        //     var errorMessage = await process.StandardError.ReadToEndAsync();
        //     Console.WriteLine(errorMessage);
        //
        //     throw new Exception("Failed to run the install_protobuf script.");
        // }

        // Now, you can run the script from the temporary directory
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            ArgumentList = { "generate_contract_base.sh", contractCodeDirectory, contractCodeDirectory },
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = contractCodeDirectory // Set the working directory to the temporary directory
        };

        var process = Process.Start(processStartInfo);
        process.WaitForExit();
        
        // Check if the script ran successfully
        if (process.ExitCode != 0)
        {
            // Log the error message
            var errorMessage = await process.StandardError.ReadToEndAsync();
            Console.WriteLine(errorMessage);
        
            throw new Exception("Failed to run the generate_contract_base script.");
        }
        
        //list the files in the directory
        var files = Directory.GetFiles(contractCodeDirectory);
        foreach (var file in files) Console.WriteLine(file);

        //after running the generate_contract_base script, it should generate a directory called: protobuf
        // verify if the protobuf directory exists
        // if (!Directory.Exists(Path.Combine(contractCodeDirectory, "Protobuf")))
        //     throw new Exception("Failed to generate the protobuf directory.");

        // Run the dotnet build command
        processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            ArgumentList = { "build" },
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = contractCodeDirectory
        };

        process = Process.Start(processStartInfo);
        process.WaitForExit();
        
        // Check if the script ran successfully
        if (process.ExitCode != 0)
        {
            // Log the error message
            var errorMessage = await process.StandardError.ReadToEndAsync();
            Console.WriteLine(errorMessage);
        
            throw new Exception("Failed to run the generate_contract_base script.");
        }

        // dll files are generated under bin/Debug/net6.0
        // generated dll file has file name that will end with .dll.patched
        // pull that dll file from the bin directory and return it to the client
        var dllFilePath = Directory.GetFiles(Path.Combine(contractCodeDirectory, "bin", "Debug", "net6.0"))
            .FirstOrDefault(file => file.EndsWith(".dll.patched"));
        if (dllFilePath == null) throw new Exception("Failed to generate the dll file.");
        // Read the file into a byte array
        var fileBytes = await File.ReadAllBytesAsync(dllFilePath);

        // Return the file bytes and file name
        return (fileBytes, Path.GetFileName(dllFilePath));
    }
}