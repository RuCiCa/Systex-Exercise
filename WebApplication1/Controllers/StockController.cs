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
                    try
                    {
                        //資料庫內尋找所有的交易紀錄，如果沒找到任何紀錄就會回傳404 Not Found
                        Logger.Log(0, "開始", $"開始搜尋{bhno}帳號{cseq}的交易紀錄");
                        var UnOffsetDetailList = await _unOffsetService.GetUnOffsetDetailList(bhno, cseq, stockSymbol);
                        if (UnOffsetDetailList == null)
                        {
                            return Ok(_unOffsetService.GetFailedUnOffsetAccsum("404", "未實現損益 – 個股明細獲取失敗"));
                        }
                        if (UnOffsetDetailList.Count == 0)
                        {
                            return Ok(_unOffsetService.GetFailedUnOffsetAccsum("404", $"未找到{bhno}帳號{cseq}的交易紀錄"));
                        }
                        var UnOffsetSumList = await _unOffsetService.GetUnOffsetSumList(UnOffsetDetailList);
                        if (UnOffsetSumList == null)
                        {
                            return Ok(_unOffsetService.GetFailedUnOffsetAccsum("404", "個股未實現損益獲取失敗"));
                        }
                        var response = await _unOffsetService.GetUnOffsetAccsum(UnOffsetSumList);
                        return Ok(response);
                    }
                    //過程中未防呆的部分都會抓到這邊
                    catch (Exception ex)
                    {
                        Logger.Log(4, "錯誤", $"搜尋{bhno}帳號{cseq}的交易紀錄時出現了錯誤：{ex}");
                        var response = await _unOffsetService.GetFailedUnOffsetAccsum("500", "Internal Server Error");
                        return Ok(response);
                    }
                case ("0002"):
                    try
                    {
                        Logger.Log(0, "開始", $"開始搜尋{bhno}帳號{cseq}的交易紀錄");
                        List<ExtendedHCNTD> HCNTDList = await _profitService.GetHCNTDList(bhno, cseq, request.sdate, request.Edate, stockSymbol);
                        List<ExtendedHCNRH> HCNRHList = await _profitService.GetHCNRHList(bhno, cseq, request.sdate, request.Edate, stockSymbol);

                        Logger.Log(1, "參數", $"HCNTD有{HCNTDList.Count()}筆、HCNRH有{HCNRHList.Count()}筆");
                        List<ProfitDetailOut> profitDetailOuts = new List<ProfitDetailOut>();
                        List<ProfitDetail> profitDetails = new List<ProfitDetail>();
                        List<ProfitSum> profitSumList = new List<ProfitSum>();

                        switch ((HCNTDList.Count() > 0, HCNRHList.Count() > 0))
                        {
                            case (true, true):
                                {
                                    List<ProfitDetailSet> HCNTDSet = await _profitService.GetProfitDetailSets(HCNTDList.Cast<dynamic>().ToList());
                                    List<ProfitDetailSet> HCNRHSet = await _profitService.GetProfitDetailSets(HCNRHList.Cast<dynamic>().ToList());
                                    List<ProfitSum> HCNTDSum = await _profitService.GetProfitSumList(bhno, cseq, HCNTDSet);
                                    List<ProfitSum> HCNRHSum = await _profitService.GetProfitSumList(bhno, cseq, HCNRHSet);
                                    profitSumList = HCNRHSum.Concat(HCNTDSum).ToList();

                                    break;
                                }
                            case (true, false):
                                {
                                    List<ProfitDetailSet> SetList = await _profitService.GetProfitDetailSets(HCNTDList.Cast<dynamic>().ToList());
                                    profitSumList = await _profitService.GetProfitSumList(bhno, cseq, SetList);
                                    break;
                                }
                            case (false, true):
                                {
                                    List<ProfitDetailSet> SetList = await _profitService.GetProfitDetailSets(HCNRHList.Cast<dynamic>().ToList());
                                    profitSumList = await _profitService.GetProfitSumList(bhno, cseq, SetList);
                                    break;
                                }
                            case (false, false):
                                {
                                    Logger.Log(3, "錯誤", $"未找到分公司{bhno}帳號{cseq}在{request.sdate}到{request.Edate}這段期間的交易紀錄");
                                    return Ok(await _profitService.GetProfittAccsumFailed("404", $"未找到分公司{bhno}帳號{cseq}在{request.sdate}到{request.Edate}這段期間的交易"));
                                }
                        }

                        var response = await _profitService.GetProfittAccsum(profitSumList);
                        if (response is null) 
                        {
                            return Ok(await _profitService.GetProfittAccsumFailed("500", $"獲取國內證券 已實現損益 帳戶彙總資料時出現錯誤"));
                        }

                        return Ok(response);
                    }
                    catch (Exception ex) 
                    {
                        Logger.Log(4, "錯誤", $"搜尋{bhno}帳號{cseq}的交易紀錄時出現了錯誤：{ex}");
                        var response = await _profitService.GetProfittAccsumFailed("500", "Internal Server Error");
                        return Ok(response);
                    }
                case ("0003"):
                    try
                    {
                        Logger.Log(0, "開始", $"開始搜尋{bhno}帳號{cseq}的交易紀錄");
                        List<ExtendedTMHIO> TMHIOList = await _profileService.GetTMHIOList(bhno, cseq, request.sdate, request.Edate, stockSymbol);
                        List<ExtendedHCMIO> HCMIOList = await _profileService.GetHCMIOList(bhno, cseq, request.sdate, request.Edate, stockSymbol);

                        Logger.Log(1, "參數", $"TMHIO有{TMHIOList.Count()}筆、HCMIO有{HCMIOList.Count()}筆");
                        List<Profile> profileList = new List<Profile>();
                        BillSum billSum = new BillSum();
                        ProfileSum profileSum = new ProfileSum();

                        switch ((TMHIOList.Count() > 0, HCMIOList.Count() > 0))
                        {
                            case (true, true):
                                {
                                    List<Profile> TMHIOProfileList = await _profileService.GetProfileList(TMHIOList.Cast<dynamic>().ToList());
                                    if (TMHIOProfileList is null)
                                    {
                                        return Ok(await _profileService.GetProfileSumFailed("500", $"獲取TMHIO對帳單 - 明細時出現錯誤"));
                                    }
                                    List<Profile> HCMIOProfileList = await _profileService.GetProfileList(HCMIOList.Cast<dynamic>().ToList());
                                    if (HCMIOProfileList is null)
                                    {
                                        return Ok(await _profileService.GetProfileSumFailed("500", $"獲取HCMIO對帳單 - 明細時出現錯誤"));
                                    }
                                    profileList = TMHIOProfileList.Concat(HCMIOProfileList).ToList();
                                    break;
                                }
                            case (true, false):
                                {
                                    profileList = await _profileService.GetProfileList(TMHIOList.Cast<dynamic>().ToList());
                                    if (profileList is null)
                                    {
                                        return Ok(await _profileService.GetProfileSumFailed("500", $"TMHIO獲取對帳單 - 明細時出現錯誤"));
                                    }
                                    break;
                                }
                            case (false, true):
                                {
                                    profileList = await _profileService.GetProfileList(HCMIOList.Cast<dynamic>().ToList());
                                    if (profileList is null)
                                    {
                                        return Ok(await _profileService.GetProfileSumFailed("500", $"獲取HCMIO對帳單 - 明細時出現錯誤"));
                                    }
                                    break;
                                }
                            case (false, false):
                                {
                                    Logger.Log(3, "錯誤", $"未找到分公司{bhno}帳號{cseq}在{request.sdate}到{request.Edate}這段期間的交易紀錄");
                                    return Ok(await _profileService.GetProfileSumFailed("404", $"未找到分公司{bhno}帳號{cseq}在{request.sdate}到{request.Edate}這段期間的交易"));
                                }
                        }
                        billSum = await _profileService.GetBillSum(profileList);
                        if (billSum is null)
                        {
                            return Ok(await _profileService.GetProfileSumFailed("500", $"獲取對帳單匯總資料時出現錯誤"));
                        }

                        profileSum = await _profileService.GetProfileSum(billSum, profileList);
                        if (profileSum is null)
                        {
                            return Ok(await _profileService.GetProfileSumFailed("500", $"獲取對帳單 - 彙總資料時出現錯誤"));
                        }

                        var response = profileSum;
                        return Ok(response);
                    }
                    catch
                    {
                        var response = await _profileService.GetProfileSumFailed("500", "Internal Server Error");
                        return Ok(response);
                    }
                default:
                    return Ok(await _errorService.GetErrorReturn("403", "不能使用的qtype"));
            }


        }

    }
}
