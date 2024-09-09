using WebApplication1.Common;
using WebApplication1.Service.Dtos;


namespace WebApplication1.Service.Api
{
    public interface IUnOffsetService
    {
        Task<List<UnOffset>> GetUnOffsetList(string bhno, string cseq, string stockSymbol);
        UnOffsetDetail GetUnOffsetDetail(UnOffset unOffset);
        Task<List<UnOffsetDetail>> GetUnOffsetDetailList(List<UnOffset> list);
        UnOffsetSum GetUnOffsetSum(List<UnOffsetDetail> list);
        Task<List<UnOffsetSum>> GetUnOffsetSumList(List<UnOffsetDetail> list);
        Task<UnOffsetAccsum> GetUnOffsetAccsum(List<UnOffsetSum> list);
        Task<UnOffsetAccsum> GetFailedUnOffsetAccsum(string errcode, string errmsg);
    }
}
