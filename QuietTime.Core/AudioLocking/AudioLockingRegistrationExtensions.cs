using Microsoft.Extensions.DependencyInjection;
using NAudio.CoreAudioApi;

namespace QuietTime.Core.AudioLocking
{
    public static class AudioLockingRegistrationExtensions
    {
        public static IServiceCollection AddAudioLocking(this IServiceCollection services)
        {
            services.AddTransient<MMDeviceEnumerator>()
            .AddSingleton<IAudioLocker, AudioLocker>();

            return services;
        }
    }
}