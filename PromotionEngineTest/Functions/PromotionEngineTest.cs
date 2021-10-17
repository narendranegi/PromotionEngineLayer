using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using PromotionEngineTest.Helpers;

namespace PromotionEngineTest.Functions
{
    [TestClass]
    public class PromotionEngineTest
    {
        private Mock<ILogger<PromotionEngine.Functions.PromotionEngine>> _loggerMock;
        private PromotionEngine.Functions.PromotionEngine _promotionEngine;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILogger<PromotionEngine.Functions.PromotionEngine>>();
            _promotionEngine = new PromotionEngine.Functions.PromotionEngine(_loggerMock.Object);
        }

        [TestMethod]
        public async Task TestRunPromotionEngine()
        {
            var request = CartHelper.CartRequest();
            var body = JsonConvert.SerializeObject(request);
            var result = await _promotionEngine.RunPromotionEngineAsync(HttpRequestSetup(body));
            var resultObject = (OkObjectResult)result;
            Assert.AreEqual(resultObject.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultObject.Value, "Promotion Engine function executed successfully.");
        }

        #region Private Methods

        private HttpRequest HttpRequestSetup(string body)
        {
            var reqMock = new Mock<HttpRequest>();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(body);
            writer.Flush();
            stream.Position = 0;
            reqMock.Setup(req => req.Body).Returns(stream);
            return reqMock.Object;
        }

        #endregion
    }
}
