using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MagicMedia;
using MagicMedia.BingMaps;
using MagicMedia.Discovery;
using MagicMedia.Face;
using MagicMedia.Playground;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Extensions.Context;

namespace Playground
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceProvider sp = BuildServiceProvider();
            ImportSample importSample = sp.GetService<ImportSample>();

            DiscoverySample discovery = sp.GetService<DiscoverySample>();
            FaceScanner faceScanner = sp.GetService<FaceScanner>();

            await faceScanner.RunAsync(default);

            return;

            await discovery.ScanExistingAsync( new FileSystemDiscoveryOptions
            {
                Locations = new List<FileDiscoveryLocation>
                {
                    new FileDiscoveryLocation
                    {
                         Filter = "*.jpg",
                         Path = @"Family\2019",
                         Root = @"P:\Drive\Moments",
                    }
                }
            }, default);

        }

        private static IServiceProvider BuildServiceProvider()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .AddUserSecrets<Program>(optional: true)
                 .Build();

            BingMapsOptions bingOptions = config.GetSection("MagicMedia:BingMaps")
                .Get<BingMapsOptions>();

            var services = new ServiceCollection();
            services.AddMongoDbStore(config);

            services.AddFileSystemStore(config);
            services.AddMagicMedia(config);
            services.AddBingMaps(bingOptions);

            services.Decorate<IGeoDecoderService, GeoDecoderCacheStore>();

            services.AddSingleton<ImportSample>();
            services.AddSingleton<DiscoverySample>();
            services.AddSingleton<FaceScanner>();
            services.AddFileSystemDiscovery();

            return services.BuildServiceProvider();
        }
    }
}
