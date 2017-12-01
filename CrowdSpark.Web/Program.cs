using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CrowdSpark.Entitites;

namespace CrowdSpark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var context = new CrowdSparkContext())
            {
                var user = context.User.FirstOrDefault();

                Console.WriteLine(user.Firstname);
            }

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
