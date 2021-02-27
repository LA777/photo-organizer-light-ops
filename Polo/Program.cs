﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polo.Abstractions;
using Polo.Abstractions.Commands;
using Polo.Commands;
using Polo.Options;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Polo
{
    public static class Program
    {
        public const string Version = "0.0.5";

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
            var applicationSettings = serviceProvider.GetService<IOptions<ApplicationSettings>>().Value;

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
            IOptions<ApplicationSettingsReadOnly> applicationSettingsReadOnlyOptions = Microsoft.Extensions.Options.Options.Create(applicationSettingsReadOnly);

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("Version", Version)
                .WriteTo.Console(LogEventLevel.Information)
                .WriteTo.File(applicationSettings.LogFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

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
        }
    }
}
