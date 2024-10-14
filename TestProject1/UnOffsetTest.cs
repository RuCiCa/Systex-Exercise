using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

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
        private Util _util;
        private InMemoryCache _inMemoryCache;
        private MyDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseSqlServer("Server=鯽魚的電腦;Database=ESMP;User Id=test;Password=test;TrustServerCertificate=True;")
                .Options;

            _context = new MyDbContext(options);
            _inMemoryCache = new InMemoryCache(_context);
            _util = new Util();
            _unOffsetService = new UnOffsetService(new UnOffsetAccsum(), null, _util, _inMemoryCache);
            _inMemoryCache.MSTMBData = new Dictionary<string, MSTMB>
            {
                {
                    "2603", new MSTMB
                    {
                        STOCK = "2603",
                        CNAME = "長榮",
                        MTYPE = "T",
                        SCLASS = "1",
                        TSDATE = "19941111",
                        TEDATE = "20201130",
                        CLDATE = "20201230",
                        CPRICE = 17.5000M,
                        TPRICE = 98.1000M,
                        BPRICE = 80.3000M,
                        TSTATUS = "T",
                        SHARE = 1000,
                        MFLAG = "1",
                        TAXTYPE = "1",
                        PTYPE = "20210818",
                        DRDATE = "20210810",
                        PDRDATE = "20210810",
                        CDIV = 2.48606241M,
                        SDIV = 0M,
                        CNTDTYPE = "X",
                        TRDATE = "20211111",
                        TRTIME = "163139",
                        MODDATE = "20201220",
                        MODTIME = "222143",
                        MODUSER = "SYSTEM"
                    }
                }
            };
        }

        [TearDown]
        public void TearDown()
        {
            // 釋放 MyDbContext 的資源
            _context?.Dispose();
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
            var result = _unOffsetService.GetUnOffsetAccsum(sumList);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("0000", result.errcode);
            Assert.AreEqual(300, result.bqty); // 150 (台積電) + 150 (鴻海)
            Assert.AreEqual(89000, result.cost); // 74000 (台積電) + 15000 (鴻海)
            Assert.AreEqual(99000, result.estimateAmt); // 82500 (台積電) + 16500 (鴻海)
        }

        [Test]
        public async Task TestTMHIOCalc()
        {
            // Arrange
            var tmhios = UnOffsetTestHelper.GetSampleTMHIO();

            // Act
            var result =  _unOffsetService.GetTMHIOList(tmhios);



            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.IsNotEmpty(result, "The result list should not be empty.");

            if (result.Count > 0)
            {
                Assert.AreEqual("1", result[0].ETYPE, "The ETYPE of the first item is incorrect.");
                Assert.AreEqual(15m, result[0].AMT, "The AMT of the first item is incorrect.");
                Assert.AreEqual(1m, result[0].FEE, "The FEE of the first item is incorrect.");
                Assert.AreEqual(16m, result[0].COST, "The COST of the first item is incorrect.");
            }

            if (result.Count > 1)
            {
                Assert.AreEqual("0", result[1].ETYPE, "The ETYPE of the second item is incorrect.");
                Assert.AreEqual(175m, result[1].AMT, "The AMT of the second item is incorrect.");
                Assert.AreEqual(20m, result[1].FEE, "The FEE of the second item is incorrect.");
                Assert.AreEqual(195m, result[1].COST, "The COST of the second item is incorrect.");
            }
        }
        [Test]
        public async Task TestGetIoFlagName()
        {
            // Arrange
            var msysData = _context.MSYSTable.ToDictionary(msys => msys.VARNAME, msys => msys);

            // Act
            string flag1 = "076";
            string flag2 = "167";
            string flag3 = "0167";
            string flag4 = "3761";

            string name1 = "C76拋棄轉出";
            string name2 = "配股及匯入";
            string name3 = "配股及匯入";
            string name4 = "繼承贈與轉出";



            // Assert
            Assert.IsNotNull(msysData, "資料庫搜尋不應該為null");
            Assert.IsNotEmpty(msysData, "資料庫搜尋不應該為empty.");

            Assert.AreEqual(name1, _util.GetIoflagname(flag1, msysData));
            Assert.AreEqual(name2, _util.GetIoflagname(flag2, msysData));
            Assert.AreEqual(name3, _util.GetIoflagname(flag3, msysData));
            Assert.AreEqual(name4, _util.GetIoflagname(flag4, msysData));


        }

    }
}
