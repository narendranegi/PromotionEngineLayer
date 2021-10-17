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

namespace PromotionEngine.Functions
{
    public class PromotionEngine
    {
        private readonly ILogger<PromotionEngine> _logger;

        public PromotionEngine(ILogger<PromotionEngine> logger)
        {
            _logger = logger;
        }

        [FunctionName("RunPromotionEngine")]
        public async Task<IActionResult> RunPromotionEngineAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            [RequestBodyType(typeof(CartRequest), "Promotion engine executed")]
            HttpRequest req)
        {
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var cartItem = JsonConvert.DeserializeObject<CartRequest>(requestBody);

            string responseMessage = "Promotion Engine function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
