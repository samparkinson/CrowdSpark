using CrowdSpark.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
namespace CrowdSpark.App.Models
{
    class IoCContainer
    {
        public static IServiceProvider Create() => ConfigureServices();

        private static IServiceProvider ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddScoped<HttpClient>();
            services.AddScoped<MainPageViewModel>();
            services.AddScoped<UserPageViewModel>();
            services.AddScoped<ProjectPageViewModel>();
            services.AddScoped<SearchPageViewModel>();
            services.AddScoped<AddProjectPageViewModel>();
            services.AddScoped<ISettings, Settings>();
            services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();
            services.AddScoped<DelegatingHandler, AuthorizedHandler>();

            return services.BuildServiceProvider();
        }
    }
}
