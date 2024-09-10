using Microsoft.Identity.Client;
using WebApplication1.Service.Dtos;
using WebApplication1.Common;
using WebApplication1.service.dtos;

namespace WebApplication1.Service.Api
{
    public interface IProfileService
    {
        List<Profile> GetProfileList(List<dynamic> list);
        BillSum GetBillSum(List<Profile> list);
        ProfileSum GetProfileSum(BillSum bill, List<Profile> list);
        ProfileSum GetProfileSumFailed(string errcode, string errmsg);
        List<ExtendedTMHIO> GetTMHIOList(string bhno, string cseq, string sdate, string edate, string stockSymbol);
        List<ExtendedHCMIO> GetHCMIOList(string bhno, string cseq, string sdate, string edate, string stockSymbol);
        Task<ProfileSum> GetProfileService(string bhno, string cseq, string sdate, string Edate, string stockSymbol);
    }
}
