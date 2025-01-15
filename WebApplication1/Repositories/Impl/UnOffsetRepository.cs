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
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication1.Repositories.Impl
{
    public class UnOffsetRepository : IUnOffsetRepository
    {
        private readonly MyDbContext _context;
        private readonly Util _util;
        private readonly InMemoryCache _inMemoryCache;

        public UnOffsetRepository(MyDbContext context, Util util, InMemoryCache inMemoryCache)
        {
            _inMemoryCache = inMemoryCache;
            _util = util;
            _context = context;
        }

        /// <summary>
        /// 取出TCNUD跟MSTMB
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <param name="stockSymbol">股票代碼</param>
        /// <returns>回傳UnOffset，裡面包含TCNUD跟CNAME還有CPRICE</returns>
        public async Task<IEnumerable<TCNUD>> GetByTwoKey(string bhno, string cseq, string stockSymbol)
        {
            var query = from tcnud in _context.TCNUDTable
                        where tcnud.BHNO == bhno &&
                              tcnud.CSEQ == cseq &&
                              (string.IsNullOrEmpty(stockSymbol) || tcnud.STOCK == stockSymbol)
                        select new TCNUD
                        {
                            TDATE = tcnud.TDATE,
                            BHNO = tcnud.BHNO,
                            CSEQ = tcnud.CSEQ,
                            STOCK = tcnud.STOCK,
                            PRICE = tcnud.PRICE,
                            QTY = tcnud.QTY,
                            BQTY = tcnud.BQTY,
                            FEE = tcnud.FEE,
                            COST = tcnud.COST,
                            DSEQ = tcnud.DSEQ,
                            DNO = tcnud.DNO,
                            ADJDATE = tcnud.ADJDATE,
                            WTYPE = tcnud.WTYPE,                                                   
                            TRDATE = tcnud.TRDATE,
                            TRTIME = tcnud.TRTIME,
                            MODATE = tcnud.MODATE,
                            MODTIME = tcnud.MODTIME,
                            MODUSER = tcnud.MODUSER,
                            IOFLAG = tcnud.IOFLAG,
                            AMT = 0m,
                            ETYPE = ""
                        };

            return await Task.FromResult(query.ToList());
        }
        /// <summary>
        /// 取出TCNUD跟MSTMB
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <param name="stockSymbol">股票代碼</param>
        /// <returns>回傳UnOffset，裡面包含TCNUD跟CNAME還有CPRICE</returns>
        public async Task<IEnumerable<TMHIO>> GetByTwoKeyTMHIO(string bhno, string cseq, string stockSymbol)
        {
            var query = from tmhio in _context.TMHIOTable
                        where tmhio.BHNO == bhno &&
                              tmhio.CSEQ == cseq &&
                              (string.IsNullOrEmpty(stockSymbol) || tmhio.STOCK == stockSymbol)
                        select new TMHIO
                        {
                            TDATE = tmhio.TDATE ?? string.Empty,
                            BHNO = tmhio.BHNO ?? string.Empty,
                            DSEQ = tmhio.DSEQ ?? string.Empty,
                            JRNUM = tmhio.JRNUM ?? string.Empty,
                            MTYPE = tmhio.MTYPE ?? string.Empty,
                            CSEQ = tmhio.CSEQ ?? string.Empty,
                            TTYPE = tmhio.TTYPE ?? string.Empty,
                            ETYPE = _util.TransEtpye(tmhio.ETYPE) ?? string.Empty,
                            BSTYPE = tmhio.BSTYPE ?? string.Empty,
                            STOCK = tmhio.STOCK ?? string.Empty,
                            QTY = tmhio.QTY,
                            PRICE = tmhio.PRICE,
                            SALES = tmhio.SALES ?? string.Empty,
                            ORIGN = tmhio.ORIGN ?? string.Empty,
                            MTIME = tmhio.MTIME ?? string.Empty,
                            TRDATE = tmhio.TRDATE ?? string.Empty,
                            TRTIME = tmhio.TRTIME ?? string.Empty,
                            MODDATE = tmhio.MODDATE ?? string.Empty,
                            MODTIME = tmhio.MODTIME ?? string.Empty,
                            MODUSER = tmhio.MODUSER ?? string.Empty,
                            AMT = _util.CalcMamt(tmhio.PRICE, tmhio.QTY, 0)
                        };

            return await Task.FromResult(query.ToList());
        }

        /// <summary>
        /// 取出TCNUD跟MSTMB
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <param name="stockSymbol">股票代碼</param>
        /// <returns>回傳UnOffset，裡面包含TCNUD跟CNAME還有CPRICE</returns>
        public async Task<IEnumerable<TCSIO>> GetByTwoKeyTCSIO(string bhno, string cseq, string stockSymbol)
        {
            var query = from tcsio in _context.TCSIOTable
                        where tcsio.BHNO == bhno &&
                              tcsio.CSEQ == cseq &&
                              (string.IsNullOrEmpty(stockSymbol) || tcsio.STOCK == stockSymbol)
                        select new TCSIO
                        {
                            TDATE = tcsio.TDATE ?? string.Empty,
                            BHNO = tcsio.BHNO ?? string.Empty,
                            DSEQ = tcsio.DSEQ ?? string.Empty,
                            CSEQ = tcsio.CSEQ ?? string.Empty,
                            STOCK = tcsio.STOCK ?? string.Empty,
                            BSTYPE = tcsio.BSTYPE ?? string.Empty,
                            QTY = tcsio.QTY ?? 0m,
                            IOFLAG = tcsio.IOFLAG ?? string.Empty,
                            REMARK = tcsio.REMARK ?? string.Empty,
                            JRNUM = tcsio.JRNUM ?? string.Empty,
                            TRDATE = tcsio.TRDATE ?? string.Empty,
                            TRTIME = tcsio.TRTIME ?? string.Empty,
                            MODDATE = tcsio.MODDATE ?? string.Empty,
                            MODTIME = tcsio.MODTIME ?? string.Empty,
                            MODUSER = tcsio.MODUSER ?? string.Empty
                        };

            return await Task.FromResult(query.ToList());
        }
}
}    