using Microsoft.Identity.Client;
using WebApplication1.Service.Dtos;
using WebApplication1.Common;
using WebApplication1.service.dtos;

namespace WebApplication1.Service.Api
{
    public interface IProfileService
    {
        Task<List<Profile>> GetProfileList(List<dynamic> list);
        Task<BillSum> GetBillSum(List<dynamic> list);
        Task<ProfileSum> GetProfileSum(BillSum bill, List<Profile> profiles);
        Task<ProfileSum> GetProfileSumFailed(string errorcode, string errormsg);
        Task<List<ExtendedTMHIO>> GetTMHIOList(string bhno, string cseq, string sdate, string edate, string stockSymbol);
        Task<List<ExtendedHCMIO>> GetHCMIOList(string bhno, string cseq, string sdate, string edate, string stockSymbol);
    }
}
