// Repositories/IRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Common.HCN;
using WebApplication1.Service.Dtos;

namespace WebApplication1.Repositories.Api
{
    public interface IUnOffsetRepository
    {
        Task<IEnumerable<TCNUD>> GetByTwoKey(string cseq, string bhno, string stockSymbol);
    }
}
