using AzureFunctions.Extensions.Swashbuckle.Attribute;
using CombinedPromotion.Services.Contracts;
using CommonModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CombinedPromotion.Functions
{
    public class CombinedEngine
    {
        private readonly IApplyPromotionService _promotionService;
        private readonly ILogger<CombinedEngine> _logger;

        public CombinedEngine(IApplyPromotionService promotionService
            , ILogger<CombinedEngine> logger)
        {
            _promotionService = promotionService;
            _logger = logger;
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PromotionEngineResponse))]
        [FunctionName("RunCombinedEngine")]
        public async Task<IActionResult> RunCombinedEngineAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            [RequestBodyType(typeof(CartRequest), "Combined engine executed")]
            HttpRequest req)
        {
            string orderId = string.Empty;

            try
            {
                _logger.LogDebug("CombinedEngine.RunCombinedEngineAsync processed cart request. {orderId}", orderId);
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
                    Code = $"RunCombinedEngineAsync_Err_{orderId}",
                    Note = ex.Message
                };
                _logger.LogError(ex, "CombinedEngine.RunCombinedEngineAsync failed. {orderId}", orderId);
                return new OkObjectResult(new PromotionEngineResponse
                {
                    IsSuccess = false,
                    ResultCodes = new List<Result> { result }
                });
            }
        }
    }
}
