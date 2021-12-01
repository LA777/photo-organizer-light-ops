using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polo.Abstractions;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Commands;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Polo
{
    public static class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, @"settings\settings.json"), optional: false, reloadOnChange: false)
            .Build();

        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var commandParser = serviceProvider.GetRequiredService<ICommandParser>();
            var commands = serviceProvider.GetServices<ICommand>();
            var logger = serviceProvider.GetService<ILogger>();

            var task = Task.Run(() => commandParser.Parse(args, commands));

            try
            {
                ValidateApplicationSettings(serviceProvider);
                task.Wait();
            }
            catch (Exception exception)
            {
                logger?.Error(exception.Message);
            }
        }

        private static void ValidateApplicationSettings(ServiceProvider serviceProvider)
        {
            var applicationSettings = serviceProvider.GetService<IOptions<ApplicationSettings>>()?.Value;

            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings), "ERROR: Application settings are absent.");
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<ApplicationSettings>()
                .Bind(Configuration)
                .ValidateDataAnnotations();

            var applicationSettings = Configuration.Get<ApplicationSettings>();
            var applicationSettingsReadOnly = new ApplicationSettingsReadOnly(applicationSettings);
            IOptions<ApplicationSettingsReadOnly> applicationSettingsReadOnlyOptions = Options.Create(applicationSettingsReadOnly);

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("Version", version)
                .WriteTo.Seq("http://localhost:5341", apiKey: "amlPYT4U7TP5u5w8GhWR", controlLevelSwitch: new LoggingLevelSwitch(LogEventLevel.Verbose))// TODO LA - Move logging setting to JSON file
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(Path.Combine(AppContext.BaseDirectory, applicationSettings.LogFilePath), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(logger: logger, dispose: true);
            });

            services.AddSingleton<ILogger>(logger);
            services.AddSingleton(applicationSettingsReadOnlyOptions);
            services.AddSingleton<ICommandParser, CommandParser>();
            services.AddSingleton<ICommand, VersionCommand>();
            services.AddSingleton<ICommand, HelpCommand>();
            services.AddSingleton<ICommand, MoveRawToJpegFolderCommand>();
            services.AddSingleton<ICommand, RawCommand>();
            services.AddSingleton<ICommand, RemoveOrphanageRawCommand>();
            services.AddSingleton<ICommand, CopyAllFilesCommand>();
            services.AddSingleton<ICommand, MoveAllFilesCommand>();
            services.AddSingleton<ICommand, MoveVideoToSubfolderCommand>();
            services.AddSingleton<ICommand, AddWatermarkCommand>();
            services.AddSingleton<ICommand, ResizeCommand>();
            services.AddSingleton<ICommand, ResizeWithWatermarkCommand>();
            services.AddSingleton<ICommand, ClearExifCommand>();
            services.AddSingleton<ICommand, UpdateExifDateCommand>();
            services.AddSingleton<ICommand, GooglePhotoUploadCommand>();
            services.AddSingleton<ICommand, RemoveRedundantFilesCommand>();
            services.AddSingleton<ICommand, GooglePhotoCompareCommand>();
            services.AddSingleton<ICommand, CompareFileNamesCommand>();
            services.AddSingleton<ICommand, ShowVideoFilesCommand>();
            services.AddSingleton<ICommand, ConvertExifTimezoneCommand>();
        }
    }
}
