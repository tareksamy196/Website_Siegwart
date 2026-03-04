using Microsoft.Extensions.DependencyInjection;

namespace Website.Siegwart.BLL.Services.Classes
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVideoMediaServices(this IServiceCollection services)
        {
            services.AddScoped<IVideoMediaService, VideoMediaService>();
            return services;
        }
    }
}