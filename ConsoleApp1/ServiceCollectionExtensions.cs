using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceCollection AddServices(this ServiceCollection services)
        {
            services.AddSingleton<TeacherService, TeacherService>();

            return services;
        }
    }
}
