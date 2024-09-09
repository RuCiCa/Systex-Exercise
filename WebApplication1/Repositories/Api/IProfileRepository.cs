// Repositories/IRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Common.HCN;
using WebApplication1.Service.Dtos;

namespace WebApplication1.Repositories.Api
{
    public interface IProfileRepository
    {
        Task<IEnumerable<HCMIO>> GetByTwoKeyWithTimeForHCMIO(string bhno, string cseq, string sdate, string edate, string stockSymbol);
        Task<IEnumerable<TMHIO>> GetByTwoKeyWithTimeForTMHIO(string bhno, string cseq, string sdate, string edate, string stockSymbol);
    }
}
