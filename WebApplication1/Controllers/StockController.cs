using Autofac.Core;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using WebApplication1.Common;
using WebApplication1.Service.Dtos;
using WebApplication1.Service.Api;
using WebApplication1.Repositories.Api;
using WebApplication1.Common.HCN;
using WebApplication1.Repositories.Impl;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Service.Impl;
using WebApplication1.service.dtos;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        
        private readonly IUnOffsetService _unOffsetService;
        private readonly IProfitService _profitService;
        private readonly IProfileService _profileService;
        private readonly IErrorService _errorService;

        public StockController(IUnOffsetService unOffsetService, IProfitService profitService, IProfileService profileService, IErrorService errorService)
        {
            _unOffsetService = unOffsetService;
            _profitService = profitService;
            _profileService = profileService;
            _errorService = errorService;
        }


        //用兩個鍵位(分公司名稱)(帳戶)獲得國內證券-未實現損益 帳號彙總
        [HttpPost("UnrealizedGainOrLoss")]
        public async Task<ActionResult<dynamic>> GetUnrealizedGainOrLoss([FromBody] Request request)
        {
            string bhno = request.BHNO;
            string cseq = request.CSEQ;
            string qtype = request.qtype;
            string stockSymbol = request.stockSymbol;
            switch (qtype)
            {
                case ("0001"):
                    var unOffsetResponse = await _unOffsetService.GetUnOffsetService(bhno, cseq, stockSymbol);
                    return Ok(unOffsetResponse);
                case ("0002"):
                    var profitResponse = await _profitService.GetProfitService(bhno, cseq, request.sdate, request.Edate, stockSymbol);
                    return Ok(profitResponse);
                case ("0003"):
                    var profileResponse = await _profileService.GetProfileService(bhno, cseq, request.sdate, request.Edate, stockSymbol);
                    return Ok(profileResponse);
                default:
                    return Ok(await _errorService.GetErrorReturn("403", "不能使用的qtype"));
            }


        }

    }
}
