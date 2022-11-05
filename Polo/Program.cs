using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polo.Abstractions;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Commands;
using Polo.Parameters.Handler;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;

namespace Polo
{
    public static class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, @"settings\settings.json"), false, false)
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
            var applicationSettingsReadOnlyOptions = Options.Create(applicationSettingsReadOnly);

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("Version", version)
                .WriteTo.Console(LogEventLevel.Information, "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(Path.Combine(AppContext.BaseDirectory, applicationSettings.LogFilePath), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddLogging(loggingBuilder => { loggingBuilder.AddSerilog(logger, true); });

            services.AddTransient<IParameterHandler, ParameterHandler>();

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
            services.AddSingleton<ICommand, AddWatermarkWithConvertExifTimezoneCommand>();
            services.AddSingleton<ICommand, SaveFolderTreeCommand>();
            services.AddSingleton<ICommand, CopyValidImagesCommand>();
            services.AddSingleton<ICommand, MoveCorruptedImagesCommand>();
        }
    }
}