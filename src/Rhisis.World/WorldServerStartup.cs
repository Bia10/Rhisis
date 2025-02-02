﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Rhisis.Business;
using Rhisis.Core;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.Resources;
using Rhisis.Core.Resources.Loaders;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database;
using Rhisis.Network;
using Rhisis.World.Game.Loaders;
using Rhisis.World.ISC;
using Rhisis.World.Systems.Party;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Rhisis.World
{
    public sealed class WorldServerStartup : IProgramStartup
    {
        private const string WorldConfigFile = "config/world.json";
        private const string DatabaseConfigFile = "config/database.json";

        private readonly IEnumerable<Type> _loaders = new[]
        {
            // Common loaders
            typeof(DefineLoader),
            typeof(TextLoader),
            typeof(MoverLoader),
            typeof(ItemLoader),
            typeof(DialogLoader),
            typeof(ShopLoader),
            typeof(JobLoader),
            typeof(TextClientLoader),
            typeof(ExpTableLoader),
            typeof(PenalityLoader),

            // World server specific loaders
            typeof(NpcLoader),
            typeof(BehaviorLoader),
            typeof(MapLoader),
            typeof(SystemLoader)

            // TODO: add more loaders
        };

        private IWorldServer _server;

        /// <inheritdoc />
        public void Configure()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            PacketHandler<ISCClient>.Initialize();
            PacketHandler<WorldClient>.Initialize();
            
            var dbConfig = ConfigurationHelper.Load<DatabaseConfiguration>(DatabaseConfigFile);
            DependencyContainer.Instance
                .GetServiceCollection()
                .RegisterDatabaseServices(dbConfig);
            
            BusinessLayer.Initialize();
            DependencyContainer.Instance.Register<IWorldServer, WorldServer>(ServiceLifetime.Singleton);
            DependencyContainer.Instance.Register(typeof(PartyManager), ServiceLifetime.Singleton);
            DependencyContainer.Instance.Configure(services => services.AddLogging(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning);
#if DEBUG
                builder.SetMinimumLevel(LogLevel.Trace);
#else
                builder.SetMinimumLevel(LogLevel.Warning);
#endif
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            }));
            DependencyContainer.Instance.Configure(services =>
            {
                var worldConfiguration = ConfigurationHelper.Load<WorldConfiguration>(WorldConfigFile, true);
                services.AddSingleton(worldConfiguration);
            });
            GameResources.Instance.Initialize(this._loaders);
            DependencyContainer.Instance.BuildServiceProvider();
        }

        /// <inheritdoc />
        public void Run()
        {
            var logger = DependencyContainer.Instance.Resolve<ILogger<WorldServerStartup>>();
            this._server = DependencyContainer.Instance.Resolve<IWorldServer>();

            try
            {
                logger.LogInformation("Starting WorldServer...");

                GameResources.Instance.Load();
                this._server.Start();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, $"An unexpected error occured in WorldServer.");
#if DEBUG
                Console.ReadLine();
#endif
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._server?.Dispose();
            DependencyContainer.Instance.Dispose();
            LogManager.Shutdown();
        }
    }
}