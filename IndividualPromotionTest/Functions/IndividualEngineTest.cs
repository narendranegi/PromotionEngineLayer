using System;
using System.IO;
using System.Threading.Tasks;
using CommonModel.Models;
using IndividualPromotion.Functions;
using IndividualPromotion.Services.Contracts;
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
        private Mock<IApplyPromotionService> _applyPromotionServiceMock;
        private Mock<ILogger<IndividualEngine>> _loggerMock;
        private IndividualEngine _individualEngine;

        [TestInitialize]
        public void Initialize()
        {
            _applyPromotionServiceMock = new Mock<IApplyPromotionService>();
            _loggerMock = new Mock<ILogger<IndividualEngine>>();
            _individualEngine = new IndividualEngine(_applyPromotionServiceMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task TestRunIndividualEngine()
        {
            var request = CartHelper.Request_ScenarioA();
            var body = JsonConvert.SerializeObject(request);
            MockApplyPromotionServiceResponse(CartHelper.Response_ScenarioA());
            var result = await _individualEngine.RunIndividualEngineAsync(HttpRequestSetup(body));
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
