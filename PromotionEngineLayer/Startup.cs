using System;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using System.Reflection;
using CommonModel.Constants;
using Microsoft.Extensions.DependencyInjection;
using PromotionEngine.Helpers;
using PromotionEngine.Helpers.Contracts;
using PromotionEngine.Services.PromotionEngine;
using PromotionEngine.Services.PromotionEngine.Contracts;
using PromotionEngine.Services.PromotionProxy;
using PromotionEngine.Services.PromotionProxy.Contracts;

[assembly: FunctionsStartup(typeof(PromotionEngine.Startup))]
namespace PromotionEngine
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            AddHttpServices(builder);
            AddScopedServices(builder);
            AddSwaggerServices(builder);
        }

        private void AddHttpServices(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            // Add handler here to perform poly retry if any socket or network error occurs
            // Retry settings needed => retry count, time interval between retry
        }

        private void AddScopedServices(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IConfigurationHelper, ConfigurationHelper>();
            builder.Services.AddScoped<IPromotionEngineService, PromotionEngineService>();
            builder.Services.AddScoped<IndividualProxyService>();
            builder.Services.AddScoped<CombinedProxyService>();
            builder.Services.AddScoped<Func<string, IPromotionProxyService>>(sp => (promotionRule) =>
            {
                if (!string.IsNullOrEmpty(promotionRule))
                {
                    return promotionRule switch
                    {
                        PromotionRuleType.Individual => sp.GetRequiredService<IndividualProxyService>(),
                        PromotionRuleType.Combined => sp.GetRequiredService<CombinedProxyService>(),
                        _ => null
                    };
                }

                return null;
            });
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
