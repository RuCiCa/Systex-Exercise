// Repositories/Repository.cs
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using WebApplication1.Common;
using WebApplication1.Common.HCN;
using WebApplication1.Repositories.Api;
using WebApplication1.Service.Dtos;

namespace WebApplication1.Repositories.Impl
{
    public class UnOffsetRepository : IUnOffsetRepository
    {
        private readonly MyDbContext _context;

        public UnOffsetRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取出TCNUD跟MSTMB
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <param name="stockSymbol">股票代碼</param>
        /// <returns>回傳UnOffset，裡面包含TCNUD跟CNAME還有CPRICE</returns>
        /// <summary>
        /// 取出TCNUD跟MSTMB
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <param name="stockSymbol">股票代碼</param>
        /// <returns>回傳UnOffset，裡面包含TCNUD跟CNAME還有CPRICE</returns>
        public async Task<IEnumerable<TCNUD>> GetByTwoKey(string bhno, string cseq, string stockSymbol)
        {
            var query = from tcnud in InMemoryCache.TCNUDData
                        where tcnud.BHNO == bhno &&
                              tcnud.CSEQ == cseq &&
                              (string.IsNullOrEmpty(stockSymbol) || tcnud.STOCK == stockSymbol)
                        select tcnud;

            return await Task.FromResult(query.ToList());
        }
    }
}