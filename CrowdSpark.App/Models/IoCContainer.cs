using CrowdSpark.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace CrowdSpark.App.Models
{
    public class IoCContainer
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
            services.AddScoped<RegisterPageViewModel>();
            services.AddScoped<ISettings, Settings>();
            services.AddScoped<INavigationService, NavigationService>();
            services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();
            services.AddScoped<DelegatingHandler, AuthorizedHandler>();

            // logic
            services.AddScoped<IProjectAPI, ProjectAPI>();
            services.AddScoped<IUserAPI, UserAPI>();
            services.AddScoped<ISkillAPI, SkillAPI>();

            return services.BuildServiceProvider();
        }
    }
}
