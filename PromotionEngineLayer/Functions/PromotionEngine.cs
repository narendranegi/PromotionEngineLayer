using System;
using System.IO;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using CommonModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PromotionEngine.Services.PromotionEngine.Contracts;

namespace PromotionEngine.Functions
{
    public class PromotionEngine
    {
        private readonly IPromotionEngineService _promotionEngineService;
        private readonly ILogger<PromotionEngine> _logger;

        public PromotionEngine(IPromotionEngineService promotionEngineService
            , ILogger<PromotionEngine> logger)
        {
            _promotionEngineService = promotionEngineService;
            _logger = logger;
        }

        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(PromotionEngineResponse))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)System.Net.HttpStatusCode.InternalServerError)]
        [FunctionName("RunPromotionEngine")]
        public async Task<IActionResult> RunPromotionEngineAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            [RequestBodyType(typeof(CartRequest), "Promotion engine executed")]
            HttpRequest req)
        {

            string orderId = string.Empty;

            try
            {
                _logger.LogDebug("PromotionEngine.RunPromotionEngine initiated");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                // Request should not be empty
                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult("Input is empty");
                }
                var cartItem = JsonConvert.DeserializeObject<CartRequest>(requestBody);
                orderId = cartItem.OrderId;
                var result = await _promotionEngineService.RunPromotionEngineAsync(cartItem);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                var result = new Result
                {
                    Code = $"RunPromotionEngine_Err_{orderId}",
                    Note = ex.Message
                };
                _logger.LogError(ex, "PromotionEngine.RunPromotionEngine failed. {orderId}", orderId);

                return new ObjectResult(result)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
