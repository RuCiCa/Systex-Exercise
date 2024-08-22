using WebApplication1.Common;
using WebApplication1.Service.Dtos;


namespace WebApplication1.Service.Api
{
    public interface IUnOffsetService
    {
        UnOffsetDetail GetUnOffsetDetail(UnOffset unOffset);
        Task<List<UnOffsetDetail>> GetUnOffsetDetailList(string bhno, string cseq);
        UnOffsetSum GetUnOffsetSum(List<UnOffsetDetail> list);
        Task<List<UnOffsetSum>> GetUnOffsetSumList(List<UnOffsetDetail> list);
        Task<UnOffsetAccsum> GetUnOffsetAccsum(List<UnOffsetSum> list);
        Task<UnOffsetAccsum> GetFailedUnOffsetAccsum(string errcode, string errmsg);
    }
}
