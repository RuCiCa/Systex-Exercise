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
    public class ProfileRepository : IProfileRepository
    {
        private readonly MyDbContext _context;

        public ProfileRepository(MyDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 取出特定分公司特定帳號在開始日到結束日之間TMHIO
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <param name="sdate">開始日</param>
        /// <param name="edate">結束日</param>
        /// <param name="stockSymbol">股票代碼</param>
        /// <returns>回傳開始日到結束日之間的TMHIO以及CNAME</returns>
        public async Task<IEnumerable<TMHIO>> GetByTwoKeyWithTimeForTMHIO(string bhno, string cseq, string sdate, string edate, string stockSymbol)
        {
            var query = from tmhio in InMemoryCache.TMHIOData
                        where tmhio.CSEQ == cseq &&
                              tmhio.BHNO == bhno &&
                              string.Compare(tmhio.TDATE, sdate) >= 0 &&
                              string.Compare(tmhio.TDATE, edate) <= 0 &&
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
                            ETYPE = tmhio.ETYPE ?? string.Empty,
                            BSTYPE = tmhio.BSTYPE ?? string.Empty,
                            STOCK = tmhio.STOCK ?? string.Empty,
                            QTY = tmhio.QTY ?? 0m,
                            PRICE = tmhio.PRICE,
                            SALES = tmhio.SALES ?? string.Empty,
                            ORIGN = tmhio.ORIGN ?? string.Empty,
                            MTIME = tmhio.MTIME ?? string.Empty,
                            TRDATE = tmhio.TRDATE ?? string.Empty,
                            TRTIME = tmhio.TRTIME ?? string.Empty,
                            MODDATE = tmhio.MODDATE ?? string.Empty,
                            MODTIME = tmhio.MODTIME ?? string.Empty,
                            MODUSER = tmhio.MODUSER ?? string.Empty,
                        };

            return await Task.FromResult(query.ToList());
        }


        /// <summary>
        /// 取出特定分公司特定帳號在開始日到結束日之間的歷史現股當沖
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <param name="sdate">開始日</param>
        /// <param name="edate">結束日</param>
        /// <param name="stockSymbol">股票代碼</param>
        /// <returns>回傳開始日到結束日之間的歷史現股當沖以及CNAME跟STOCK</returns>
        public async Task<IEnumerable<HCMIO>> GetByTwoKeyWithTimeForHCMIO(string bhno, string cseq, string sdate, string edate, string stockSymbol)
        {
            var query = from hcmio in InMemoryCache.HCMIOData
                        where hcmio.CSEQ == cseq &&
                              hcmio.BHNO == bhno &&
                              string.Compare(hcmio.TDATE, sdate) >= 0 &&
                              string.Compare(hcmio.TDATE, edate) <= 0 &&
                              (string.IsNullOrEmpty(stockSymbol) || hcmio.STOCK == stockSymbol)
                        select new HCMIO
                        {
                            TDATE = hcmio.TDATE ?? string.Empty,
                            BHNO = hcmio.BHNO ?? string.Empty,
                            CSEQ = hcmio.CSEQ ?? string.Empty,
                            DSEQ = hcmio.DSEQ ?? string.Empty,
                            DNO = hcmio.DNO ?? string.Empty,
                            WTYPE = hcmio.WTYPE ?? string.Empty,
                            STOCK = hcmio.STOCK ?? string.Empty,
                            TTYPE = hcmio.TTYPE ?? string.Empty,
                            ETYPE = hcmio.ETYPE ?? string.Empty,
                            BSTYPE = hcmio.BSTYPE ?? string.Empty,
                            PRICE = hcmio.PRICE ?? 0m,
                            QTY = hcmio.QTY ?? 0m,
                            FEE = hcmio.FEE ?? 0m,
                            AMT = hcmio.AMT ?? 0m,
                            TAX = hcmio.TAX ?? 0m,
                            RVINT = hcmio.RVINT ?? 0m,
                            NETAMT = hcmio.NETAMT ?? 0m,
                            DBFEE = hcmio.DBFEE ?? 0m,
                            CRAMT = hcmio.CRAMT ?? 0m,
                            DNAMT = hcmio.DNAMT ?? 0m,
                            CRINT = hcmio.CRINT ?? 0m,
                            DNINT = hcmio.DNINT ?? 0m,
                            DLFEE = hcmio.DLFEE ?? 0m,
                            BFINT = hcmio.BFINT ?? 0m,
                            OBAMT = hcmio.OBAMT ?? 0m,
                            INTAX = hcmio.INTAX ?? 0m,
                            SFCODE = hcmio.SFCODE ?? string.Empty,
                            CDTDQTY = hcmio.CDTDQTY ?? 0m,
                            ORIGN = hcmio.ORIGN ?? string.Empty,
                            SALES = hcmio.SALES ?? string.Empty,
                            DATAFLAG = hcmio.DATAFLAG ?? string.Empty,
                            IOFLAG = hcmio.IOFLAG ?? string.Empty,
                            ADJCOST = hcmio.ADJCOST ?? 0m,
                            ADJDATE = hcmio.ADJDATE ?? string.Empty,
                            STINTAX = hcmio.STINTAX ?? 0m,
                            HEALTHFEE = hcmio.HEALTHFEE ?? 0m,
                            TRDATE = hcmio.TRDATE ?? string.Empty,
                            TRTIME = hcmio.TRTIME ?? string.Empty,
                            MODDATE = hcmio.MODDATE ?? string.Empty,
                            MODTIME = hcmio.MODTIME ?? string.Empty,
                            MODUSER = hcmio.MODUSER ?? string.Empty,
                        };

            return await Task.FromResult(query.ToList());
        }
    }
}