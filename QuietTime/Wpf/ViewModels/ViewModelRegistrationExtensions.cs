using Microsoft.Extensions.DependencyInjection;

namespace QuietTime.Wpf.ViewModels
{
    public static class ViewModelRegistrationExtensions
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<HostViewModel>()
            .AddTransient<NavigationBarVM>()
            .AddTransient<TrayIconVM>()
            .AddTransient<MainPageVM>()
            .AddTransient<ScheduleWindowVM>()
            .AddTransient<SettingsPageVM>();

            return services;
        }
    }
}