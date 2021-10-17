using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using System.Reflection;
using CombinedPromotion.Helpers;
using CombinedPromotion.Helpers.Contracts;
using CombinedPromotion.Services;
using CombinedPromotion.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CombinedPromotion.Startup))]
namespace CombinedPromotion
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            AddScopedServices(builder);
            AddSwaggerServices(builder);
        }

        private void AddScopedServices(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IConfigurationHelper, ConfigurationHelper>();
            builder.Services.AddScoped<IApplyPromotionService, ApplyPromotionService>();
        }

        private void AddSwaggerServices(IFunctionsHostBuilder builder)
        {
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts =>
            {
                opts.SpecVersion = OpenApiSpecVersion.OpenApi3_0;
                opts.AddCodeParameter = true;
                opts.PrependOperationWithRoutePrefix = true;
                opts.Documents = new[]
                {
                    new SwaggerDocument
                    {
                        Name = "v1",
                        Title = "Engine",
                        Description = "Engine API",
                        Version = "v1"
                    }
                };
                opts.Title = "Engine API";
            });
        }
    }
}
