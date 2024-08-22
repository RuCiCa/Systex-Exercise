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

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        
        private readonly IUnOffsetService _unOffsetService;
        private readonly IProfitService _profitService;
        private readonly IErrorService _errorService;

        public StockController(IUnOffsetService unOffsetService, IProfitService profitService, IErrorService errorService)
        {
            _unOffsetService = unOffsetService;
            _profitService = profitService;
            _errorService = errorService;
        }


        //用兩個鍵位(分公司名稱)(帳戶)獲得國內證券-未實現損益 帳號彙總
        [HttpPost("UnrealizedGainOrLoss")]
        public async Task<ActionResult<dynamic>> GetUnrealizedGainOrLoss([FromBody] Request request)
        {
            string bhno = request.BHNO;
            string cseq = request.CSEQ;
            string qtype = request.qtype;
            switch (qtype)
            {
                case ("0001"):
                    try
                    {
                        //資料庫內尋找所有的交易紀錄，如果沒找到任何紀錄就會回傳404 Not Found
                        Logger.Log(0, "開始", $"開始搜尋{bhno}帳號{cseq}的交易紀錄");
                        var UnOffsetDetailList = await _unOffsetService.GetUnOffsetDetailList(bhno, cseq);
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
                        List<ExtendedHCNTD> HCNTDList = await _profitService.GetHCNTDList(bhno, cseq, request.sdate, request.Edate);
                        List<ExtendedHCNRH> HCNRHList = await _profitService.GetHCNRHList(bhno, cseq, request.sdate, request.Edate);

                        Logger.Log(1, "參數", $"HCNTD有{HCNTDList.Count()}筆、HCNRH有{HCNRHList.Count()}筆");
                        List<ProfitDetailOut> profitDetailOuts = new List<ProfitDetailOut>();
                        List<ProfitDetail> profitDetails = new List<ProfitDetail>();

                        switch ((HCNTDList.Count() > 0, HCNRHList.Count() > 0))
                        {
                            case (true, true):
                                {
                                    List<ProfitDetail> HCNTDDetails = await _profitService.GetProfitDetailList(HCNTDList.Cast<dynamic>().ToList());
                                    List<ProfitDetail> HCNRHDetails = await _profitService.GetProfitDetailList(HCNRHList.Cast<dynamic>().ToList());
                                    if (HCNTDDetails is null || HCNRHDetails is null)
                                    {
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"獲取未實現損益 - 個股明細資料時出現問題"));
                                    }
                                    profitDetails = HCNTDDetails.Concat(HCNRHDetails).ToList();

                                    List<ProfitDetailOut> HCNTDDetailOuts = await _profitService.GetProfitDetailOutList(HCNTDList.Cast<dynamic>().ToList());
                                    List<ProfitDetailOut> HCNRHDetailOuts = await _profitService.GetProfitDetailOutList(HCNRHList.Cast<dynamic>().ToList());
                                    if (HCNTDDetailOuts is null || HCNRHDetailOuts is null)
                                    {
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"獲取已實現損益 - 個股明細資料時出現問題"));
                                    }
                                    profitDetailOuts = HCNTDDetailOuts.Concat(HCNRHDetailOuts).ToList();
                                    break;
                                }
                            case (true, false):
                                {
                                    profitDetails = await _profitService.GetProfitDetailList(HCNTDList.Cast<dynamic>().ToList());
                                    if (profitDetails is null)
                                    {
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"獲取未實現損益 - 個股明細資料時出現問題"));
                                    }
                                    profitDetailOuts = await _profitService.GetProfitDetailOutList(HCNTDList.Cast<dynamic>().ToList());
                                    if (profitDetailOuts is null)
                                    {
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"獲取已實現損益 - 個股明細資料時出現問題"));
                                    }
                                    break;
                                }
                            case (false, true):
                                {
                                    profitDetails = await _profitService.GetProfitDetailList(HCNRHList.Cast<dynamic>().ToList());
                                    if (profitDetails is null)
                                    {
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"獲取未實現損益 - 個股明細資料時出現問題"));
                                    }
                                    profitDetailOuts = await _profitService.GetProfitDetailOutList(HCNRHList.Cast<dynamic>().ToList());
                                    if (profitDetailOuts is null)
                                    {
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"獲取已實現損益 - 個股明細資料時出現問題"));
                                    }
                                    break;
                                }
                            case (false, false):
                                {
                                    Logger.Log(3, "錯誤", $"未找到分公司{bhno}帳號{cseq}在{request.sdate}到{request.Edate}這段期間的交易紀錄");
                                    return Ok(await _profitService.GetProfittAccsumFailed("404", $"未找到分公司{bhno}帳號{cseq}在{request.sdate}到{request.Edate}這段期間的交易"));
                                }
                        }


                        profitDetailOuts = await _profitService.SumProfitDetailOut(profitDetailOuts);
                        if (profitDetailOuts is null)
                        {
                            return Ok(await _profitService.GetProfittAccsumFailed("500", $"加總已實現損益時出現錯誤"));
                        }

                        var profitSumList = await _profitService.GetProfitSumList(bhno, cseq, profitDetailOuts, profitDetails);
                        if (profitDetailOuts is null)
                        {
                            return Ok(await _profitService.GetProfittAccsumFailed("500", $"獲取已實現損益-個股彙總資料時出現錯誤"));
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
                default:
                    return Ok(await _errorService.GetErrorReturn("403", "不能使用的qtype"));
            }


        }

    }
}
