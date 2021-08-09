using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;

namespace QuietTime.Core.Scheduling
{
    public static class SchedulerRegistrationExtensions
    {
        public static async Task<IServiceCollection> AddScheduling(this IServiceCollection services)
        {
            StdSchedulerFactory schedulerFactory = new();
            IScheduler scheduler = await schedulerFactory.GetScheduler();

            services.AddSingleton<ScheduleJobFactory>()
            .AddTransient(s =>
            {
                scheduler.JobFactory = s.GetRequiredService<ScheduleJobFactory>();
                return scheduler;
            })
            .AddTransient<ISchedulingService, Scheduler>();

            return services;
        }
    }
}