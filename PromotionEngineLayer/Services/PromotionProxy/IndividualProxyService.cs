using CommonModel.Constants;
using CommonModel.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PromotionEngine.Helpers.Contracts;
using PromotionEngine.Services.PromotionProxy.Contracts;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PromotionEngine.Services.PromotionProxy
{
    public class IndividualProxyService : IPromotionProxyService
    {
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IndividualProxyService> _logger;

        public IndividualProxyService(IConfigurationHelper configurationHelper
          , IHttpClientFactory clientFactory
          , ILogger<IndividualProxyService> logger)
        {
            _configurationHelper = configurationHelper;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<PromotionEngineResponse> GetPromotionRuleResult(CartRequest cartItems)
        {
            PromotionEngineResponse promotionEngineResponse = null;

            try
            {
                _logger.LogDebug("IndividualProxyService.GetPromotionRuleResult processed. {orderId}"
                      , cartItems.OrderId);
                var requestUri = new Uri(_configurationHelper.GetUri(PromotionRuleType.Individual));
                var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(JsonConvert.SerializeObject(cartItems), Encoding.UTF8);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var client = _clientFactory.CreateClient();
                using HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                Stream content = await response.Content.ReadAsStreamAsync();
                using var streamReader = new StreamReader(content, new UTF8Encoding());
                using var jsonTextReader = new JsonTextReader(streamReader);
                var jToken = await JToken.LoadAsync(jsonTextReader);
                promotionEngineResponse = jToken.ToObject<PromotionEngineResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IndividualProxyService.GetPromotionRuleResult failed. {orderId}"
                    , cartItems.OrderId);
            }

            return promotionEngineResponse;
        }
    }
}
