﻿using Ether.Network.Packets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rhisis.Core.Handlers;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database;
using Rhisis.Login.Core;
using Rhisis.Login.Core.Packets;
using Rhisis.Login.Packets;
using Rhisis.Network.Packets;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Rhisis.Login
{
    public static class Program
    {
        private static async Task Main()
        {
            const string culture = "en-US";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(culture);

            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("config/login.json", optional: false);
                    configApp.AddJsonFile("config/database.json", optional: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<LoginConfiguration>(hostContext.Configuration.GetSection("loginServer"));
                    services.Configure<ISCConfiguration>(hostContext.Configuration.GetSection("isc"));
                    services.RegisterDatabaseServices(hostContext.Configuration.Get<DatabaseConfiguration>());

                    services.AddHandlers();

                    // Login Server
                    services.AddSingleton<ILoginServer, LoginServer>();
                    services.AddSingleton<ILoginPacketFactory, LoginPacketFactory>();
                    services.AddSingleton<IHostedService, LoginServerService>(); // LoginServer service starting the server

                    // Core Server
                    services.AddSingleton<ICoreServer, CoreServer>();
                    services.AddSingleton<ICorePacketFactory, CorePacketFactory>();
                    services.AddSingleton<IHostedService, CoreServerService>(); // CoreServer service starting the core server
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddFilter("Microsoft", LogLevel.Warning);
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                })
                .Build();

            await host
                .AddHandlerParameterTransformer<INetPacketStream, IPacketDeserializer>((source, dest) =>
                {
                    dest?.Deserialize(source);
                    return dest;
                })
                .RunAsync();
        }
    }
}