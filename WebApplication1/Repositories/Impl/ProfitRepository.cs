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
    public class ProfitRepository : IProfitRepository
    {
        private readonly MyDbContext _context;

        public ProfitRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取出特定分公司特定帳號在開始日到結束日之間的歷史現股沖銷
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <param name="sdate">開始日</param>
        /// <param name="edate">結束日</param>
        /// <param name="stockSymbol">股票代碼</param>
        /// <returns>回傳開始日到結束日之間的歷史現股沖銷以及CNAME跟STOCK</returns>
        public async Task<IEnumerable<HCNRH>> GetByTwoKeyWithTimeForHCNRH(string bhno, string cseq, string sdate, string edate, string stockSymbol)
        {
            Logger.Log(1, "參數", $"sdate{string.Compare("20201228", sdate)}, edate{string.Compare("20201228", edate)}");
            var query = from hcnrh in InMemoryCache.HCNRHData
                        where hcnrh.CSEQ == cseq &&
                              hcnrh.BHNO == bhno &&
                              string.Compare(hcnrh.TDATE, sdate) >= 0 &&
                              string.Compare(hcnrh.TDATE, edate) <= 0 &&
                              (string.IsNullOrEmpty(stockSymbol) || hcnrh.STOCK == stockSymbol)
                        select new ExtendedHCNRH
                        {
                            STOCK = hcnrh.STOCK ?? string.Empty,
                            CSEQ = hcnrh.CSEQ ?? string.Empty,
                            BHNO = hcnrh.BHNO ?? string.Empty,
                            TDATE = hcnrh.TDATE ?? string.Empty,
                            RDATE = hcnrh.RDATE ?? string.Empty,
                            BDSEQ = hcnrh.BDSEQ ?? string.Empty,
                            SDSEQ = hcnrh.SDSEQ ?? string.Empty,
                            BDNO = hcnrh.BDNO ?? string.Empty,
                            SDNO = hcnrh.SDNO ?? string.Empty,
                            CQTY = hcnrh.CQTY ?? 0m,
                            BPRICE = hcnrh.BPRICE ?? 0m,
                            BFEE = hcnrh.BFEE ?? 0m,
                            SPRICE = hcnrh.SPRICE ?? 0m,
                            SFEE = hcnrh.SFEE ?? 0m,
                            TAX = hcnrh.TAX ?? 0m,
                            INCOME = hcnrh.INCOME ?? 0m,
                            COST = hcnrh.COST ?? 0m,
                            PROFIT = hcnrh.PROFIT ?? 0m,
                            BQTY = hcnrh.BQTY ?? 0m,
                            SQTY = hcnrh.SQTY ?? 0m,
                            TRDATE = hcnrh.TRDATE ?? string.Empty,
                            TRTIME = hcnrh.TRTIME ?? string.Empty,
                            MODDATE = hcnrh.MODDATE ?? string.Empty,
                            MODTIME = hcnrh.MODTIME ?? string.Empty,
                            MODUSER = hcnrh.MODUSER ?? string.Empty,
                            WTYPE = hcnrh.WTYPE ?? string.Empty,
                            STINTAX = hcnrh.STINTAX ?? 0m,
                            IOFLAG = hcnrh.IOFLAG ?? string.Empty,
                            ADJDATE = hcnrh.ADJDATE ?? string.Empty,
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
        public async Task<IEnumerable<HCNTD>> GetByTwoKeyWithTimeForHCNTD(string bhno, string cseq, string sdate, string edate, string stockSymbol)
        {
            var query = from hcntd in InMemoryCache.HCNTDData
                        where hcntd.CSEQ == cseq &&
                              hcntd.BHNO == bhno &&
                              string.Compare(hcntd.TDATE, sdate) >= 0 &&
                              string.Compare(hcntd.TDATE, edate) <= 0 &&
                              (string.IsNullOrEmpty(stockSymbol) || hcntd.STOCK == stockSymbol)
                        select new ExtendedHCNTD
                        {
                            STOCK = hcntd.STOCK ?? string.Empty,
                            CSEQ = hcntd.CSEQ ?? string.Empty,
                            BHNO = hcntd.BHNO ?? string.Empty,
                            TDATE = hcntd.TDATE ?? string.Empty,
                            BDSEQ = hcntd.BDSEQ ?? string.Empty,
                            SDSEQ = hcntd.SDSEQ ?? string.Empty,
                            BDNO = hcntd.BDNO ?? string.Empty,
                            SDNO = hcntd.SDNO ?? string.Empty,
                            CQTY = hcntd.CQTY ?? 0m,
                            BPRICE = hcntd.BPRICE ?? 0m,
                            BFEE = hcntd.BFEE ?? 0m,
                            SPRICE = hcntd.SPRICE ?? 0m,
                            SFEE = hcntd.SFEE ?? 0m,
                            TAX = hcntd.TAX ?? 0m,
                            INCOME = hcntd.INCOME ?? 0m,
                            COST = hcntd.COST ?? 0m,
                            PROFIT = hcntd.PROFIT ?? 0m,
                            BQTY = hcntd.BQTY ?? 0m,
                            SQTY = hcntd.SQTY ?? 0m,
                            TRDATE = hcntd.TRDATE ?? string.Empty,
                            TRTIME = hcntd.TRTIME ?? string.Empty,
                            MODDATE = hcntd.MODDATE ?? string.Empty,
                            MODTIME = hcntd.MODTIME ?? string.Empty,
                            MODUSER = hcntd.MODUSER ?? string.Empty,
                        };

            return await Task.FromResult(query.ToList());
        }
    }
}