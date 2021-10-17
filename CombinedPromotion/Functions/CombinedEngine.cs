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

namespace CombinedPromotion.Functions
{
    public class CombinedEngine
    {
        private readonly ILogger<CombinedEngine> _logger;

        public CombinedEngine(ILogger<CombinedEngine> logger)
        {
            _logger = logger;
        }

        [FunctionName("RunCombinedEngine")]
        public async Task<IActionResult> RunCombinedEngineAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            [RequestBodyType(typeof(CartRequest), "Combined engine executed")]
            HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var cartItem = JsonConvert.DeserializeObject<CartRequest>(requestBody);

            string responseMessage = "Combined Engine function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
