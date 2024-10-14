using Microsoft.Extensions.Hosting;

namespace WebApplication1.Common
{
    public class Util
    {
        /// <summary>
        /// 通用計算方法，根據傳入的費率來計算
        /// </summary>
        /// <param name="price">單價</param>
        /// <param name="qty">數量</param>
        /// <param name="rate">費率，例如手續費是1.425‰，稅金是3‰，總金額是1</param>
        /// <param name="round">四捨五入的位數</param>
        /// <returns>回傳計算後的值</returns>
        public decimal Calculate(decimal price, decimal qty, decimal rate = 1m, int round = 0)
        {
            decimal val = price * qty * rate;
            return Math.Round(val, round);
        }

        /// <summary>
        /// [TMHIO].[ETYPE] 若為「2」零股，請改提供「1」，其他資料則給「0」
        /// </summary>
        /// <param name="etype">單價</param>
        /// <returns>回傳計算後的值</returns>
        public string TransEtpye(string etype)
        {
            if (etype == "2")
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        /// 計算總金額
        /// </summary>
        /// <param name="price">單價</param>
        /// <param name="qty">數量</param>
        /// <param name="round">四捨五入的位數</param>
        /// <returns>回傳計算後的總金額</returns>
        public decimal CalcMamt(decimal price, decimal qty, int round = 0)
        {
            return Calculate(price, qty, 1m, round); // rate = 1，代表無附加費率
        }

        /// <summary>
        /// 計算手續費（price * qty * 1.425‰）
        /// </summary>
        /// <param name="price">單價</param>
        /// <param name="qty">數量</param>
        /// <param name="round">四捨五入的位數，預設是0</param>
        /// <param name="rate">手續費比率，預設是1.425</param>
        /// <param name="percent">決定是多少分比，預設是1000</param>
        /// <param name="etype">用來決定手續費最少收到多少，預設是"0"</param>
        /// <returns>回傳計算後的手續費</returns>
        public decimal CalcFee(decimal price, decimal qty, int round = 0, decimal rate = 1.425m, decimal percent = 1000m, string etype = "0")
        {
            decimal feeRate = rate / percent;
            decimal fee = Calculate(price, qty, feeRate, round);
            if (etype == "1" && fee < 1m)
            {
                fee = 1;
                return fee;
            }
            if (etype == "0" && fee < 20m) {
                fee = 20;
                return fee;
            }

            return fee;
        }

        /// <summary>
        /// 計算稅金（price * qty * 3‰）
        /// </summary>
        /// <param name="price">單價</param>
        /// <param name="qty">數量</param>
        /// <param name="round">四捨五入的位數，預設是0</param>
        /// <param name="rate">稅金比率，預設是3</param>
        /// <param name="percent">決定是多少分比，預設是1000</param>
        /// <returns>回傳計算後的稅金</returns>
        public decimal CalcTax(decimal price, decimal qty, int round = 0, decimal rate = 3m, decimal percent = 1000m)
        {
            decimal taxRate = rate / percent; 
            return Calculate(price, qty, taxRate, round);
        }

        /// <summary>
        /// 獲利率（profit / cost * 100）
        /// </summary>
        /// <param name="profit">單價</param>
        /// <param name="cost">數量</param>
        /// <param name="round">四捨五入的位數，預設是2</param>
        /// <returns>回傳計算後的稅金</returns>
        public decimal CalcPlRatio(decimal profit, decimal cost, int round = 2)
        {
            decimal pl_ratio = 0m;
            if (cost != 0)
            {
                pl_ratio = profit / cost * 100;
                pl_ratio = Math.Round(pl_ratio, round);
            }

            return pl_ratio;
        }

        /// <summary>
        /// 用來取得IOFlagName的function，輸入的IOFlag格式為3位或4位數字，輸入後如果是四位數字且0開頭，會去掉第一個零之後加上IOFLAG進行查詢
        /// </summary>
        /// <param name="IOFLAG">IOFLAG</param>
        /// <param name="MSYSData">存放MSYS的dict</param>
        /// <returns>查詢成功回傳Flag對應的Name，查詢失敗回傳轉換過的IOFLAG</returns>
        public string GetIoflagname(string IOFLAG, Dictionary<string, MSYS> MSYSData)
        {
            if (IOFLAG.StartsWith("0") && IOFLAG.Length >= 4)
            {
                IOFLAG = IOFLAG.Substring(1);
            }
            string inputName = "IOFLAG" + IOFLAG;
            
            if (MSYSData.ContainsKey(inputName))
            {
                return MSYSData[inputName].VALUE;
            }
            return inputName;
        }

        /// <summary>
        /// 用來合併List的function，並且如果其中一個有null也沒問題
        /// </summary>
        /// <param name="list1">第一個list</param>
        /// <param name="list2">第二個list</param>
        /// <returns>合併成功回傳合併好的list，合併失敗回傳null</returns>
        public List<T> ConcatLists<T>(List<T> list1, List<T> list2)
        {
            List<T>? list = (list1, list2) switch
            {
                (null, null) => null,
                (null, _) => list2,
                (_, null) => list1,
                _ => list1.Concat(list2).ToList()
            };
            return list;
        }
    }


}
