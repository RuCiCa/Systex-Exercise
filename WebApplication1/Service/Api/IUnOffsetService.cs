using WebApplication1.Common;
using WebApplication1.Service.Dtos;


namespace WebApplication1.Service.Api
{
    public interface IUnOffsetService
    {
        List<ExtendedTCNUD> GetTCNUDList(List<TCNUD> tcnudList);
        List<ExtendedTCNUD> GetTMHIOList(List<TMHIO> tmhioList);
        UnOffsetDetail GetUnOffsetDetail(ExtendedTCNUD extendedTCNUD);
        List<UnOffsetDetail> GetUnOffsetDetailList(List<ExtendedTCNUD> list);
        UnOffsetSum GetUnOffsetSum(List<UnOffsetDetail> list);
        List<UnOffsetSum> GetUnOffsetSumList(List<UnOffsetDetail> list);
        UnOffsetAccsum GetUnOffsetAccsum(List<UnOffsetSum> list);
        UnOffsetAccsum GetFailedUnOffsetAccsum(string errcode, string errmsg);
        Task<UnOffsetAccsum> GetUnOffsetService(string bhno, string cseq, string stockSymbol);
    }
}
