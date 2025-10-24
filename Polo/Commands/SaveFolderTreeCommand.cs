using Polo.Abstractions.Commands;
using Polo.Abstractions.Parameters.Handler;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Polo.Commands;

public class SaveFolderTreeCommand : ICommand
{
    private const string NameLong = "save-folder-tree";
    private const string NameShort = "sft";
    private readonly ILogger _logger;

    public SaveFolderTreeCommand(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string Name => NameLong;

    public string ShortName => NameShort;

    public string Description => "Saves folder structure with all files and sub-folders.";

    public IParameterHandler ParameterHandler => new ParameterHandler
    {
        SourceParameter = new SourceParameter(),
        DestinationParameter = new DestinationParameter()
    };

    public async Task ActionAsync(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
    {
        var currentDirectory = Environment.CurrentDirectory;
        var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, currentDirectory);
        var destinationFolder = ParameterHandler.DestinationParameter!.Initialize(parameters, sourceFolderPath);

        var folderTree = CreateFolderTree(sourceFolderPath);

        var json = ConvertToJson(folderTree);

        const string outputFileName = "_FolderTree.json";
        var outputFilePath = Path.Join(destinationFolder, outputFileName);
        await System.IO.File.WriteAllTextAsync(outputFilePath, json, Encoding.Unicode);

        var message = $"Folder tree saved in file: {outputFilePath}";
        Console.WriteLine(message);
        _logger.Information(message);
    }

    private static string ConvertToJson(Folder folderTree)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true, // Your global setting
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        var json = JsonSerializer.Serialize(folderTree, serializerOptions);

        //return json.Replace(@"\\", @"\");
        return json;
    }

    private static Folder CreateFolderTree(string path)
    {
        var folder = new Folder
        {
            FolderName = new DirectoryInfo(path).Name,
            FolderPath = path //.Replace(@"\\", @"\")
        };

        var subFolders = Directory.EnumerateDirectories(path);
        foreach (var subFolderPath in subFolders)
        {
            var subFolder = CreateFolderTree(subFolderPath);
            folder.Subfolders ??= new List<Folder>();

            folder.Subfolders.Add(subFolder);
        }

        var files = Directory.EnumerateFiles(path);
        foreach (var filePath in files)
        {
            var fileInfo = new FileInfo(filePath);

            var file = new File
            {
                FileName = fileInfo.Name,
                FileSize = fileInfo.Length
            };
            folder.Files ??= new List<File>();
            folder.Files.Add(file);
        }

        return folder;
    }

    public class Folder
    {
        public string? FolderName { get; set; }
        public string? FolderPath { get; set; }
        public IList<Folder>? Subfolders { get; set; }
        public IList<File>? Files { get; set; }
    }

    public class File
    {
        public string? FileName { get; set; }
        public long FileSize { get; set; }
    }
}