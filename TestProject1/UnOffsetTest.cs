using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using WebApplication1.Common;
using WebApplication1.Repositories.Api;
using WebApplication1.Service.Api;
using WebApplication1.Service.Dtos;
using WebApplication1.Service.Impl;

namespace TestProject1
{
    public class UnOffsetServiceTests
    {
        private UnOffsetService _unOffsetService;

        [SetUp]
        public void Setup()
        {
            _unOffsetService = new UnOffsetService(new UnOffsetAccsum(), null);
        }

        [Test]
        public void TestGetUnOffsetSum()
        {
            // Arrange
            var details = UnOffsetTestHelper.GetSampleUnOffsetDetails();
            var taiwanSemiconductorDetails = details.FindAll(detail => detail.stock == "2330");

            // Act
            var result = _unOffsetService.GetUnOffsetSum(taiwanSemiconductorDetails);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("2330", result.stock);
            Assert.AreEqual("台積電", result.stocknm);
            Assert.AreEqual(150, result.bqty); // 100 + 50
            Assert.AreEqual(74000, result.cost); // 48000 + 26000
            Assert.AreEqual(82500, result.estimateAmt); // 55000 + 27500
        }

        [Test]
        public async Task TestGetUnOffsetAccsum()
        {
            // Arrange
            var details = UnOffsetTestHelper.GetSampleUnOffsetDetails();
            var sumList = new List<UnOffsetSum>
            {
                _unOffsetService.GetUnOffsetSum(details.FindAll(detail => detail.stock == "2330")),
                _unOffsetService.GetUnOffsetSum(details.FindAll(detail => detail.stock == "2317"))
            };

            // Act
            var result = await _unOffsetService.GetUnOffsetAccsum(sumList);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("0000", result.errcode);
            Assert.AreEqual(300, result.bqty); // 150 (台積電) + 150 (鴻海)
            Assert.AreEqual(89000, result.cost); // 74000 (台積電) + 15000 (鴻海)
            Assert.AreEqual(99000, result.estimateAmt); // 82500 (台積電) + 16500 (鴻海)
        }
    }
}
