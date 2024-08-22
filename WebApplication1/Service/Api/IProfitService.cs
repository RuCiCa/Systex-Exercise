using Microsoft.Identity.Client;
using WebApplication1.Service.Dtos;
using WebApplication1.Common.HCN;

namespace WebApplication1.Service.Api
{
    public interface IProfitService
    {
        Task<List<ProfitDetail>> GetProfitDetailList(List<dynamic> list);
        Task<List<ProfitDetailOut>> GetProfitDetailOutList(List<dynamic> list);
        ProfitSum GetProfitSum(ProfitDetailOut profitDetailOut, List<ProfitDetail> profitDetail, string bhno, string cseq);
        Task<List<ProfitDetailOut>> SumProfitDetailOut(List<ProfitDetailOut> list);
        Task<List<ProfitSum>> GetProfitSumList(string bhno, string cseq, List<ProfitDetailOut> profitDetailOuts, List<ProfitDetail> profitDetails);
        Task<ProfitAccsum> GetProfittAccsum(List<ProfitSum> list);
        Task<ProfitAccsum> GetProfittAccsumFailed(string errorcode, string errormsg);
        Task<List<ExtendedHCNTD>> GetHCNTDList(string bhno, string cseq, string sdate, string edate);
        Task<List<ExtendedHCNRH>> GetHCNRHList(string bhno, string cseq, string sdate, string edate);
    }
}
