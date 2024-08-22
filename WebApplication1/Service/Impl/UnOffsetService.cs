using System.Collections.Generic;
using System.ComponentModel;
using WebApplication1.Common;
using WebApplication1.Repositories.Api;
using WebApplication1.Service.Api;
using WebApplication1.Service.Dtos;

namespace WebApplication1.Service.Impl
{
    public class UnOffsetService : IUnOffsetService
    {
        private readonly UnOffsetAccsum _unOffsetAccsum;
        private readonly IRepository _repository;


        public UnOffsetService(UnOffsetAccsum unOffsetAccsum, IRepository repository)
        {
            _unOffsetAccsum = unOffsetAccsum;
            _repository = repository;
        }

        /// <summary>
        /// 獲取未實現損益的個股明細
        /// </summary>
        /// <param name="unOffset">存放TCNUD以及CNAME跟CPRICE</param>
        /// <returns>成功會回傳unOffsetDetail，用來保存</returns>
        public UnOffsetDetail GetUnOffsetDetail(UnOffset unOffset)
        {
            TCNUD tcnud = unOffset.TCNUD;
            Logger.Log(0, "開始", $"開始計算{unOffset.CNAME}之未實現損益 – 個股明細，委託書號{tcnud.DSEQ}-分單號碼{tcnud.DNO}");
            try
            {
                string stock = tcnud.STOCK;
                string stocknm = unOffset.CNAME;
                decimal? bqty = tcnud.BQTY;
                decimal? mprice = tcnud.PRICE;
                decimal? mamt = bqty * mprice;
                decimal? lastprice = unOffset.CPRICE;
                decimal? cost = tcnud.COST;
                decimal? estimateAmt = lastprice * tcnud.BQTY;
                decimal estFee = 0.001425m;
                decimal estTax = 0.003m;
                Logger.Log(2, "變數", $"手續費為：{estFee}, 稅率為：{estTax}");
                decimal? estimateFee = estimateAmt * estFee;
                decimal? estimateTax = estimateAmt * estTax;
                decimal? marketvalue = estimateAmt - estimateFee - estimateTax;
                decimal? profit = marketvalue - cost;
                string pl_ratio;

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
                    tdate = tcnud.TDATE,
                    ttype = "0",
                    ttypename = "現買",
                    bstype = "B",
                    dseq = tcnud.DSEQ,
                    dno = tcnud.DNO,
                    bqty = bqty,
                    mprice = mprice,
                    mamt = mamt,
                    lastprice = lastprice,
                    marketvalue = marketvalue,
                    fee = tcnud.FEE,
                    tax = 0,
                    cost = cost,
                    estimateAmt = estimateAmt,
                    estimateFee = estimateFee,
                    estimateTax = estimateTax,
                    profit = profit,
                    pl_ratio = pl_ratio
                };
                Logger.Log(0, "成功", $"{unOffset.CNAME}之未實現損益 – 個股明細，委託書號{tcnud.DSEQ}-分單號碼{tcnud.DNO}計算成功");
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
        public async Task<List<UnOffsetDetail>> GetUnOffsetDetailList(string bhno, string cseq)
        {
            //建立一個dict，然後使用StringArrayComparer來對作為key的string[]檢查
            var unOffsetDetailList = new List<UnOffsetDetail>();
            var list = (await _repository.GetByTwoKey(bhno, cseq)).ToList();
            if (list.Count == 0)
            {
                Logger.Log(3, "警告", $"資料庫內找不到分公司{bhno}帳號{cseq}的交易紀錄");
                return [] ;
            }
            Logger.Log(0, "成功", $"搜尋到{bhno}帳號{cseq}的{list.Count}筆交易紀錄");

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
                decimal? estimateAmt = list.Sum(t => t.estimateAmt);
                decimal? estimateFee = list.Sum(t => t.estimateFee);
                decimal? estimateTax = list.Sum(t => t.estimateTax);
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
        public async Task<List<UnOffsetSum>> GetUnOffsetSumList(List<UnOffsetDetail> list)
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
        public async Task<UnOffsetAccsum> GetUnOffsetAccsum(List<UnOffsetSum> list)
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
                var estimateAmt = list.Sum(t => t.estimateAmt);
                var estimateFee = list.Sum(t => t.estimateFee);
                var estimateTax = list.Sum(t => t.estimateTax);

                //如果都沒問題就將errcode設為0000，errmsg設為成功
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
        public async Task<UnOffsetAccsum> GetFailedUnOffsetAccsum(string errcode, string errmsg)
        {
            UnOffsetAccsum unoffset_qtype_accsum = new UnOffsetAccsum();

            unoffset_qtype_accsum.errcode = errcode;
            unoffset_qtype_accsum.errmsg = errmsg;
            unoffset_qtype_accsum.unoffset_qtype_sum = [];

            return unoffset_qtype_accsum;
        }
    }
}
