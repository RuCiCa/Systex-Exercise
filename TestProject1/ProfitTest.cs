using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using WebApplication1.Service.Dtos;
using WebApplication1.Service.Impl;
using WebApplication1.Common.HCN;

namespace TestProject1
{
    public class ProfitServiceTests
    {
        private ProfitService _profitService;

        [SetUp]
        public void Setup()
        {
            _profitService = new ProfitService(null, new ProfitAccsum(), null);
        }

        [Test]
        public async Task TestSumProfitDetailOut()
        {
            // Arrange
            List<ProfitDetailOut> profitDetailOutsHCNRH = (await _profitService.GetProfitDetailOutList(ProfitTestHelper.GetSampleExtendedHCNRH().Cast<dynamic>().ToList())).ToList();
            List<ProfitDetailOut> profitDetailOutsHCNTD = (await _profitService.GetProfitDetailOutList(ProfitTestHelper.GetSampleExtendedHCNTD().Cast<dynamic>().ToList())).ToList();
            List<ProfitDetailOut> profitDetailOuts = profitDetailOutsHCNRH.Concat(profitDetailOutsHCNTD).ToList();

            // Act
            List<ProfitDetailOut> summedProfitDetails = (await _profitService.SumProfitDetailOut(profitDetailOuts)).ToList();

            // Assert
            Assert.IsNotNull(summedProfitDetails);
            Assert.AreEqual(3, summedProfitDetails.Count);
            Assert.AreEqual("2330", summedProfitDetails[0].stock);
            //50000+25000
            Assert.AreEqual(75000, summedProfitDetails[0].cost);
            //100+50
            Assert.AreEqual(150, summedProfitDetails[0].cqty);
            //9+3
            Assert.AreEqual(12, summedProfitDetails[0].fee);
            //52000+26000
            Assert.AreEqual(78000, summedProfitDetails[0].income);
            //
            Assert.AreEqual("65000", summedProfitDetails[0].mamt);
            //0+0
            Assert.AreEqual(0, summedProfitDetails[0].mqty);
            //52000+26000
            Assert.AreEqual(78000, summedProfitDetails[0].netamt);
            //(52000+26000)/(52000+26000)
            Assert.AreEqual("100%", summedProfitDetails[0].pl_ratio);
            //0+0
            Assert.AreEqual(0, summedProfitDetails[0].tax);
        }

        [Test]
        public async Task TestGetProfitSum()
        {
            // Arrange
            var profitDetailOut = new ProfitDetailOut
            {
                stock = "2330",
                stocknm = "台積電",
                tdate = "20240802",
                dseq = "S001",
                dno = "SN001",
                cqty = 100,
                cost = 50000m,
                income = 52000m,
                profit = 2000m
            };

            List<ProfitDetail> profitDetailsHCNRH = await _profitService.GetProfitDetailList(ProfitTestHelper.GetSampleExtendedHCNRH().Cast<dynamic>().ToList());
            List<ProfitDetail> profitDetailsHCNTD = await _profitService.GetProfitDetailList(ProfitTestHelper.GetSampleExtendedHCNTD().Cast<dynamic>().ToList());
            List<ProfitDetail> profitDetails = profitDetailsHCNRH.Concat(profitDetailsHCNTD).ToList();

            // Act
            var profitSum = _profitService.GetProfitSum(profitDetailOut, profitDetails, "001", "A12345");

            // Assert
            Assert.IsNotNull(profitSum);
            Assert.AreEqual("2330", profitSum.stock);
            Assert.AreEqual("台積電", profitSum.stocknm);
            Assert.AreEqual("S001", profitSum.dseq);
            Assert.AreEqual(2000, profitSum.profit);
            Assert.AreEqual(50000, profitSum.cost);
            // 2000/50000
            Assert.AreEqual("4.00%", profitSum.pl_ratio);
            Assert.AreEqual(4, profitSum.profit_detail.Count);
        }

        [Test]
        public async Task TestGetProfittAccsum()
        {
            // Arrange
            List<ProfitDetailOut> profitDetailOutsHCNRH = (await _profitService.GetProfitDetailOutList(ProfitTestHelper.GetSampleExtendedHCNRH().Cast<dynamic>().ToList())).ToList();
            List<ProfitDetailOut> profitDetailOutsHCNTD = (await _profitService.GetProfitDetailOutList(ProfitTestHelper.GetSampleExtendedHCNTD().Cast<dynamic>().ToList())).ToList();
            List<ProfitDetailOut> profitDetailOuts = profitDetailOutsHCNRH.Concat(profitDetailOutsHCNTD).ToList();
            List<ProfitDetailOut> summedProfitDetails = (await _profitService.SumProfitDetailOut(profitDetailOuts)).ToList();

            List<ProfitDetail> profitDetailsHCNRH = await _profitService.GetProfitDetailList(ProfitTestHelper.GetSampleExtendedHCNRH().Cast<dynamic>().ToList());
            List<ProfitDetail> profitDetailsHCNTD = await _profitService.GetProfitDetailList(ProfitTestHelper.GetSampleExtendedHCNTD().Cast<dynamic>().ToList());
            List<ProfitDetail> profitDetails = profitDetailsHCNRH.Concat(profitDetailsHCNTD).ToList();

            var profitSums = new List<ProfitSum>();

            foreach (var detailOut in summedProfitDetails)
            {
                var profitSum = _profitService.GetProfitSum(detailOut, profitDetails, "001", "A12345");
                profitSums.Add(profitSum);
            }

            // Act
            var profitAccsum = await _profitService.GetProfittAccsum(profitSums);

            // Assert
            Assert.IsNotNull(profitAccsum);
            Assert.AreEqual("0000", profitAccsum.errcode);
            //75000+20000+20000
            Assert.AreEqual(75000+20000+20000, profitAccsum.cost);
            //150+200+200
            Assert.AreEqual(150+200+200, profitAccsum.cqty);
            //12+4+4
            Assert.AreEqual(12+4+4, profitAccsum.fee);
            //78000+22000+22000
            Assert.AreEqual(78000+22000+22000, profitAccsum.income);
            //(75000+20000+20000)/(75000+20000+20000)*100
            Assert.AreEqual("100%", profitAccsum.pl_ratio);
            //75000+20000+20000
            Assert.AreEqual(75000+20000+20000, profitAccsum.profit);
            Assert.AreEqual(3, profitAccsum.profit_sum.Count);
        }
    }
}
