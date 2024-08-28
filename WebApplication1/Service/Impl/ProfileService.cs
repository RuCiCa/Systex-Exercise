using WebApplication1.Common;
using WebApplication1.Common.HCN;
using WebApplication1.service.dtos;
using WebApplication1.Service.Api;
using WebApplication1.Service.Dtos;

namespace WebApplication1.Service.Impl
{
    public interface ProfileService : IProfileService
    {
        Task<List<Profile>> GetProfileList(List<dynamic> tables)
        {
            try
            {
                List<Profile> list = new List<Profile>();

                foreach (var table in tables)
                {
                    string tableType = table is ExtendedTMHIO ? "TMHIO" : "HCMIO";
                    string stock = table.STOCK;
                    string stocknm = table.STOCKnm;
                    string mdate = table.TDATE;
                    string dseq = table.DSEQ;
                    string dno = table.DNO;
                    string ttype = table is ExtendedTMHIO ? "0" : table.TTYPE;


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
                    list.Add(profitDetail);
                }
                return list;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "失敗", $"未實現損益-個股明細資料 (買入)獲取失敗，錯誤訊息：{ex.Message}");
                return null;
            }
        }
        Task<BillSum> GetBillSum(List<dynamic> list);
        Task<ProfileSum> GetProfileSum(BillSum bill, List<Profile> profiles);
        Task<ProfileSum> GetProfileSumFailed(string errorcode, string errormsg);
        Task<List<ExtendedTMHIO>> GetTMHIOList(string bhno, string cseq, string sdate, string edate, string stockSymbol);
        Task<List<ExtendedHCMIO>> GetHCMIOList(string bhno, string cseq, string sdate, string edate, string stockSymbol);
    }
}
