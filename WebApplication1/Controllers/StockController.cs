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


        //�Ψ�����(�����q�W��)(�b��)��o�ꤺ�Ҩ�-����{�l�q �b���J�`
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
                        //��Ʈw���M��Ҧ�����������A�p�G�S����������N�|�^��404 Not Found
                        Logger.Log(0, "�}�l", $"�}�l�j�M{bhno}�b��{cseq}���������");
                        var UnOffsetDetailList = await _unOffsetService.GetUnOffsetDetailList(bhno, cseq);
                        if (UnOffsetDetailList == null)
                        {
                            return Ok(_unOffsetService.GetFailedUnOffsetAccsum("404", "����{�l�q �V �Ӫѩ����������"));
                        }
                        if (UnOffsetDetailList.Count == 0)
                        {
                            return Ok(_unOffsetService.GetFailedUnOffsetAccsum("404", $"�����{bhno}�b��{cseq}���������"));
                        }
                        var UnOffsetSumList = await _unOffsetService.GetUnOffsetSumList(UnOffsetDetailList);
                        if (UnOffsetSumList == null)
                        {
                            return Ok(_unOffsetService.GetFailedUnOffsetAccsum("404", "�Ӫѥ���{�l�q�������"));
                        }
                        var response = await _unOffsetService.GetUnOffsetAccsum(UnOffsetSumList);
                        return Ok(response);
                    }
                    //�L�{�������b���������|���o��
                    catch (Exception ex)
                    {
                        Logger.Log(4, "���~", $"�j�M{bhno}�b��{cseq}����������ɥX�{�F���~�G{ex}");
                        var response = await _unOffsetService.GetFailedUnOffsetAccsum("500", "Internal Server Error");
                        return Ok(response);
                    }
                case ("0002"):
                    try
                    {
                        Logger.Log(0, "�}�l", $"�}�l�j�M{bhno}�b��{cseq}���������");
                        List<ExtendedHCNTD> HCNTDList = await _profitService.GetHCNTDList(bhno, cseq, request.sdate, request.Edate);
                        List<ExtendedHCNRH> HCNRHList = await _profitService.GetHCNRHList(bhno, cseq, request.sdate, request.Edate);

                        Logger.Log(1, "�Ѽ�", $"HCNTD��{HCNTDList.Count()}���BHCNRH��{HCNRHList.Count()}��");
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
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"�������{�l�q - �Ӫѩ��Ӹ�ƮɥX�{���D"));
                                    }
                                    profitDetails = HCNTDDetails.Concat(HCNRHDetails).ToList();

                                    List<ProfitDetailOut> HCNTDDetailOuts = await _profitService.GetProfitDetailOutList(HCNTDList.Cast<dynamic>().ToList());
                                    List<ProfitDetailOut> HCNRHDetailOuts = await _profitService.GetProfitDetailOutList(HCNRHList.Cast<dynamic>().ToList());
                                    if (HCNTDDetailOuts is null || HCNRHDetailOuts is null)
                                    {
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"����w��{�l�q - �Ӫѩ��Ӹ�ƮɥX�{���D"));
                                    }
                                    profitDetailOuts = HCNTDDetailOuts.Concat(HCNRHDetailOuts).ToList();
                                    break;
                                }
                            case (true, false):
                                {
                                    profitDetails = await _profitService.GetProfitDetailList(HCNTDList.Cast<dynamic>().ToList());
                                    if (profitDetails is null)
                                    {
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"�������{�l�q - �Ӫѩ��Ӹ�ƮɥX�{���D"));
                                    }
                                    profitDetailOuts = await _profitService.GetProfitDetailOutList(HCNTDList.Cast<dynamic>().ToList());
                                    if (profitDetailOuts is null)
                                    {
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"����w��{�l�q - �Ӫѩ��Ӹ�ƮɥX�{���D"));
                                    }
                                    break;
                                }
                            case (false, true):
                                {
                                    profitDetails = await _profitService.GetProfitDetailList(HCNRHList.Cast<dynamic>().ToList());
                                    if (profitDetails is null)
                                    {
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"�������{�l�q - �Ӫѩ��Ӹ�ƮɥX�{���D"));
                                    }
                                    profitDetailOuts = await _profitService.GetProfitDetailOutList(HCNRHList.Cast<dynamic>().ToList());
                                    if (profitDetailOuts is null)
                                    {
                                        return Ok(await _profitService.GetProfittAccsumFailed("500", $"����w��{�l�q - �Ӫѩ��Ӹ�ƮɥX�{���D"));
                                    }
                                    break;
                                }
                            case (false, false):
                                {
                                    Logger.Log(3, "���~", $"���������q{bhno}�b��{cseq}�b{request.sdate}��{request.Edate}�o�q�������������");
                                    return Ok(await _profitService.GetProfittAccsumFailed("404", $"���������q{bhno}�b��{cseq}�b{request.sdate}��{request.Edate}�o�q���������"));
                                }
                        }


                        profitDetailOuts = await _profitService.SumProfitDetailOut(profitDetailOuts);
                        if (profitDetailOuts is null)
                        {
                            return Ok(await _profitService.GetProfittAccsumFailed("500", $"�[�`�w��{�l�q�ɥX�{���~"));
                        }

                        var profitSumList = await _profitService.GetProfitSumList(bhno, cseq, profitDetailOuts, profitDetails);
                        if (profitDetailOuts is null)
                        {
                            return Ok(await _profitService.GetProfittAccsumFailed("500", $"����w��{�l�q-�ӪѷJ�`��ƮɥX�{���~"));
                        }

                        var response = await _profitService.GetProfittAccsum(profitSumList);
                        if (response is null) 
                        {
                            return Ok(await _profitService.GetProfittAccsumFailed("500", $"����ꤺ�Ҩ� �w��{�l�q �b��J�`��ƮɥX�{���~"));
                        }

                        return Ok(response);
                    }
                    catch (Exception ex) 
                    {
                        Logger.Log(4, "���~", $"�j�M{bhno}�b��{cseq}����������ɥX�{�F���~�G{ex}");
                        var response = await _profitService.GetProfittAccsumFailed("500", "Internal Server Error");
                        return Ok(response);
                    }
                default:
                    return Ok(await _errorService.GetErrorReturn("403", "����ϥΪ�qtype"));
            }


        }

    }
}
