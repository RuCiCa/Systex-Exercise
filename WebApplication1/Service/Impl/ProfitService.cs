using Azure.Core;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
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
        private readonly ProfitAccsum _profitAccsum;
        private readonly IProfitRepository _repository;
        private readonly Util _util;
        private readonly InMemoryCache _inMemoryCache; 


        public ProfitService(ProfitAccsum profitAccsum, IProfitRepository repository, Util util, InMemoryCache inMemoryCache)
        {
            _util = util;
            _profitAccsum = profitAccsum;
            _repository = repository;
            _inMemoryCache = inMemoryCache;
        }


        /// <summary>
        /// 獲取所有的買入個股明細，並依據類型存入不同的資料
        /// </summary>
        /// <param name="table">HCNTD或HCNRH的表</param>
        /// <returns>成功會回傳ProfitDetail</returns>
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
                decimal pl_ratio = _util.CalcPlRatio(profitVal, cost);

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
        /// <returns>成功會回傳ProfitDetailOut</returns>
        public ProfitDetailOut GetProfitDetailOut(dynamic table)
        {
            try
            {
                string tableType = table is ExtendedHCNRH ? "HCNRH" : "HCNTD";
                string stock = table.STOCK;
                string tdate = table.TDATE;
                string dseq = table.SDSEQ;
                string dno = table.SDNO;
                decimal sqty = table.SQTY;
                Logger.Log(1, "參數", $"已實現損益 - 個股明細資料 (賣出) - 資料庫： {tableType}, stock： {stock}, tdate： {tdate}, dseq： {dseq}, dno： {dno}, sqty: {sqty}");

                decimal cost = table.COST ?? 0m;
                decimal profitVal = table.PROFIT ?? 0m;
                decimal pl_ratio = _util.CalcPlRatio(profitVal, cost);

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
                    tax = table.TAX,
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
        /// 獲取所有的買入個股明細，讓買入賣出一起處理，並依據交易日、委託書號、分單號作為依據進行分類
        /// </summary>
        /// <param name="tables">HCNTD或HCNRH組成的List</param>
        /// <returns>成功會回傳list，用來保存ProfitDetailOut</returns>
        public List<ProfitSum> GetProfitSumList(List<dynamic> tables, string bhno, string cseq)
        {
            // 使用 GroupBy 將表格依據 TDATE, SDSEQ 和 SDNO 分組
            var groupedTables = tables.GroupBy(table => new
            {
                Tdate = table.TDATE ?? string.Empty,
                Dseq = table.SDSEQ ?? string.Empty,
                Dno = table.SDNO ?? string.Empty
            });

            List<ProfitSum> list = new List<ProfitSum>();

            foreach (var group in groupedTables)
            {
                // 取得分組的關鍵值
                string tdate = group.Key.Tdate;
                string dseq = group.Key.Dseq;
                string dno = group.Key.Dno;

                List<ProfitDetail> profitDetails = new List<ProfitDetail>();
                List<ProfitDetailOut> combinedOuts = new List<ProfitDetailOut>();

                foreach (var table in group)
                {
                    // 處理 ProfitDetail 和 ProfitDetailOut    
                    ProfitDetail profitDetail = GetProfitDetail(table);
                    ProfitDetailOut profitDetailOut = GetProfitDetailOut(table);
                    // 將每個群組的 profitDetail 加入到 list
                    profitDetails.Add(profitDetail);
                    combinedOuts.Add(profitDetailOut);
                }
                // 合併 ProfitDetailOut 的資料
                var mergedOuts = SumProfitDetailOut(combinedOuts);
                ProfitSum profitSum = GetProfitSum(profitDetails, mergedOuts, bhno, cseq);

                // 將新的 ProfitDetailSet 加入到結果列表中
                list.Add(profitSum);
            }
            
            return list;
        }

        /// <summary>
        /// 將TDATE、SDSEQ、SDNO相同的ProfitDetailOut加總
        /// </summary>
        /// <param name="list">存放所有ProfitDetailOut的List</param>
        /// <returns>成功會回傳加總後的ProfitDetailOut</returns>
        public ProfitDetailOut SumProfitDetailOut(List<ProfitDetailOut> list)
        {
            try
            {
                // 假設所有的 ProfitDetailOut 都具有相同的 TDATE、SDSEQ 和 SDNO
                var firstItem = list.FirstOrDefault();
                if (firstItem == null) return null;
                // 記錄所有的 mqty 值
                var mqtyValues = string.Join(", ", list.Select(t => t.mqty));
                Logger.Log(1, "參數", $"加總已實現損益 - 個股明細資料 (賣出) - 處理日期: {firstItem.tdate}, 委託書號: {firstItem.dseq}, 分單號: {firstItem.dno}, mqty 值: {mqtyValues}");

                return new ProfitDetailOut
                {
                    stock = firstItem.stock,
                    stocknm = firstItem.stocknm,
                    tdate = firstItem.tdate,
                    dseq = firstItem.dseq,
                    dno = firstItem.dno,
                    mqty = firstItem.mqty,
                    cqty = list.Sum(t => t.cqty),
                    mprice = firstItem.mprice,
                    mamt = list.Sum(t => decimal.Parse(t.mamt)).ToString(),
                    cost = list.Sum(t => t.cost),
                    income = list.Sum(t => t.income),
                    netamt = list.Sum(t => t.netamt),
                    fee = list.Sum(t => t.fee),
                    tax = list.Sum(t => t.tax),
                    ttype = "0",
                    ttypename = "現股",
                    bstype = "S",
                    wtype = firstItem.wtype,
                    profit = list.Sum(t => t.profit),
                    pl_ratio = _util.CalcPlRatio(list.Sum(t => t.profit).GetValueOrDefault(), list.Sum(t => t.cost).GetValueOrDefault()).ToString() + "%",
                    ctype = "0",
                    ttypename2 = firstItem.ttypename2,
                };
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"處理日期: {list.FirstOrDefault()?.tdate}, 委託書號: {list.FirstOrDefault()?.dseq}, 分單號: {list.FirstOrDefault()?.dno}, 加總失敗, 錯誤訊息: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 獲取已實現損益-個股彙總資料
        /// </summary>
        /// <param name="profitDetailOut">存放單一股票且加總過的ProfitDetailOut的List</param>
        /// <param name="profitDetail">存放單一股票的ProfitDetail的List</param>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <returns>成功會回傳所有加總後ProfitDetailOut的list</returns>
        public ProfitSum GetProfitSum(List<ProfitDetail> profitDetail, ProfitDetailOut profitDetailOut, string bhno, string cseq)
        {
            try
            {
                string tdate = profitDetailOut.tdate;
                string dseq = profitDetailOut.dseq;
                string dno = profitDetailOut.dno;
                Logger.Log(1, "參數", $"已實現損益-個股彙總資料 - 處理日期: {tdate}, 委託書號: {dseq}, 分單號: {dno}");

                decimal? profit = profitDetailOut.profit;
                decimal? cost = profitDetailOut.cost;
                decimal pl_ratio = 0m;
                if (cost.HasValue && cost.Value != 0m)
                {
                    pl_ratio = profit.HasValue ? (profit.Value / cost.Value * 100m) : 0m;
                    pl_ratio = Math.Round(pl_ratio, 2);
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

        public List<ExtendedTMHIO> GetTMHIOList(List<TMHIO> tmhioList)
        {
            try
            {
                if (tmhioList.Count() == 0)
                {
                    Logger.Log(1, "參數", $"未找到TMHIO");
                    return new List<ExtendedTMHIO>();
                }
                Logger.Log(1, "參數", $"TMHIO一共有{tmhioList.Count()}筆資料");
                var mstmbDict = _inMemoryCache.MSTMBData;

                var result = (from tmhio in tmhioList
                              select new ExtendedTMHIO
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
                                  CNAME = mstmbDict[tmhio.STOCK].CNAME,

                                  AMT = _util.CalcMamt(tmhio.PRICE, (decimal)tmhio.QTY),
                                  //RES>B
                                  RESINCOME = _util.CalcMamt(tmhio.PRICE, (decimal)tmhio.QTY) + _util.CalcFee(tmhio.PRICE, (decimal)tmhio.QTY, etype: _util.TransEtpye(tmhio.ETYPE)),
                                  RESQTY = tmhio.QTY,
                                  RESTAX = _util.CalcTax(tmhio.PRICE, (decimal)tmhio.QTY),
                                  RESFEE = _util.CalcFee(tmhio.PRICE, (decimal)tmhio.QTY),


                              }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"獲取TMHIO與MSTMB失敗, 錯誤訊息: {ex.Message}");
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
        /// <param name="stockSymbol">股票代號</param>
        /// <returns>回傳HCNRH加上CNAME組成的ExtendedHCNrh</returns>
        public List<ExtendedTCNUD> GetTCNUDList(List<TCNUD> tcnudList)
        {
            try
            {
                if (tcnudList.Count() == 0)
                {
                    Logger.Log(1, "參數", $"未找到TCNUD");
                    return new List<ExtendedTCNUD>();
                }
                Logger.Log(1, "參數", $"TCNUD一共有{tcnudList.Count()}筆資料");
                var mstmbDict = _inMemoryCache.MSTMBData;

                Logger.Log(0, "開始", $"獲取TCNUD");
                var result = (from tcnud in tcnudList
                              select new ExtendedTCNUD
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

                                  AMT = 0,
                                  CNAME = mstmbDict[tcnud.STOCK].CNAME,
                                  CPRICE = mstmbDict[tcnud.STOCK].CPRICE,
                                  ETYPE = "",

                                  //剩餘用B
                                  RESCOST = tcnud.COST,
                                  RESQTY = tcnud.QTY,
                                  RESFEE = tcnud.FEE,
                              }).ToList();
                result = result.OrderBy(t => t.TDATE)
                    .ThenBy(t => t.WTYPE)
                    .ThenBy(t => t.DNO)
                    .ToList();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"獲取TCNUD與MSTMB失敗, 錯誤訊息: {ex.Message}");
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
        /// <param name="stockSymbol">股票代號</param>
        /// <returns>回傳HCNTD加上CNAME組成的ExtendedHCNTD</returns>
        public async Task<List<ExtendedHCNTD>> GetHCNTDList(string bhno, string cseq, string sdate, string edate, string stockSymbol)
        {
            Logger.Log(1, "參數", $"獲取HCNTD table - bhno{bhno}, cseq{cseq}, sdate{sdate}, edate{edate}");
            try
            {
                var hcntdList = await _repository.GetByTwoKeyWithTimeForHCNTD(bhno, cseq, sdate, edate, stockSymbol);
                var mstmbDict = _inMemoryCache.MSTMBData;
                var result = (from hcntd in hcntdList
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
                                  CNAME = mstmbDict[hcntd.STOCK].CNAME ?? string.Empty 
                              }).ToList();

                return result;
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
        /// <param name="stockSymbol">股票代號</param>
        /// <returns>回傳HCNRH加上CNAME組成的ExtendedHCNrh</returns>
        public async Task<List<ExtendedHCNRH>> GetHCNRHList(string bhno, string cseq, string sdate, string edate, string stockSymbol)
        {
            Logger.Log(1, "參數", $"獲取HCNRH table - bhno{bhno}, cseq{cseq}, sdate{sdate}, edate{edate}");
            try
            {
                var hcnrhList = await _repository.GetByTwoKeyWithTimeForHCNRH(bhno, cseq, sdate, edate, stockSymbol);
                var tcnudList = await _repository.GetByTwoKeyWithTimeForTCNUD(bhno, cseq, stockSymbol);
                var tmhioList = await _repository.GetByTwoKeyWithTimeForTMHIO(bhno, cseq, sdate, edate, stockSymbol);

                Logger.Log(1, "參數", $"HCNRH有{hcnrhList.Count()}筆、TCNUD有{tcnudList.Count()}筆、TMHIO有{tmhioList.Count()}筆");
                List<ExtendedTMHIO> tmhios = GetTMHIOList(tmhioList.ToList());
                var tmhioStocks = tmhios.Select(t => t.STOCK).Distinct();
                Logger.Log(1, "TMHIO STOCKS", string.Join(", ", tmhioStocks));

                List<ExtendedTCNUD> tcnuds = GetTCNUDList(tcnudList.ToList());

                // 列出 tcnuds 的 STOCK
                var tcnudStocks = tcnuds.Select(t => t.STOCK).Distinct();
                Logger.Log(1, "TCNUD STOCKS", string.Join(", ", tcnudStocks));

                List<HCNRH> hcnrhList2 = new List<HCNRH>();
                // 將 tcnuds 按照 STOCK 分組
                var groupedTCNUD = tcnuds.GroupBy(t => t.STOCK);

                // 逐一處理每個分組
                foreach (var group in groupedTCNUD)
                {
                    // 獲取分組內的資料
                    List<ExtendedTCNUD> groupedTcnuds = group.ToList();

                    // 可以根據需要篩選 tmhioList（如有需要，也可以加上條件）
                    List<ExtendedTMHIO> relatedTmhios = tmhios.Where(t => t.STOCK == group.Key).ToList();

                    // 調用 WriteOff 方法
                    var newHCNRHList = WriteOff(groupedTcnuds, relatedTmhios);

                    // 合併結果
                    hcnrhList2 = _util.ConcatLists(hcnrhList2, newHCNRHList);
                }

                hcnrhList = _util.ConcatLists(hcnrhList.ToList(), hcnrhList2);

                var mstmbDict = _inMemoryCache.MSTMBData;
                var result = (from hcnrh in hcnrhList
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
                                  CNAME = mstmbDict[hcnrh.STOCK].CNAME ?? string.Empty 
                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"獲取特定分公司特定帳號在時間範圍內的歷史現股沖銷失敗, 錯誤訊息: {ex.Message}");
                return null;
            }
        }


        /// <summary>
        /// 將所有的profitSum進行加總
        /// </summary>
        /// <param name="list">存放所有ProfitSum的List</param>
        /// <returns>成功會回傳已實現損益的帳號匯總/// </returns>
        public ProfitAccsum GetProfittAccsum(List<ProfitSum> list)
        {
            try
            {
                decimal? profit = list.Sum(t => t.profit);
                decimal? cost = list.Sum(t => t.cost);
                decimal pl_ratio = 0m;
                if (cost.HasValue && cost.Value != 0m) // 檢查 cost 是否有值且不為 0
                {
                    pl_ratio = profit.HasValue ? (profit.Value / cost.Value * 100m) : 0m; // 檢查 profit 是否有值
                    pl_ratio = Math.Round(pl_ratio, 2);
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

        public List<HCNRH> WriteOff(List<ExtendedTCNUD> tcnuds, List<ExtendedTMHIO> tmhios)
        {
            // 先按 FIFO 原則排序 tcnuds跟tmhios
            tcnuds = tcnuds.OrderBy(x => x.TDATE).ThenBy(x => x.WTYPE).ThenBy(x => x.DNO).ToList();
            tmhios = tmhios.OrderBy(x => x.TDATE).ThenBy(x => x.JRNUM).ToList();
            List<HCNRH> hcnrhs = new List<HCNRH>();

            foreach (var tmhio in tmhios)
            {
                decimal remainingQty = tmhio.RESQTY;
                //額外宣告一個var，來作為判斷的依據
                foreach (var tcnud in tcnuds.Where(t => t.STOCK == tmhio.STOCK && t.RESQTY > 0))
                {
                    if (tmhio.RESQTY <= 0)
                    {
                        if (hcnrhs.Count > 0)
                        {
                            var lastRecord = hcnrhs.Last();

                            // 更新各欄位
                            lastRecord.BFEE += tcnuds.Sum(t => t.RESFEE);
                            lastRecord.SFEE += tmhios.Sum(t => t.RESFEE);
                            lastRecord.TAX += tmhios.Sum(t => t.RESTAX);
                            lastRecord.INCOME += tmhios.Sum(t => t.RESINCOME);
                            lastRecord.COST += tcnuds.Sum(t => t.RESCOST);

                            // 重新計算 PROFIT
                            lastRecord.PROFIT = lastRecord.INCOME - lastRecord.COST;
                        }
                        break;
                    }
                       

                    //改成cqty
                    decimal qtyToWriteOff = Decimal.Min(tcnud.RESQTY, tmhio.RESQTY);

                    decimal bFee = _util.CalcUnOffset(tcnud.FEE, qtyToWriteOff, tcnud.QTY);
                    decimal cost = Math.Floor(tcnud.PRICE * qtyToWriteOff) + bFee;

                    //用原始的fee跟tax
                    decimal sFee = _util.CalcUnOffset(tmhio.RESFEE, qtyToWriteOff, tmhio.QTY);
                    decimal tax = _util.CalcUnOffset(tmhio.RESTAX, qtyToWriteOff, tmhio.QTY);
                    decimal income = Math.Floor(tmhio.PRICE * qtyToWriteOff) - sFee - tax;

                    Logger.Log(1, "參數", $"cost:{cost}、income:{income}、sFee:{sFee}、bFee:{bFee}、tax:{tax}、qtyToWriteOff:{qtyToWriteOff}");

                    HCNRH hcnrh = new HCNRH
                    {
                        BHNO = tmhio.BHNO,
                        TDATE = tmhio.TDATE,
                        RDATE = tcnud.TDATE,
                        CSEQ = tcnud.CSEQ,
                        BDSEQ = tcnud.DSEQ,
                        BDNO = tcnud.DNO,
                        SDSEQ = tmhio.DSEQ,
                        SDNO = tmhio.JRNUM,
                        STOCK = tmhio.STOCK,
                        CQTY = qtyToWriteOff,
                        BPRICE = tcnud.COST / tcnud.BQTY,
                        BFEE = bFee,
                        SPRICE = tmhio.PRICE,
                        SFEE = sFee,
                        TAX = tax,
                        INCOME = income,
                        COST = cost,
                        PROFIT = income - cost,
                        ADJDATE = "",
                        WTYPE = "0",
                        BQTY = tcnud.BQTY,
                        SQTY = tmhio.QTY,
                    };

                    // 更新 tcnud 剩餘的數據
                    tcnud.RESQTY -= qtyToWriteOff;
                    tcnud.RESFEE -= bFee;
                    tcnud.RESCOST -= cost;
                    tmhio.RESQTY -= qtyToWriteOff;
                    tmhio.RESFEE -= sFee;
                    tmhio.RESTAX -= tax;
                    tmhio.RESINCOME -= income;
                    remainingQty -= qtyToWriteOff;

                    hcnrhs.Add(hcnrh);
                    
                    //少寫最後加總
                }
            }

            return hcnrhs;
        }


        /// <summary>
        /// 生成已實現損益的錯誤訊息
        /// </summary>
        /// <param name="errcode">錯誤碼</param>
        /// <param name="errmsg">錯誤訊息</param>
        /// <returns>成功會回傳unoffset_qtype_accsum，用來保存帳號匯總</returns>
        public ProfitAccsum GetProfittAccsumFailed(string errcode, string errmsg)
        {
            ProfitAccsum profitAccsum = new ProfitAccsum()
            {
                errcode = errcode,
                errmsg = errmsg
            };

            return profitAccsum;
        }

        public async Task<ProfitAccsum> GetProfitService(string bhno, string cseq, string sdate, string Edate, string stockSymbol)
        {
            try
            {

                Logger.Log(0, "開始", $"開始搜尋{bhno}帳號{cseq}的交易紀錄");
                List<ExtendedHCNTD> HCNTDList = await GetHCNTDList(bhno, cseq, sdate, Edate, stockSymbol);
                List<ExtendedHCNRH> HCNRHList = await GetHCNRHList(bhno, cseq, sdate, Edate, stockSymbol);

                Logger.Log(1, "參數", $"HCNTD有{HCNTDList.Count()}筆、HCNRH有{HCNRHList.Count()}筆");
                List<ProfitDetailOut> profitDetailOuts = new List<ProfitDetailOut>();
                List<ProfitDetail> profitDetails = new List<ProfitDetail>();
                List<ProfitSum> profitSumList = new List<ProfitSum>();
                
                if (HCNTDList.Count == 0 && HCNRHList.Count == 0)
                {
                    Logger.Log(3, "錯誤", $"未找到分公司{bhno}帳號{cseq}在{sdate} 到 {Edate}這段期間的交易紀錄");
                    return GetProfittAccsumFailed("404", $"未找到分公司{bhno}帳號{cseq}在{sdate} 到 {Edate}這段期間的交易");
                }

                profitSumList = _util.ConcatLists(GetProfitSumList(HCNTDList.Cast<dynamic>().ToList(), bhno, cseq), GetProfitSumList(HCNRHList.Cast<dynamic>().ToList(), bhno, cseq));



                var response = GetProfittAccsum(profitSumList);
                if (response is null)
                {
                    return GetProfittAccsumFailed("500", $"獲取國內證券 已實現損益 帳戶彙總資料時出現錯誤");
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"搜尋{bhno}帳號{cseq}的交易紀錄時出現了錯誤：{ex}");
                var response = GetProfittAccsumFailed("500", "Internal Server Error");
                return response;
            }
        }
    }
}
