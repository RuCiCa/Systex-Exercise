using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.service.dtos;
using WebApplication1.Service.Impl;

namespace TestProject1
{
    public class ProfileTest
    {
        private ProfileService _profileService;
        private Util _util;

        [SetUp]
        public void Setup()
        {
            _util = new Util();
            _profileService = new ProfileService(new ProfileSum(), null, _util, null); // 初始化 ProfileService
        }

        [Test]
        public async Task TestCalculation_All()
        {
            // Arrange
            var TMHIOs = ProfileTestHelper.GetSampleExtendedTMHIO();
            var HCMIOs = ProfileTestHelper.GetSampleExtendedHCMIO();

            // Act
            var TMHIOList = _profileService.GetProfileList(TMHIOs.Cast<dynamic>().ToList());
            var HCMIOList = _profileService.GetProfileList(HCMIOs.Cast<dynamic>().ToList());
            var profileList = TMHIOList.Concat(HCMIOList).ToList();
            var billSum = _profileService.GetBillSum(profileList);
            var profile = _profileService.GetProfileSum(billSum, profileList);

            // Assert
            Assert.NotNull(billSum);
            Assert.AreEqual("0000", profile.errcode, "errcode錯誤");
            Assert.AreEqual("成功", profile.errmsg, "errmsg錯誤");
            Assert.AreEqual(-12863m, profile.netamt, "netamt");
            Assert.AreEqual(113m, profile.fee, "fee");
            Assert.AreEqual(100m, profile.tax, "tax");
            Assert.AreEqual(4000m, profile.mqty, "mqty");
            Assert.AreEqual(79450m, profile.mamt, "mamt");

            Assert.AreEqual(63950m, billSum.cnbamt, "cnbamt");
            Assert.AreEqual(15500m, billSum.cnsamt, "cnsamt");
            Assert.AreEqual(113m, billSum.cnfee, "cnfee");
            Assert.AreEqual(100m, billSum.cntax, "cntax");
            Assert.AreEqual(-12863m, billSum.cnnetamt, "cnnetamt");
            Assert.AreEqual(3000m, billSum.bqty, "cnnetamt");
            Assert.AreEqual(1000m, billSum.sqty, "cnnetamt");

            Assert.AreEqual(4, profile.profile.Count, "");
        }

        [Test]
        public async Task TestCalculation_TMHIO()
        {
            // Arrange
            var TMHIOs = ProfileTestHelper.GetSampleExtendedTMHIO();

            // Act
            var profileList = _profileService.GetProfileList(TMHIOs.Cast<dynamic>().ToList());
            var billSum = _profileService.GetBillSum(profileList);
            var profile = _profileService.GetProfileSum(billSum, profileList);

            // Assert
            Assert.NotNull(billSum);
            Assert.AreEqual("0000", profile.errcode, "errcode錯誤");
            Assert.AreEqual("成功", profile.errmsg, "errmsg錯誤");
            Assert.AreEqual(33252m, profile.netamt, "netamt");
            Assert.AreEqual(48m, profile.fee, "fee");
            Assert.AreEqual(100m, profile.tax, "tax");
            Assert.AreEqual(2000m, profile.mqty, "mqty");
            Assert.AreEqual(33400m, profile.mamt, "mamt");

            Assert.AreEqual(17900m, billSum.cnbamt, "cnbamt");
            Assert.AreEqual(15500m, billSum.cnsamt, "cnsamt");
            Assert.AreEqual(48m, billSum.cnfee, "cnfee");
            Assert.AreEqual(100m, billSum.cntax, "cntax");
            Assert.AreEqual(33252m, billSum.cnnetamt, "cnnetamt");
            Assert.AreEqual(1000m, billSum.bqty, "cnnetamt");
            Assert.AreEqual(1000m, billSum.sqty, "cnnetamt");

            Assert.AreEqual(2, profile.profile.Count, "");
        }

        [Test]
        public async Task TestCalculation_HCMIO()
        {
            // Arrange
            var HCMIOs = ProfileTestHelper.GetSampleExtendedHCMIO();

            // Act
            var profileList = _profileService.GetProfileList(HCMIOs.Cast<dynamic>().ToList());
            var billSum = _profileService.GetBillSum(profileList);
            var profile = _profileService.GetProfileSum(billSum, profileList);

            // Assert
            Assert.NotNull(billSum);
            Assert.AreEqual("0000", profile.errcode, "errcode錯誤");
            Assert.AreEqual("成功", profile.errmsg, "errmsg錯誤");
            Assert.AreEqual(-46115m, profile.netamt, "netamt");
            Assert.AreEqual(65m, profile.fee, "fee");
            Assert.AreEqual(0m, profile.tax, "tax");
            Assert.AreEqual(2000m, profile.mqty, "mqty");
            Assert.AreEqual(46050m, profile.mamt, "mamt");

            Assert.AreEqual(46050m, billSum.cnbamt, "cnbamt");
            Assert.AreEqual(0m, billSum.cnsamt, "cnsamt");
            Assert.AreEqual(65m, billSum.cnfee, "cnfee");
            Assert.AreEqual(0m, billSum.cntax, "cntax");
            Assert.AreEqual(-46115m, billSum.cnnetamt, "cnnetamt");
            Assert.AreEqual(2000m, billSum.bqty, "cnnetamt");
            Assert.AreEqual(0m, billSum.sqty, "cnnetamt");

            Assert.AreEqual(2, profile.profile.Count, "");
        }
    }
}
