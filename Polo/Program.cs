using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polo.Abstractions;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Services;
using Polo.Commands;
using Polo.Options;
using Polo.Services;
using System.IO;
using System.Threading.Tasks;

namespace Polo
{
    internal class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("settings.json", optional: false, reloadOnChange: false)
            .Build();

        private static void Main(string[] args)
        {
            // C:\Git\LA777\photo-organizer-light-ops\Polo\bin\Debug\netcoreapp3.1
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var commandParser = serviceProvider.GetRequiredService<ICommandParser>();
            var commands = serviceProvider.GetServices<ICommand>();

            Task.Run(() => commandParser.Parse(args, commands)).Wait();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<ApplicationSettings>(Configuration);

            //var loggerSetings = new LoggerSettings();
            // Configuration.GetSection("LoggerSettings").Bind(loggerSetings);
            // var logger = new SimpleLogger(loggerSetings);

            // services.AddSingleton<ISimpleLogger>(logger);


            services.AddSingleton<ICommandParser, CommandParser>();

            services.AddSingleton<IConsoleService, ConsoleService>();

            services.AddSingleton<ICommand, VersionCommand>();
            services.AddSingleton<ICommand, HelpCommand>();
            services.AddSingleton<ICommand, MoveRawToJpegFolderCommand>();
            services.AddSingleton<ICommand, RawCommand>();
            services.AddSingleton<ICommand, RemoveOrphanageRawCommand>();
            services.AddSingleton<ICommand, CopyFilesCommand>();
        }
    }
}
