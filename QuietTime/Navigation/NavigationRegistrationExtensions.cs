using Microsoft.Extensions.DependencyInjection;
using QuietTime.Wpf.ViewModels;

namespace QuietTime.Navigation
{
    public static class NavigationRegistrationExtensions
    {
        public static IServiceCollection AddNavigation(this IServiceCollection services)
        {
            services.AddSingleton(s => new Navigator()
            {
                CurrentViewModel = s.GetRequiredService<MainPageVM>(),
            });

            return services;
        }
    }
}