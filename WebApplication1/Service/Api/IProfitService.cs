using Microsoft.Identity.Client;
using WebApplication1.Service.Dtos;
using WebApplication1.Common.HCN;

namespace WebApplication1.Service.Api
{
    public interface IProfitService
    {
        ProfitDetail GetProfitDetail(dynamic table);
        ProfitDetailOut GetProfitDetailOut(dynamic table);
        ProfitSum GetProfitSum(List<ProfitDetail> profitDetail, ProfitDetailOut profitDetailOut, string bhno, string cseq);
        List<ProfitSum> GetProfitSumList(List<dynamic> tables, string bhno, string cseq);
        ProfitAccsum GetProfittAccsum(List<ProfitSum> list);
        ProfitAccsum GetProfittAccsumFailed(string errorcode, string errormsg);
        Task<List<ExtendedHCNTD>> GetHCNTDList(string bhno, string cseq, string sdate, string edate, string stockSymbol);
        Task<List<ExtendedHCNRH>> GetHCNRHList(string bhno, string cseq, string sdate, string edate, string stockSymbol);
        Task<ProfitAccsum> GetProfitService(string bhno, string cseq, string sdate, string Edate, string stockSymbol);
    }
}
