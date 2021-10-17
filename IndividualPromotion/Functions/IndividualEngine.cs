using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using CommonModel.Models;
using IndividualPromotion.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IndividualPromotion.Functions
{
    public class IndividualEngine
    {
        private readonly IApplyPromotionService _promotionService;
        private readonly ILogger<IndividualEngine> _logger;

        public IndividualEngine(IApplyPromotionService promotionService
            , ILogger<IndividualEngine> logger)
        {
            _promotionService = promotionService;
            _logger = logger;
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PromotionEngineResponse))]
        [FunctionName("RunIndividualEngine")]
        public async Task<IActionResult> RunIndividualEngineAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            [RequestBodyType(typeof(CartRequest), "Individual engine executed")]
            HttpRequest req)
        {

            string orderId = string.Empty;

            try
            {
                _logger.LogDebug("IndividualEngine.RunIndividualEngineAsync processed cart request. {orderId}", orderId);
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<CartRequest>(requestBody);
                orderId = data.OrderId;
                var result = _promotionService.ApplyPromotion(data);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                var result = new Result
                {
                    Code = $"UWRule1_Err_{orderId}",
                    Note = ex.Message
                };
                _logger.LogError(ex, "IndividualEngine.RunIndividualEngineAsync failed. {orderId}", orderId);
                return new OkObjectResult(new PromotionEngineResponse
                {
                    IsSuccess = false,
                    ResultCodes = new List<Result> { result }
                });
            }
        }
    }
}
