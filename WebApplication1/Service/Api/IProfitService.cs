using Microsoft.Identity.Client;
using WebApplication1.Service.Dtos;
using WebApplication1.Common.HCN;

namespace WebApplication1.Service.Api
{
    public interface IProfitService
    {
        ProfitDetail GetProfitDetail(dynamic table);
        ProfitDetailOut GetProfitDetailOut(dynamic table);
        ProfitSum GetProfitSum(ProfitDetailSet profitDetailSet, string bhno, string cseq);
        Task<List<ProfitDetailOut>> SumProfitDetailOut(List<ProfitDetailOut> list);
        Task<List<ProfitDetailSet>> GetProfitDetailSets(List<dynamic> tables);
        Task<List<ProfitSum>> GetProfitSumList(string bhno, string cseq, List<ProfitDetailSet> profitDetailSets);
        Task<ProfitAccsum> GetProfittAccsum(List<ProfitSum> list);
        Task<ProfitAccsum> GetProfittAccsumFailed(string errorcode, string errormsg);
        Task<List<ExtendedHCNTD>> GetHCNTDList(string bhno, string cseq, string sdate, string edate);
        Task<List<ExtendedHCNRH>> GetHCNRHList(string bhno, string cseq, string sdate, string edate);
    }
}
