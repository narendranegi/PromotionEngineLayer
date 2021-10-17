using System.IO;
using System.Threading.Tasks;
using CombinedPromotion.Functions;
using CombinedPromotionTest.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace CombinedPromotionTest.Functions
{
    [TestClass]
    public class CombinedEngineTest
    {
        private Mock<ILogger<CombinedEngine>> _loggerMock;
        private CombinedEngine _combinedEngine;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILogger<CombinedEngine>>();
            _combinedEngine = new CombinedEngine(_loggerMock.Object);
        }

        [TestMethod]
        public async Task TestRunIndividualEngine()
        {
            var request = CartHelper.CartRequest();
            var body = JsonConvert.SerializeObject(request);
            var result = await _combinedEngine.RunCombinedEngineAsync(HttpRequestSetup(body));
            var resultObject = (OkObjectResult)result;
            Assert.AreEqual(resultObject.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultObject.Value, "Combined Engine function executed successfully.");
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
