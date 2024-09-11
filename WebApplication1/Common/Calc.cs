using Microsoft.Extensions.Hosting;

namespace WebApplication1.Common
{
    public class Calc
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
        /// 計算總金額
        /// </summary>
        /// <param name="price">單價</param>
        /// <param name="qty">數量</param>
        /// <param name="round">四捨五入的位數</param>
        /// <returns>回傳計算後的總金額</returns>
        public decimal mamtCalc(decimal price, decimal qty, int round = 0)
        {
            return Calculate(price, qty, 1m, round); // rate = 1，代表無附加費率
        }

        /// <summary>
        /// 計算手續費（price * qty * 1.425‰）
        /// </summary>
        /// <param name="price">單價</param>
        /// <param name="qty">數量</param>
        /// <param name="round">四捨五入的位數</param>
        /// <returns>回傳計算後的手續費</returns>
        public decimal feeCalc(decimal price, decimal qty, int round = 0, decimal rate = 1.425m, decimal percent = 1000m)
        {
            decimal feeRate = rate / percent;
            return Calculate(price, qty, feeRate, round);
        }

        /// <summary>
        /// 計算稅金（price * qty * 3‰）
        /// </summary>
        /// <param name="price">單價</param>
        /// <param name="qty">數量</param>
        /// <param name="round">四捨五入的位數</param>
        /// <returns>回傳計算後的稅金</returns>
        public decimal taxCalc(decimal price, decimal qty, int round = 0, decimal rate = 3m, decimal percent = 1000m)
        {
            decimal taxRate = rate / percent; 
            return Calculate(price, qty, taxRate, round);
        }

        public decimal plRatioCalc(decimal profit, decimal cost, int round = 2)
        {
            decimal pl_ratio = 0m;
            if (cost != 0)
            {
                pl_ratio = profit / cost * 100;
                pl_ratio = Math.Round(pl_ratio, round);
            }

            return pl_ratio;
        }
    }


}
