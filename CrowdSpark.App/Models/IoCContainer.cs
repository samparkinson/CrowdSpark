using CrowdSpark.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CrowdSpark.App.Models
{
    class IoCContainer
    {
        public static IServiceProvider Create() => ConfigureServices();

        private static IServiceProvider ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            //services.AddScoped<ICharacterRepository, RestCharacterRepository>();
            services.AddScoped<HttpClient>();
            services.AddScoped<MainPageViewModel>();
            services.AddScoped<ProjectPageViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
