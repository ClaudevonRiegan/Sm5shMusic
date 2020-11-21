﻿using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sm5sh.Interfaces;
using Sm5sh.Mods.Music;
using Sm5sh.Mods.Music.Interfaces;
using Sm5sh.Mods.Music.ResourceProviders;
using Sm5sh.Mods.Music.Services;
using Sm5sh.Mods.StagePlaylist;
using Sm5sh.ResourceProviders;
using System;
using System.IO;
using Sm5sh.GUI.ViewModels;
using VGMMusic;

namespace Sm5sh.GUI
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            ConfigureServices();
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        }

        public static void ConfigureServices()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();

            var loggerFactory = LoggerFactory.Create(builder => builder
                .AddFile(Path.Combine(configuration.GetValue<string>("LogPath"), "log_{Date}.txt"), Microsoft.Extensions.Logging.LogLevel.Debug, retainedFileCountLimit: 7));

            services.AddLogging();
            services.AddOptions();
            services.AddSingleton(configuration);
            services.AddSingleton(loggerFactory);

            //Sm5sh Core
            services.Configure<Sm5shOptions>(configuration);
            services.AddSingleton<IProcessService, ProcessService>();
            services.AddSingleton<IStateManager, StateManager>();
            services.AddSingleton<IResourceProvider, MsbtResourceProvider>();
            services.AddSingleton<IResourceProvider, PrcResourceProvider>();

            //MODS - TODO LOAD DYNAMICALLY?
            //Mod Music
            services.Configure<Sm5shMusicOptions>(configuration);
            services.AddSingleton<ISm5shMod, BgmMod>();
            services.AddSingleton<IResourceProvider, BgmPropertyProvider>();
            services.AddTransient<IAudioStateService, AudioStateService>();
            services.AddSingleton<IAudioMetadataService, VGAudioMetadataService>();
            services.AddSingleton<INus3AudioService, Nus3AudioService>();

            //Mod Stage Playlist
            services.Configure<Sm5shStagePlaylistOptions>(configuration);
            services.AddSingleton<ISm5shMod, StagePlaylistMod>();

            //Add ViewModels
            services.AddSingleton<MainWindowViewModel>();

            //Add UI Services
            services.AddSingleton<IVGMMusicPlayer, VGMMusicPlayer>();

            //Add to Splat
            services.UseMicrosoftDependencyResolver();
        }
    }
}