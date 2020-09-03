using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Airline.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Criar o host sem o mandar correr pois quero primeiro criar o Seed
            var host = CreateWebHostBuilder(args).Build();

            //Criar o Seed
            RunSeeding(host);

            //Correr a aplicação
            host.Run();
            //CreateWebHostBuilder(args).Build().Run();
        }

        private static void RunSeeding(IWebHost host)
        {
            //Design pattern factory
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();

            using(var scope = scopeFactory.CreateScope()) 
            {
                var seeder = scope.ServiceProvider.GetService<SeedDb>();
                seeder.SeedAsync().Wait();
            }
            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
