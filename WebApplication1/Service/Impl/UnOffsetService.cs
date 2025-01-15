using Azure.Core;
using System.Collections.Generic;
using System.ComponentModel;
using WebApplication1.Common;
using WebApplication1.Common.HCN;
using WebApplication1.Repositories.Api;
using WebApplication1.Service.Api;
using WebApplication1.Service.Dtos;

namespace WebApplication1.Service.Impl
{
    public class UnOffsetService : IUnOffsetService
    {
        private readonly UnOffsetAccsum _unOffsetAccsum;
        private readonly IUnOffsetRepository _repository;
        private readonly Util _util;
        private readonly InMemoryCache _inMemoryCache;

        public UnOffsetService(UnOffsetAccsum unOffsetAccsum, IUnOffsetRepository repository, Util util, InMemoryCache inMemoryCache)
        {
            _util = util;
            _unOffsetAccsum = unOffsetAccsum;
            _repository = repository;
            _inMemoryCache = inMemoryCache;
        }

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

        public List<ExtendedTCNUD> GetTMHIOList(List<TMHIO> tmhioList)
        {
            try
            {
                if (tmhioList.Count() == 0)
                {
                    Logger.Log(1, "參數", $"未找到TMHIO");
                    return new List<ExtendedTCNUD>();
                }
                Logger.Log(1, "參數", $"TMHIO一共有{tmhioList.Count()}筆資料");
                var mstmbDict = _inMemoryCache.MSTMBData;

                var result = (from tmhio in tmhioList
                                   select new ExtendedTCNUD
                                   {
                                       TDATE = tmhio.TDATE,
                                       BHNO = tmhio.BHNO,
                                       CSEQ = tmhio.CSEQ,
                                       STOCK = tmhio.STOCK,
                                       PRICE = tmhio.PRICE,
                                       QTY = tmhio.QTY,
                                       BQTY = tmhio.QTY,
                                       FEE = _util.CalcFee(tmhio.PRICE, (decimal)tmhio.QTY, etype: _util.TransEtpye(tmhio.ETYPE)),
                                       COST = _util.CalcMamt(tmhio.PRICE, (decimal)tmhio.QTY) + _util.CalcFee(tmhio.PRICE, (decimal)tmhio.QTY, etype: _util.TransEtpye(tmhio.ETYPE)),
                                       DSEQ = tmhio.DSEQ,
                                       DNO = tmhio.JRNUM,
                                       WTYPE = "0",
                                       TRDATE = tmhio.TRDATE,
                                       TRTIME = tmhio.TRTIME,
                                       MODTIME = tmhio.MODTIME,
                                       MODUSER = tmhio.MODUSER,


                                       ETYPE = _util.TransEtpye(tmhio.ETYPE),
                                       AMT = _util.CalcMamt(tmhio.PRICE, (decimal)tmhio.QTY),
                                       CNAME = mstmbDict[tmhio.STOCK].CNAME,
                                       CPRICE = mstmbDict[tmhio.STOCK].CPRICE,


                                   }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"獲取TMHIO與MSTMB失敗, 錯誤訊息: {ex.Message}");
                return null;
            }
        }

