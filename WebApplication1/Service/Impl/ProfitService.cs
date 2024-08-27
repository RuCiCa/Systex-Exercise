using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Net;
using WebApplication1.Common;
using WebApplication1.Common.HCN;
using WebApplication1.Repositories.Api;
using WebApplication1.Service.Api;
using WebApplication1.Service.Dtos;

namespace WebApplication1.Service.Impl
{
    public class ProfitService : IProfitService
    {
        private readonly MyDbContext _context;
        private readonly ProfitAccsum _profitAccsum;
        private readonly IRepository _repository;


        public ProfitService(MyDbContext context, ProfitAccsum profitAccsum, IRepository repository)
        {
            _context = context;
            _profitAccsum = profitAccsum;
            _repository = repository;
        }


        /// <summary>
        /// 獲取所有的買入個股明細，並依據類型存入不同的資料
        /// </summary>
        /// <param name="tables">HCNTD或HCNRH組成的List</param>
        /// <returns>成功會回傳list，用來保存ProfitDetail</returns>
        public ProfitDetail GetProfitDetail(dynamic table)
        {
            try
            {
                string tableType = table is ExtendedHCNRH ? "HCNRH" : "HCNTD";
                string stock = table.STOCK;
                string tdate = table is ExtendedHCNRH ? table.RDATE : table.TDATE;
                string dseq = table.BDSEQ;
                string dno = table.BDNO;
                Logger.Log(1, "參數", $"未實現損益-個股明細資料 (買入) - 資料庫： {tableType}, stock： {stock}, tdate： {tdate}, dseq： {dseq}, dno： {dno}");

                decimal cost = table.COST ?? 0m;
                decimal profitVal = table.COST ?? 0m;
                decimal pl_ratio = 0m;

                if (cost != 0m)
                {
                    pl_ratio = profitVal / cost * 100;
                }

                ProfitDetail profitDetail = new ProfitDetail()
                {
                    stock = stock,
                    stocknm = table.CNAME,
                    tdate = tdate,
                    dseq = dseq,
                    dno = dno,
                    mqty = table.BQTY ?? 0m,
                    cqty = table.CQTY ?? 0m,
                    mprice = table.BPRICE.ToString(),
                    mamt = (table.CQTY * table.BPRICE).ToString(),
                    cost = cost,
                    income = table.INCOME ?? 0m,
                    netamt = -(table.COST ?? 0m),
                    fee = table.BFEE ?? 0m,
                    tax = 0m,
                    ttype = "0",
                    ttypename = "現買",
                    bstype = "B",
                    wtype = table is ExtendedHCNRH ? table.WTYPE : "0",
                    profit = profitVal,
                    pl_ratio = pl_ratio.ToString() + "%",
                    ctype = "0",
                    ttypename2 = "現買",
                    ioflag = table is ExtendedHCNRH ? table.IOFLAG : "0",
                    ioname = "",
                    adjdate = table is ExtendedHCNRH ? table.ADJDATE : "",
                };
                return profitDetail;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "失敗", $"未實現損益-個股明細資料 (買入)獲取失敗，錯誤訊息：{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 獲取所有的買入個股明細，並依據類型存入不同的資料
        /// </summary>
        /// <param name="table">HCNTD或HCNRH的表</param>
        /// <returns>成功會回傳list，用來保存ProfitDetailOut</returns>
        public ProfitDetailOut GetProfitDetailOut(dynamic table)
        {
            try
            {
                string tableType = table is ExtendedHCNRH ? "HCNRH" : "HCNTD";
                string stock = table.STOCK;
                string tdate = table is ExtendedHCNRH ? table.RDATE : table.TDATE;
                string dseq = table.SDSEQ;
                string dno = table.SDNO;
                Logger.Log(1, "參數", $"已實現損益 - 個股明細資料 (賣出) - 資料庫： {tableType}, stock： {stock}, tdate： {tdate}, dseq： {dseq}, dno： {dno}");

                decimal cost = table.COST ?? 0m;
                decimal profitVal = table.COST ?? 0m;
                decimal pl_ratio = 0m;

                if (cost != 0m)
                {
                    pl_ratio = profitVal / cost * 100;
                }

                ProfitDetailOut profitDetailOut = new ProfitDetailOut()
                {
                    stock = stock,
                    stocknm = table.CNAME,
                    tdate = tdate,
                    dseq = dseq,
                    dno = dno,
                    mqty = table.SQTY ?? 0m,
                    cqty = table.CQTY ?? 0m,
                    mprice = table.SPRICE.ToString(),
                    mamt = (table.CQTY * table.SPRICE).ToString(),
                    cost = cost,
                    income = table.INCOME ?? 0m,
                    netamt = table.INCOME ?? 0m,
                    fee = table.SFEE ?? 0m,
                    tax = 0m,
                    ttype = "0",
                    ttypename = "現股",
                    bstype = "S",
                    wtype = table is ExtendedHCNRH ? table.WTYPE : "0",
                    profit = profitVal,
                    pl_ratio = pl_ratio.ToString() + "%",
                    ctype = "0",
                    ttypename2 = table is ExtendedHCNRH ? "現賣" : "賣沖"
                };

                return profitDetailOut;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "失敗", $"已實現損益 - 個股明細資料 (賣出)獲取失敗，錯誤訊息：{ex.Message}");
                return null;
            }

        }
        
        /// <summary>
        /// 獲取所有的買入個股明細，並依據類型存入不同的資料
        /// </summary>
        /// <param name="tables">HCNTD或HCNRH組成的List</param>
        /// <returns>成功會回傳list，用來保存ProfitDetailOut</returns>
        public async Task<List<ProfitDetailSet>> GetProfitDetailSets(List<dynamic> tables)
        {
            List<ProfitDetailSet> list = new List<ProfitDetailSet>();

            foreach (dynamic table in tables)
            {
                string tdate = table.TDATE ?? string.Empty;
                string bhno = table.BHNO ?? string.Empty;
                string bseq = table.BSEQ ?? string.Empty;

                ProfitDetail profitDetail = GetProfitDetail(table);
                ProfitDetailOut profitDetailOut = GetProfitDetailOut(table);

                var existingSet = list.FirstOrDefault(p =>
                    p.profitDetailOut.tdate == tdate &&
                    p.profitDetailOut.dseq == bhno &&
                    p.profitDetailOut.dno == bseq);

                if (existingSet != null)
                {
                    existingSet.profitDetails.Add(profitDetail);

                    List<ProfitDetailOut> combinedOuts = new List<ProfitDetailOut> { existingSet.profitDetailOut, profitDetailOut };
                    var mergedOuts = await SumProfitDetailOut(combinedOuts);
                    existingSet.profitDetailOut = mergedOuts.FirstOrDefault();
                }
                else
                {
                    ProfitDetailSet newSet = new ProfitDetailSet
                    {
                        profitDetails = new List<ProfitDetail> { profitDetail },
                        profitDetailOut = profitDetailOut
                    };
                    list.Add(newSet);
                }
            }

            return list;
        }

        /// <summary>
        /// 將TDATE、SDSEQ、SDNO相同的ProfitDetailOut加總
        /// </summary>
        /// <param name="list">存放所有ProfitDetailOut的List</param>
        /// <returns>成功會回傳所有加總後ProfitDetailOut的list</returns>
        public async Task<List<ProfitDetailOut>> SumProfitDetailOut(List<ProfitDetailOut> list)
        {
            var groupedProfitDetails = list
                .GroupBy(t => new { t.tdate, t.dseq, t.dno })
                .Select(g =>
                {
                    try
                    {
                        Logger.Log(1, "參數", $"加總已實現損益 - 個股明細資料 (賣出) - 處理日期: {g.Key.tdate}, 委託書號: {g.Key.dseq}, 分單號: {g.Key.dno}");

                        return new ProfitDetailOut
                        {
                            stock = g.FirstOrDefault()?.stock,
                            stocknm = g.FirstOrDefault()?.stocknm,
                            tdate = g.Key.tdate,
                            dseq = g.Key.dseq,
                            dno = g.Key.dno,
                            mqty = g.Sum(t => t.mqty),
                            cqty = g.Sum(t => t.cqty),
                            mprice = g.FirstOrDefault()?.mprice,
                            mamt = g.Sum(t => decimal.Parse(t.mamt)).ToString(),
                            cost = g.Sum(t => t.cost),
                            income = g.Sum(t => t.income),
                            netamt = g.Sum(t => t.netamt),
                            fee = g.Sum(t => t.fee),
                            tax = 0m,
                            ttype = "0",
                            ttypename = "現股",
                            bstype = "S",
                            wtype = g.FirstOrDefault()?.wtype,
                            profit = g.Sum(t => t.profit),
                            pl_ratio = g.Sum(t => t.cost) != 0m
                                ? ((g.Sum(t => t.profit) / g.Sum(t => t.cost)) * 100m).ToString() + "%"
                                : "N/A",
                            ctype = "0",
                            ttypename2 = g.FirstOrDefault()?.ttypename2,
                        };
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(4, "錯誤", $"處理日期: {g.Key.tdate}, 委託書號: {g.Key.dseq}, 分單號: {g.Key.dno}, 加總失敗, 錯誤訊息: {ex.Message}");
                        return null;
                    }
                })
                .ToList();

            return groupedProfitDetails;
        }

        /// <summary>
        /// 獲取已實現損益-個股彙總資料
        /// </summary>
        /// <param name="profitDetailOut">存放單一股票且加總過的ProfitDetailOut的List</param>
        /// <param name="profitDetail">存放單一股票的ProfitDetail的List</param>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <returns>成功會回傳所有加總後ProfitDetailOut的list</returns>
        public ProfitSum GetProfitSum(ProfitDetailSet detailSet, string bhno, string cseq)
        {
            try
            {
                List<ProfitDetail> profitDetail = detailSet.profitDetails;
                ProfitDetailOut profitDetailOut = detailSet.profitDetailOut;

                string tdate = profitDetailOut.tdate;
                string dseq = profitDetailOut.dseq;
                string dno = profitDetailOut.dno;
                Logger.Log(1, "參數", $"已實現損益-個股彙總資料 - 處理日期: {tdate}, 委託書號: {dseq}, 分單號: {dno}");

                decimal? profit = profitDetailOut.profit;
                decimal? cost = profitDetailOut.cost;
                decimal? pl_ratio = 0m;
                if (cost != 0m)
                {
                    pl_ratio = profit / cost * 100;
                }
                ProfitSum profitSum = new ProfitSum()
                {
                    bhno = bhno,
                    cseq = cseq,
                    tdate = tdate,
                    dseq = dseq,
                    dno = dno,
                    ttype = "0",
                    ttypename = "現股",
                    bstype = "S",
                    stock = profitDetailOut.stock,
                    stocknm = profitDetailOut.stocknm,
                    cqty = profitDetailOut.cqty,
                    mprice = profitDetailOut.mprice,
                    fee = profitDetailOut.fee,
                    tax = profitDetailOut.tax,
                    cost = profitDetailOut.cost,
                    income = profitDetailOut.income,
                    profit = profitDetailOut.profit,
                    pl_ratio = pl_ratio.ToString() + "%",
                    ctype = "0",
                    ttypename2 = profitDetailOut.ttypename2,
                    profit_detail = profitDetail,
                    profit_detail_out = profitDetailOut
                };

                return profitSum;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"獲取已實現損益-個股彙總資料失敗, 錯誤訊息: {ex.Message}");
                return null;
            }

        }

