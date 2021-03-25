using System;
using System.IO;
using System.Text.Json;
using Scrutor;
using Microsoft.Extensions.DependencyInjection;
using UnSave.Serialization;

namespace UnSave
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.Scan(scan =>
            {
                scan.FromAssemblyOf<SaveSerializer>()
                    .AddClasses(classes => classes.AssignableTo<IUnrealPropertySerializer>()).AsImplementedInterfaces()
                    .WithSingletonLifetime();
                scan.FromAssemblyOf<SaveSerializer>()
                    .AddClasses(classes => classes.AssignableTo<IUnrealCollectionPropertySerializer>()).AsImplementedInterfaces()
                    .WithSingletonLifetime();
            });
            services.AddSingleton<PropertySerializer>();
            services.AddSingleton<SaveSerializer>();
            var provider = services.BuildServiceProvider(new ServiceProviderOptions() {ValidateOnBuild = true});
            var serializer = provider.GetService<SaveSerializer>();
            using var stream = File.Open(@"X:\ProjectWingman\Saves\Working\SaveGames\Conquest.sav", FileMode.Open, FileAccess.Read, FileShare.Read);
            var save = serializer.Read(stream);
            // var credits = save.Get<Types.UEIntProperty>("CampaignCredits").Value;
            var json = JsonSerializer.Serialize(save);
            Console.WriteLine(save.SaveGameType);
            
        }
    }
}
