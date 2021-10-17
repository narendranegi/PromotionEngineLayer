using System.IO;
using System.Threading.Tasks;
using IndividualPromotion.Functions;
using IndividualPromotionTest.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace IndividualPromotionTest.Functions
{
    [TestClass]
    public class IndividualEngineTest
    {
        private Mock<ILogger<IndividualEngine>> _loggerMock;
        private IndividualEngine _individualEngine;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILogger<IndividualEngine>>();
            _individualEngine = new IndividualEngine(_loggerMock.Object);
        }

        [TestMethod]
        public async Task TestRunIndividualEngine()
        {
            var request = CartHelper.CartRequest();
            var body = JsonConvert.SerializeObject(request);
            var result = await _individualEngine.RunIndividualEngineAsync(HttpRequestSetup(body));
            var resultObject = (OkObjectResult)result;
            Assert.AreEqual(resultObject.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultObject.Value, "Individual Engine function executed successfully.");
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