        public List<ExtendedTCNUD> GetTCSIOList(List<TCSIO> tcsioList)
        {
            try
            {
                if (tcsioList.Count() == 0)
                {
                    Logger.Log(1, "參數", $"未找到TCSIO");
                    return new List<ExtendedTCNUD>();
                }
                Logger.Log(1, "參數", $"TCSIO一共有{tcsioList.Count()}筆資料");
                var mstmbDict = _inMemoryCache.MSTMBData;
                var msysDict = _inMemoryCache.MSYSData;

                var result = (from tcsio in tcsioList
                              select new ExtendedTCNUD
                              {
                                  TDATE = tcsio.TDATE,
                                  BHNO = tcsio.BHNO,
                                  CSEQ = tcsio.CSEQ,
                                  STOCK = tcsio.STOCK,
                                  PRICE = 0,
                                  //QTY = tcsio.QTY,
                                  //BQTY = tcsio.QTY,
                                  FEE = 0,
                                  COST = 0,
                                  DSEQ = tcsio.DSEQ,
                                  DNO = tcsio.DNO,
                                  WTYPE = "A",
                                  TRDATE = tcsio.TRDATE,
                                  TRTIME = tcsio.TRTIME,
                                  MODTIME = tcsio.MODTIME,
                                  MODUSER = tcsio.MODUSER,
                                  CNAME = mstmbDict[tcsio.STOCK].CNAME,
                                  CPRICE = mstmbDict[tcsio.STOCK].CPRICE,
                                  IOFLAG = tcsio.IOFLAG,
                                  IOFLAGNAME = _util.GetIoflagname(tcsio.IOFLAG, msysDict),
                                  BSTYPE = "B"

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
        /// 獲取未實現損益的個股明細
        /// </summary>
        /// <param name="unOffset">存放TCNUD以及CNAME跟CPRICE</param>
        /// <returns>成功會回傳unOffsetDetail，用來保存</returns>
        public UnOffsetDetail GetUnOffsetDetail(ExtendedTCNUD extendedTCNUD)
        {
            Logger.Log(0, "開始", $"開始計算{extendedTCNUD.CNAME}之未實現損益 – 個股明細，委託書號{extendedTCNUD.DSEQ}-分單號碼{extendedTCNUD.DNO}");
            try
            {
                string stock = extendedTCNUD.STOCK;
                string stocknm = extendedTCNUD.CNAME;
                decimal? bqty = extendedTCNUD.BQTY;
                decimal? mprice = extendedTCNUD.PRICE;
                decimal? mamt = extendedTCNUD.AMT;
                decimal? lastprice = extendedTCNUD.CPRICE;
                decimal? cost = extendedTCNUD.COST;
                decimal? estimateAmt = Math.Floor((lastprice * extendedTCNUD.BQTY) ?? 0m);
                decimal estFee = 0.001425m;
                decimal estTax = 0.003m;
                Logger.Log(2, "變數", $"手續費為：{estFee}, 稅率為：{estTax}");
                decimal? estimateFee = Math.Floor((estimateAmt * estFee) ?? 0m);
                if (estimateFee < 20m)
                {
                    estimateFee = 20m;
                }

                decimal? estimateTax = Math.Floor((estimateAmt * estTax) ?? 0m);
                decimal? marketvalue = estimateAmt - estimateFee - estimateTax;
                decimal? profit = marketvalue - cost;
                string pl_ratio;
                string ioflag = extendedTCNUD.IOFLAG;
                string ioflagname = extendedTCNUD.IOFLAGNAME;
                string wtype = extendedTCNUD.WTYPE;

                if (cost != 0m)
                {
                    pl_ratio = ((profit / cost) * 100).ToString() + "%";
                }
                else
                {
                    Logger.Log(3, "警告", "cost為0，無法計算pl_ratio");
                    pl_ratio = "NAN%";
                }

                UnOffsetDetail unOffsetDetail = new UnOffsetDetail()
                {
                    stock = stock,
                    stocknm = stocknm,
                    tdate = extendedTCNUD.TDATE,
                    ttype = "0",
                    ttypename = "現買",
                    bstype = "B",
                    dseq = extendedTCNUD.DSEQ,
                    dno = extendedTCNUD.DNO,
                    bqty = bqty,
                    mprice = mprice,
                    mamt = mamt,
                    lastprice = lastprice,
                    marketvalue = marketvalue,
                    fee = extendedTCNUD.FEE,
                    tax = 0,
                    cost = cost,
                    estimateAmt = estimateAmt,
                    estimateFee = estimateFee,
                    estimateTax = estimateTax,
                    profit = profit,
                    pl_ratio = pl_ratio,
                    ioflag = ioflag,
                    ioname = ioflagname,
                    wtype = wtype

                };
                Logger.Log(0, "成功", $"{extendedTCNUD.CNAME}之未實現損益 – 個股明細，委託書號{extendedTCNUD.DSEQ}-分單號碼{extendedTCNUD.DNO}計算成功");
                return unOffsetDetail;
            }
            //透過errorHandler來處理errcode跟errmsg
            catch (Exception ex)
            {
                //errorHandler("500", $"計算{unOffset.CNAME}之未實現損益 – 個股明細，委託書號{tcnud.DSEQ}-分單號碼{tcnud.DNO}時出現錯誤");
                Logger.Log(4, "失敗", $"錯誤訊息: {ex}");
                return null;
            }
        }

        /// <summary>
        /// 獲取個股未實現損益
        /// </summary>
        /// <param name="bhno">分公司</param>
        /// <param name="cseq">帳號</param>
        /// <returns>成功會回傳unOffsetDetailList，用來保存unOffsetDetail</returns>
        public List<UnOffsetDetail> GetUnOffsetDetailList(List<ExtendedTCNUD> list)
        {
            var unOffsetDetailList = new List<UnOffsetDetail>();


            //List LinQ
            foreach (var unOffset in list)
            {
                var unOffsetDetail = GetUnOffsetDetail(unOffset);

                if (unOffsetDetail == null)
                {
                    return null;
                }
                try
                {
                    unOffsetDetailList.Add(unOffsetDetail);
                }
                catch (Exception ex)
                {
                    Logger.Log(4, "失敗", $"錯誤訊息: {ex}");
                    //errorHandler("500", $"計算{cName}之個股未實現損益時出現錯誤");
                    return null;
                }
            }

            //回傳list再往上做個股明細的list
            return unOffsetDetailList;
        }

        /// <summary>
        /// 將list裡面的所有UnOffsetDetail的值加總，獲取單一股票的unOffsetSum(個股未實現損益)
        /// </summary>
        /// <param name="list">存放單一股票的所有未實現損益明細</param>
        /// <returns>成功會回傳單一股票的unOffsetSum，用來保存個股未實現損益</returns>
        public UnOffsetSum GetUnOffsetSum(List<UnOffsetDetail> list)
        {
            try
            {
                string stock = list.FirstOrDefault()?.stock;
                string stocknm = list.FirstOrDefault()?.stocknm;
                decimal? bqty = list.Sum(t => t.bqty);
                decimal? cost = list.Sum(t => t.cost);
                decimal? avgprice = cost / bqty;
                decimal? marketvalue = list.Sum(t => t.marketvalue);
                decimal? estimateAmt = Math.Floor((list.Sum(t => t.estimateAmt)) ?? 0m);
                decimal? estimateFee = Math.Floor((list.Sum(t => t.estimateFee)) ?? 0m);
                if (estimateFee < 20m)
                {
                    estimateFee = 20m;
                }
                decimal? estimateTax = Math.Floor((list.Sum(t => t.estimateTax)) ?? 0m);
                decimal? profit = list.Sum(t => t.profit);
                decimal? fee = list.Sum(t => t.fee);
                decimal? tax = list.Sum(t => t.tax);
                decimal? amt = list.Sum(t => t.mamt);
                string pl_ratio;

                //UnOffsetDetail已經有做過錯誤log紀錄
                if (cost != 0m)
                {
                    pl_ratio = ((profit / cost) * 100).ToString() + "%";
                }
                else
                {
                    pl_ratio = "NAN%";
                }

                UnOffsetSum unOffsetSum = new UnOffsetSum()
                {
                    stock = stock,
                    stocknm = stocknm,
                    ttype = "0",
                    ttypename = "現買",
                    bstype = "B",
                    bqty = bqty,
                    cost = cost,
                    avgprice = avgprice,
                    lastprice = list.FirstOrDefault()?.lastprice,
                    marketvalue = marketvalue,
                    estimateAmt = estimateAmt,
                    estimateFee = estimateFee,
                    estimateTax = estimateTax,
                    profit = profit,
                    pl_ratio = pl_ratio,
                    fee = fee,
                    tax = tax,
                    amt = amt,
                    unoffset_qtype_detail = list
                };

                return unOffsetSum;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "失敗", $"錯誤訊息: {ex}");
                //errorHandler("500", $"計算{stocknm}個股未實現損益時出現錯誤");
                return null;
            }
        }

        /// <summary>
        /// 將list依照股票對每個list進行分類後一一獲取UnOffsetSum並存入list
        /// </summary>
        /// <param name="list">所有的個股明細</param>
        /// <returns>成功會回傳unOffsetSumList，用來保存所有的個股未實現損益</returns>
        public List<UnOffsetSum> GetUnOffsetSumList(List<UnOffsetDetail> list)
        {
            List<UnOffsetSum> unOffsetSums = new List<UnOffsetSum>();
            var groupedUnOffsets = list.GroupBy(u => new { u.stock, u.stocknm });
            //根據一種股票一個個股未實現權益
            foreach (var group in groupedUnOffsets)
            {
                Logger.Log(0, "開始", $"開始計算{group.Key.stocknm}之個股未實現損益");
                try
                {
                    UnOffsetSum unOffsetSum = GetUnOffsetSum(group.ToList());
                    if (unOffsetSum == null)
                    {
                        return null;
                    }
                    unOffsetSums.Add(unOffsetSum);
                }
                catch (Exception ex)
                {
                    Logger.Log(4, "失敗", $"錯誤訊息: {ex}");
                    //errorHandler("500", $"計算{group.Key.stocknm}之個股未實現損益時出現錯誤");
                    return null;
                }
                Logger.Log(0, "成功", $"{group.Key.stocknm}之個股未實現損益計算成功");
            }
            return unOffsetSums;
        }

        /// <summary>
        /// 將所有的個股未實現損益加總並傳出
        /// </summary>
        /// <param name="list">所有的個股未實現損益</param>
        /// <returns>成功會回傳unoffset_qtype_accsum，用來保存帳號匯總</returns>
        public UnOffsetAccsum GetUnOffsetAccsum(List<UnOffsetSum> list)
        {
            Logger.Log(0, "開始", $"開始獲取國內證券-未實現損益 帳號彙總");
            UnOffsetAccsum unoffset_qtype_accsum = new UnOffsetAccsum();
            try
            {

                //當GetUnOffsetSum或是GetUnOffsetDetail出現問題時會回傳null，因為訊息已經寫好，所以直接回傳只存放有錯誤訊息的
                if (list == null)
                {
                    return unoffset_qtype_accsum;
                }

                //透過list的加總功能加總每一項
                var bqty = list.Sum(t => t.bqty);
                var cost = list.Sum(t => t.cost);
                var marketvalue = list.Sum(t => t.marketvalue);
                var profit = list.Sum(t => t.profit);

                //在UnOffsetDetail已經有對pl_ratio做錯誤log的紀錄了，所以只在裡面記錄一次
                string pl_ratio;
                if (cost != 0m)
                {
                    pl_ratio = ((profit / cost) * 100).ToString() + "%";
                }
                else
                {
                    pl_ratio = "NAN%";
                }

                var fee = list.Sum(t => t.fee);
                var tax = list.Sum(t => t.tax);
                decimal? estimateAmt = Math.Floor((list.Sum(t => t.estimateAmt)) ?? 0m);
                decimal? estimateFee = Math.Floor((list.Sum(t => t.estimateFee)) ?? 0m);
                if (estimateFee < 20m)
                {
                    estimateFee = 20m;
                }
                decimal? estimateTax = Math.Floor((list.Sum(t => t.estimateTax)) ?? 0m);

                var errcode = "0000";
                var errmsg = "成功";

                unoffset_qtype_accsum = new UnOffsetAccsum
                {
                    errcode = errcode,
                    errmsg = errmsg,
                    bqty = bqty,
                    cost = cost,
                    marketvalue = marketvalue,
                    profit = profit,
                    pl_ratio = pl_ratio,
                    fee = fee,
                    tax = tax,
                    estimateAmt = estimateAmt,
                    estimateFee = estimateFee,
                    estimateTax = estimateTax,
                    unoffset_qtype_sum = list
                };
                Logger.Log(0, "成功", "國內證券-未實現損益 帳號彙總獲取成功");
                return unoffset_qtype_accsum;
            }
            catch (Exception ex)
            {
                unoffset_qtype_accsum.errcode = "500";
                unoffset_qtype_accsum.errmsg = "國內證券-未實現損益 帳號彙總獲取失敗";
                unoffset_qtype_accsum.unoffset_qtype_sum = [];

                Logger.Log(4, "失敗", $"國內證券-未實現損益 帳號彙總獲取失敗，錯誤訊息：{ex.Message}");

                return unoffset_qtype_accsum;
            }
        }

        /// <summary>
        /// 生成未實現損益的錯誤訊息
        /// </summary>
        /// <param name="errcode">錯誤碼</param>
        /// <param name="errmsg">錯誤訊息</param>
        /// <returns>成功會回傳unoffset_qtype_accsum，用來保存帳號匯總</returns>
        public UnOffsetAccsum GetFailedUnOffsetAccsum(string errcode, string errmsg)
        {
            UnOffsetAccsum unoffset_qtype_accsum = new UnOffsetAccsum();

            unoffset_qtype_accsum.errcode = errcode;
            unoffset_qtype_accsum.errmsg = errmsg;
            unoffset_qtype_accsum.unoffset_qtype_sum = [];

            return unoffset_qtype_accsum;
        }




        //沖銷原則為［先進先出］，TCNUD排序條件：TDATE、WTYPE、DNO
        public async Task<UnOffsetAccsum> GetUnOffsetService(string bhno, string cseq, string stockSymbol)
        {
            try
            {
                //資料庫內尋找所有的交易紀錄，如果沒找到任何紀錄就會回傳404 Not Found
                Logger.Log(0, "開始", $"開始搜尋{bhno}帳號{cseq}的交易紀錄");
                List<TCNUD> tcnudList = (await _repository.GetByTwoKey(bhno, cseq, stockSymbol)).ToList();
                List<TMHIO> tmhioList = (await _repository.GetByTwoKeyTMHIO(bhno, cseq, stockSymbol)).ToList();
                
                List<TCSIO> tcsioList = (await _repository.GetByTwoKeyTCSIO(bhno, cseq, stockSymbol)).ToList();

                Logger.Log(1, "參數", $"獲取TCNUD與MSTMB - bhno: {bhno}, cseq: {cseq}, stockSymbol: {stockSymbol}");
                var TCNUDList = GetTCNUDList(tcnudList);

                Logger.Log(1, "參數", $"獲取數量為{TCNUDList.Count()}");

                Logger.Log(1, "參數", $"獲取TMHIO與MSTMB - bhno: {bhno}, cseq: {cseq}, stockSymbol: {stockSymbol}");
                var TMHIOList = GetTMHIOList(tmhioList);
                Logger.Log(1, "參數", $"獲取數量為{TMHIOList.Count()}");

                Logger.Log(1, "參數", $"獲取TCSIO與MSTMB - bhno: {bhno}, cseq: {cseq}, stockSymbol: {stockSymbol}");
                var TCSIOList = GetTCSIOList(tcsioList);
                Logger.Log(1, "參數", $"獲取數量為{TCSIOList.Count()}");


                var list = new List<ExtendedTCNUD>();
                if (TMHIOList.Count() == 0 && TCNUDList.Count() == 0 && TCSIOList.Count() == 0)
                {
                    return GetFailedUnOffsetAccsum("404", "未查詢到任何資料");
                }
                list = _util.ConcatLists(TMHIOList, TCNUDList);
                list = _util.ConcatLists(list, TCSIOList);


                var UnOffsetDetailList = GetUnOffsetDetailList(list);
                if (UnOffsetDetailList == null)
                {
                    return GetFailedUnOffsetAccsum("404", "未實現損益 – 個股明細獲取失敗");
                }
                if (UnOffsetDetailList.Count == 0)
                {
                    return GetFailedUnOffsetAccsum("404", $"未找到{bhno}帳號{cseq}的交易紀錄");
                }
                var UnOffsetSumList = GetUnOffsetSumList(UnOffsetDetailList);
                if (UnOffsetSumList == null)
                {
                    return GetFailedUnOffsetAccsum("404", "個股未實現損益獲取失敗");
                }
                var response = GetUnOffsetAccsum(UnOffsetSumList);
                return response;
            }
            //過程中未防呆的部分都會抓到這邊
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"搜尋{bhno}帳號{cseq}的交易紀錄時出現了錯誤：{ex}");
                var response = GetFailedUnOffsetAccsum("500", "Internal Server Error");
                return response;
            }
        }
    }
}
