﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polo.Abstractions;
using Polo.Abstractions.Commands;
using Polo.Commands;
using Polo.Options;
using Serilog;
using System.IO;
using System.Threading.Tasks;

namespace Polo
{
    internal class Program
    {
        public static readonly string Version = "0.0.3";

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
            var applicationSettings = Configuration.Get<ApplicationSettings>();

            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProperty("Version", Version)
                .WriteTo.Console()
                .WriteTo.File(applicationSettings.LogFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddSingleton<ILogger>(logger);
            services.AddSingleton<ICommandParser, CommandParser>();
            services.AddSingleton<ICommand, VersionCommand>();
            services.AddSingleton<ICommand, HelpCommand>();
            services.AddSingleton<ICommand, MoveRawToJpegFolderCommand>();
            services.AddSingleton<ICommand, RawCommand>();
            services.AddSingleton<ICommand, RemoveOrphanageRawCommand>();
            services.AddSingleton<ICommand, CopyFilesCommand>();
            services.AddSingleton<ICommand, MoveFilesCommand>();
            services.AddSingleton<ICommand, MoveVideoToSubfolderCommand>();
        }
    }
}
