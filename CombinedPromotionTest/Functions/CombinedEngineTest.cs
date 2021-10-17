using System;
using System.IO;
using System.Threading.Tasks;
using CombinedPromotion.Functions;
using CombinedPromotion.Services.Contracts;
using CombinedPromotionTest.Helpers;
using CommonModel.Models;
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
        private Mock<IApplyPromotionService> _applyPromotionServiceMock;
        private Mock<ILogger<CombinedEngine>> _loggerMock;
        private CombinedEngine _combinedEngine;

        [TestInitialize]
        public void Initialize()
        {
            _applyPromotionServiceMock = new Mock<IApplyPromotionService>();
            _loggerMock = new Mock<ILogger<CombinedEngine>>();
            _combinedEngine = new CombinedEngine(_applyPromotionServiceMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task TestRunCombinedEngine()
        {
            var request = CartHelper.Request_ScenarioA();
            var body = JsonConvert.SerializeObject(request);
            MockApplyPromotionServiceResponse(CartHelper.Response_ScenarioA());
            var result = await _combinedEngine.RunCombinedEngineAsync(HttpRequestSetup(body));
            var resultObject = (OkObjectResult)result;
            Assert.AreEqual(resultObject.StatusCode, StatusCodes.Status200OK);
            Assert.IsTrue(resultObject.Value is PromotionEngineResponse);
            var content = resultObject.Value as PromotionEngineResponse;
            Assert.IsTrue(content.IsSuccess);
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

        private void MockApplyPromotionServiceResponse(PromotionEngineResponse response, bool isSuccess = true)
        {
            if (isSuccess)
                _applyPromotionServiceMock.Setup(x => x.ApplyPromotion(It.IsAny<CartRequest>()))
                    .Returns(response);
            else
                _applyPromotionServiceMock.Setup(x => x.ApplyPromotion(It.IsAny<CartRequest>()))
                    .Throws<Exception>();
        }

        #endregion
    }
}
