using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using WebApplication1.Service.Dtos;
using WebApplication1.Service.Impl;
using WebApplication1.Common.HCN;
using WebApplication1.service.dtos;
using WebApplication1.Common;
using WebApplication1.Service.Api;

namespace TestProject1
{
    public class ProfitServiceTests
    {
        private ProfitService _profitService;
        private Util _util;

        [SetUp]
        public void Setup()
        {
            _util = new Util();
            _profitService = new ProfitService(new ProfitAccsum(), null, _util, null); // 初始化 ProfileService
        }

        [Test]
        public async Task ProfitSum_HCNRH_Calculation_Check()
        {
            // Arrange
            var hcnrhList = ProfitTestHelper.GetSampleExtendedHCNRH();

            // Act
            var profitSumList = _profitService.GetProfitSumList(hcnrhList.Cast<dynamic>().ToList(), "001", "A12345");
            var profitSum = profitSumList.FirstOrDefault();
            // Assert
            Assert.NotNull(profitSumList);
            Assert.AreEqual("20210104", profitSum.tdate);
            Assert.AreEqual("t0146", profitSum.dseq);
            Assert.AreEqual("0000005", profitSum.dno);
            Assert.AreEqual("0", profitSum.ttype);
            Assert.AreEqual("現股", profitSum.ttypename);
            Assert.AreEqual("S", profitSum.bstype);
            Assert.AreEqual("2337", profitSum.stock);
            Assert.AreEqual(4000, profitSum.cqty);
            Assert.AreEqual("41.8500", profitSum.mprice);
            Assert.AreEqual(239, profitSum.fee);
            Assert.AreEqual(502, profitSum.tax);
            Assert.AreEqual(174897, profitSum.cost);
            Assert.AreEqual(166659, profitSum.income);
            Assert.AreEqual(-8238, profitSum.profit);
            Assert.AreEqual("-4.71%", profitSum.pl_ratio);
            Assert.AreEqual("0", profitSum.ctype);
            Assert.AreEqual("現賣", profitSum.ttypename2);
        }

        [Test]
        public async Task ProfitSum_HCNTD_Calculation_Check()
        {
            // Arrange
            var hcntdList = ProfitTestHelper.GetSampleExtendedHCNTD();

            // Act
            var profitSumList = _profitService.GetProfitSumList(hcntdList.Cast<dynamic>().ToList(), "001", "A12345");
            var profitSum = profitSumList.FirstOrDefault();
            // Assert
            Assert.NotNull(profitSumList);
            Assert.AreEqual("20210104", profitSum.tdate);
            Assert.AreEqual("0000016", profitSum.dseq);
            Assert.AreEqual("0000020", profitSum.dno);
            Assert.AreEqual("0", profitSum.ttype);
            Assert.AreEqual("現股", profitSum.ttypename);
            Assert.AreEqual("S", profitSum.bstype);
            Assert.AreEqual("8069", profitSum.stock);
            Assert.AreEqual(1000, profitSum.cqty);
            Assert.AreEqual("48.2500", profitSum.mprice);
            Assert.AreEqual(68, profitSum.fee);
            Assert.AreEqual(72, profitSum.tax);
            Assert.AreEqual(49320, profitSum.cost);
            Assert.AreEqual(48110, profitSum.income);
            Assert.AreEqual(-1210, profitSum.profit);
            Assert.AreEqual("-2.45%", profitSum.pl_ratio);
            Assert.AreEqual("0", profitSum.ctype);
            Assert.AreEqual("賣沖", profitSum.ttypename2);
        }

        [Test]
        public async Task ProfitAccsum_Calculation_Check()
        {
            // Arrange
            var hcntdList = ProfitTestHelper.GetOneExtendedHCNTD();
            var hcnrhList = ProfitTestHelper.GetSampleExtendedHCNRH();

            // Act
            var profitSumListHCNRH = _profitService.GetProfitSumList(hcnrhList.Cast<dynamic>().ToList(), "001", "A12345");
            var profitSumListHCNTD = _profitService.GetProfitSumList(hcntdList.Cast<dynamic>().ToList(), "001", "A12345");
            var profitSumList = profitSumListHCNRH.Concat(profitSumListHCNTD).ToList();
            var profitAccsum = _profitService.GetProfittAccsum(profitSumList);

            // Assert
            Assert.NotNull(profitSumList);
            Assert.AreEqual("0000", profitAccsum.errcode);
            Assert.AreEqual("成功", profitAccsum.errmsg);
            Assert.AreEqual(4000 + 1000, profitAccsum.cqty);
            Assert.AreEqual(174897 + 49320, profitAccsum.cost);
            Assert.AreEqual(166659 + 48110, profitAccsum.income);
            Assert.AreEqual(-8238 - 1210, profitAccsum.profit);
            Assert.AreEqual("-4.21%", profitAccsum.pl_ratio);
            Assert.AreEqual(239 + 68, profitAccsum.fee);
            Assert.AreEqual(502 + 72, profitAccsum.tax);
        }
    }
}