        /// <summary>
        /// 獲取特定分公司特定帳號在時間範圍內的歷史現股當沖
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <param name="sdate">開始日</param>
        /// <param name="edate">結束日</param>
        /// <returns>回傳HCNTD加上CNAME組成的ExtendedHCNTD</returns>
        public async Task<List<ExtendedHCNTD>> GetHCNTDList(string bhno, string cseq, string sdate, string edate)
        {
            Logger.Log(1, "參數", $"獲取HCNTD table - bhno{bhno}, cseq{cseq}, sdate{sdate}, edate{edate}");
            try
            {
                return (await _repository.GetByTwoKeyWithTimeForHCNTD(bhno, cseq, sdate, edate)).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"獲取特定分公司特定帳號在時間範圍內的歷史現股當沖失敗, 錯誤訊息: {ex.Message}");
                return null;
            }

        }

        /// <summary>
        /// 獲取特定分公司特定帳號在時間範圍內的歷史現股沖銷
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <param name="sdate">開始日</param>
        /// <param name="edate">結束日</param>
        /// <returns>回傳HCNrh加上CNAME組成的ExtendedHCNrh</returns>
        public async Task<List<ExtendedHCNRH>> GetHCNRHList(string bhno, string cseq, string sdate, string edate)
        {
            Logger.Log(1, "參數", $"獲取HCNRH table - bhno{bhno}, cseq{cseq}, sdate{sdate}, edate{edate}");
            try
            {
                return (await _repository.GetByTwoKeyWithTimeForHCNRH(bhno, cseq, sdate, edate)).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"獲取特定分公司特定帳號在時間範圍內的歷史現股沖銷失敗, 錯誤訊息: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 將所有profitDetailOut依照股票一一取出並進行加總
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>\
        /// <param name="profitDetailSets">存放所有的ProfitDetail的List跟ProfitDetailOut的變數</param>
        /// <returns>成功會回傳所有加總後ProfitDetailOut的list</returns>
        public async Task<List<ProfitSum>> GetProfitSumList(string bhno, string cseq, List<ProfitDetailSet> profitDetailSets)
        {
            List<ProfitSum> profitSumList = new List<ProfitSum>();

            foreach (var profitDetailSet in profitDetailSets)
            {
                var profitSum = GetProfitSum(profitDetailSet, bhno, cseq);
                if (profitSum is null)
                {
                    return null;
                }
                profitSumList.Add(profitSum);
            }

            return profitSumList;
        }

        /// <summary>
        /// 將所有的profitSum進行加總
        /// </summary>
        /// <param name="list">存放所有ProfitSum的List</param>
        /// <returns>成功會回傳已實現損益的帳號匯總/// </returns>
        public async Task<ProfitAccsum> GetProfittAccsum(List<ProfitSum> list)
        {
            try
            {
                decimal? profit = list.Sum(t => t.profit);
                decimal? cost = list.Sum(t => t.cost);
                decimal? pl_ratio = 0m;
                if (cost != 0m)
                {
                    pl_ratio = profit / cost * 100;
                }

                ProfitAccsum profitAccsum = new ProfitAccsum()
                {
                    errcode = "0000",
                    errmsg = "成功",
                    cqty = list.Sum(t => t.cqty),
                    cost = cost,
                    income = list.Sum(t => t.income),
                    profit = profit,
                    pl_ratio = pl_ratio.ToString() + "%",
                    fee = list.Sum(t => t.fee),
                    tax = list.Sum(t => t.tax),
                    profit_sum = list

                };
                return profitAccsum;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"已實現損益的帳號匯總獲取失敗, 錯誤訊息: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 生成已實現損益的錯誤訊息
        /// </summary>
        /// <param name="errcode">錯誤碼</param>
        /// <param name="errmsg">錯誤訊息</param>
        /// <returns>成功會回傳unoffset_qtype_accsum，用來保存帳號匯總</returns>
        public async Task<ProfitAccsum> GetProfittAccsumFailed(string errcode, string errmsg)
        {
            ProfitAccsum profitAccsum = new ProfitAccsum()
            {
                errcode = errcode,
                errmsg = errmsg
            };

            return profitAccsum;
        }
    }
}
