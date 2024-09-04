using System.Collections.Generic;
using System.Linq;
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
        public async Task ProfitSum_HCNRH_Calculation_Check()
        {
            // Arrange
            var hcnrhList = ProfitTestHelper.GetSampleExtendedHCNRH();

            // Act
            var profitDetailSets = await _profitService.GetProfitDetailSets(hcnrhList.Cast<dynamic>().ToList());
            var profitSumList = await _profitService.GetProfitSumList("001", "A12345", profitDetailSets);

            // Assert
            Assert.NotNull(profitSumList);
            Assert.AreEqual(3, profitDetailSets.Count);  // 確認生成了3個ProfitDetailSet

            // 檢查每個ProfitDetail的數據是否正確
            foreach (var profitDetailSet in profitDetailSets)
            {
                var tdate = profitDetailSet.profitDetailOut.tdate;
                var sdseq = profitDetailSet.profitDetailOut.dseq;
                var sdno = profitDetailSet.profitDetailOut.dno;

                var expectedCQTY = hcnrhList
                    .Where(x => x.TDATE == tdate && x.SDSEQ == sdseq && x.SDNO == sdno)
                    .Sum(x => x.CQTY);
                var expectedBPRICE = hcnrhList
                    .Where(x => x.TDATE == tdate && x.SDSEQ == sdseq && x.SDNO == sdno)
                    .Sum(x => x.BPRICE * x.CQTY) / expectedCQTY;

                var actualCQTY = profitDetailSet.profitDetailOut.cqty;
                var actualBPRICE = decimal.Parse(profitDetailSet.profitDetailOut.mprice);

                Assert.AreEqual(expectedCQTY, actualCQTY, $"CQTY 計算錯誤 for TDATE: {tdate}, SDSEQ: {sdseq}, SDNO: {sdno}");
                Assert.AreEqual(expectedBPRICE, actualBPRICE, $"BPRICE 計算錯誤 for TDATE: {tdate}, SDSEQ: {sdseq}, SDNO: {sdno}");
            }
        }

        [Test]
        public async Task ProfitSum_HCNTD_Calculation_Check()
        {
            // Arrange
            var hcntdList = ProfitTestHelper.GetSampleExtendedHCNTD();

            // Act
            var profitDetailSets = await _profitService.GetProfitDetailSets(hcntdList.Cast<dynamic>().ToList());
            var profitSumList = await _profitService.GetProfitSumList("001", "A12345", profitDetailSets);

            // Assert
            Assert.NotNull(profitSumList);
            Assert.AreEqual(3, profitDetailSets.Count);  // 確認生成了3個ProfitDetailSet

            // 檢查每個ProfitDetail的數據是否正確
            foreach (var profitDetailSet in profitDetailSets)
            {
                var tdate = profitDetailSet.profitDetailOut.tdate;
                var sdseq = profitDetailSet.profitDetailOut.dseq;
                var sdno = profitDetailSet.profitDetailOut.dno;

                var expectedCQTY = hcntdList
                    .Where(x => x.TDATE == tdate && x.SDSEQ == sdseq && x.SDNO == sdno)
                    .Sum(x => x.CQTY);
                var expectedBPRICE = hcntdList
                    .Where(x => x.TDATE == tdate && x.SDSEQ == sdseq && x.SDNO == sdno)
                    .Sum(x => x.BPRICE * x.CQTY) / expectedCQTY;

                var actualCQTY = profitDetailSet.profitDetailOut.cqty;
                var actualBPRICE = decimal.Parse(profitDetailSet.profitDetailOut.mprice);

                Assert.AreEqual(expectedCQTY, actualCQTY, $"CQTY 計算錯誤 for TDATE: {tdate}, SDSEQ: {sdseq}, SDNO: {sdno}");
                Assert.AreEqual(expectedBPRICE, actualBPRICE, $"BPRICE 計算錯誤 for TDATE: {tdate}, SDSEQ: {sdseq}, SDNO: {sdno}");
            }
        }

        [Test]
        public async Task ProfitDetailSets_Should_Not_Combine_Different_Keys()
        {
            // Arrange
            var hcnrhList = ProfitTestHelper.GetSampleExtendedHCNRH();
            var hcntdList = ProfitTestHelper.GetSampleExtendedHCNTD();

            // Act
            var profitDetailSetsHCNRH = await _profitService.GetProfitDetailSets(hcnrhList.Cast<dynamic>().ToList());
            var profitDetailSetsHCNTD = await _profitService.GetProfitDetailSets(hcntdList.Cast<dynamic>().ToList());

            // Assert that different TDATE, SDSEQ, SDNO do not combine
            foreach (var profitDetailSet in profitDetailSetsHCNRH)
            {
                var tdate = profitDetailSet.profitDetailOut.tdate;
                var sdseq = profitDetailSet.profitDetailOut.dseq;
                var sdno = profitDetailSet.profitDetailOut.dno;

                var matchedEntries = hcnrhList.Where(x => x.TDATE == tdate && x.SDSEQ == sdseq && x.SDNO == sdno).ToList();
                Assert.AreEqual(matchedEntries.Count, profitDetailSet.profitDetails.Count, $"Mismatch in combined entries for HCNRH with TDATE: {tdate}, SDSEQ: {sdseq}, SDNO: {sdno}");
            }

            foreach (var profitDetailSet in profitDetailSetsHCNTD)
            {
                var tdate = profitDetailSet.profitDetailOut.tdate;
                var sdseq = profitDetailSet.profitDetailOut.dseq;
                var sdno = profitDetailSet.profitDetailOut.dno;

                var matchedEntries = hcntdList.Where(x => x.TDATE == tdate && x.SDSEQ == sdseq && x.SDNO == sdno).ToList();
                Assert.AreEqual(matchedEntries.Count, profitDetailSet.profitDetails.Count, $"Mismatch in combined entries for HCNTD with TDATE: {tdate}, SDSEQ: {sdseq}, SDNO: {sdno}");
            }
        }
    }
}
