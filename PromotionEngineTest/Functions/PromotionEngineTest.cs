using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommonModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using PromotionEngine.Services.PromotionEngine.Contracts;
using PromotionEngineTest.Helpers;

namespace PromotionEngineTest.Functions
{
    [TestClass]
    public class PromotionEngineTest
    {
        private Mock<IPromotionEngineService> _promotionServiceMock;
        private Mock<ILogger<PromotionEngine.Functions.PromotionEngine>> _loggerMock;
        private PromotionEngine.Functions.PromotionEngine _promotionEngine;

        [TestInitialize]
        public void Initialize()
        {
            _promotionServiceMock = new Mock<IPromotionEngineService>();
            _loggerMock = new Mock<ILogger<PromotionEngine.Functions.PromotionEngine>>();
            _promotionEngine = new PromotionEngine.Functions.PromotionEngine(_promotionServiceMock.Object
                , _loggerMock.Object);
        }

        [TestMethod]
        public async Task TestPromotionEngine_ScenarioA()
        {
            var request = CartHelper.Request_ScenarioA();
            var body = JsonConvert.SerializeObject(request);
            MockPromotionEngineResponse(CartHelper.Response_ScenarioA());
            var result = await _promotionEngine.RunPromotionEngineAsync(req: HttpRequestSetup(body));
            var resultObject = (OkObjectResult)result;
            var response = resultObject.Value as PromotionEngineResponse;
            Assert.IsTrue(response.IsSuccess);
            Assert.IsNotNull(response.TotalAmount);
            Assert.IsTrue(response.CartProductOffers.All(x => x.IsOfferApplied == false));
            Assert.IsTrue(response.CartProductOffers.All(x => string.IsNullOrEmpty(x.OfferId)));
        }

        [TestMethod]
        public async Task TestPromotionEngine_ScenarioB()
        {
            var request = CartHelper.Request_ScenarioB();
            var body = JsonConvert.SerializeObject(request);
            MockPromotionEngineResponse(CartHelper.Response_ScenarioB());
            var result = await _promotionEngine.RunPromotionEngineAsync(req: HttpRequestSetup(body));
            var resultObject = (OkObjectResult)result;
            var response = resultObject.Value as PromotionEngineResponse;
            Assert.IsTrue(response.IsSuccess);
            Assert.IsNotNull(response.TotalAmount);
            Assert.IsTrue(response.CartProductOffers.Any(x => x.IsOfferApplied));
            Assert.IsTrue(response.CartProductOffers.Any(x => !string.IsNullOrEmpty(x.OfferId)));
        }

        [TestMethod]
        public async Task TestPromotionEngine_ScenarioC()
        {
            var request = CartHelper.Request_ScenarioC();
            var body = JsonConvert.SerializeObject(request);
            MockPromotionEngineResponse(CartHelper.Response_ScenarioC());
            var result = await _promotionEngine.RunPromotionEngineAsync(req: HttpRequestSetup(body));
            var resultObject = (OkObjectResult)result;
            var response = resultObject.Value as PromotionEngineResponse;
            Assert.IsTrue(response.IsSuccess);
            Assert.IsNotNull(response.TotalAmount);
            Assert.IsTrue(response.CartProductOffers.All(x => x.IsOfferApplied));
            Assert.IsTrue(response.CartProductOffers.All(x => !string.IsNullOrEmpty(x.OfferId)));
        }

        [TestMethod]
        public async Task TestPromotionEngine_Bad_Request()
        {
            var result = await _promotionEngine.RunPromotionEngineAsync(req: HttpRequestSetup(string.Empty));
            var resultObject = (BadRequestObjectResult)result;
            Assert.AreEqual(resultObject.StatusCode, StatusCodes.Status400BadRequest);
            Assert.AreEqual(resultObject.Value, "Input is empty");
        }

        [TestMethod]
        public async Task TestPromotionEngine_Exception()
        {
            var request = CartHelper.Request_ScenarioA();
            var body = JsonConvert.SerializeObject(request);
            MockPromotionEngineResponse(null, false);
            var result = await _promotionEngine.RunPromotionEngineAsync(req: HttpRequestSetup(body));
            var resultObject = (ObjectResult)result;
            Assert.AreEqual(resultObject.StatusCode, StatusCodes.Status500InternalServerError);
            Assert.IsTrue(resultObject.Value is Result);
            var resultCode = resultObject.Value as Result;
            Assert.IsTrue(resultCode.Code.Contains("RunPromotionEngine_Err_"));
            Assert.IsTrue(!string.IsNullOrEmpty(resultCode.Note));
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

        private void MockPromotionEngineResponse(PromotionEngineResponse response, bool isSuccess = true)
        {
            if (isSuccess)
                _promotionServiceMock.Setup(x => x.RunPromotionEngineAsync(It.IsAny<CartRequest>()))
                .ReturnsAsync(response);
            else
                _promotionServiceMock.Setup(x => x.RunPromotionEngineAsync(It.IsAny<CartRequest>()))
                    .Throws<Exception>();
        }

        #endregion
    }
}
