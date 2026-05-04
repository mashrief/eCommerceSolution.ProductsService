using BusinessLogicLayer.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ProductAddRequestToProductMappingProfile).Assembly);

            return services;
        }
    }
}
