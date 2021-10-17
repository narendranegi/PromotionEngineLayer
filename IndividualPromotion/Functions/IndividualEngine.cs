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

namespace IndividualPromotion.Functions
{
    public class IndividualEngine
    {
        private readonly ILogger<IndividualEngine> _logger;

        public IndividualEngine(ILogger<IndividualEngine> logger)
        {
            _logger = logger;
        }

        [FunctionName("RunIndividualEngine")]
        public async Task<IActionResult> RunIndividualEngineAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            [RequestBodyType(typeof(CartRequest), "Individual engine executed")]
            HttpRequest req)
        {
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var cartItem = JsonConvert.DeserializeObject<CartRequest>(requestBody);
           
            string responseMessage = "Individual Engine function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
