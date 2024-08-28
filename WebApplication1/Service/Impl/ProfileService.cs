using System.Collections.Generic;
using WebApplication1.Common;
using WebApplication1.Common.HCN;
using WebApplication1.service.dtos;
using WebApplication1.Service.Api;
using WebApplication1.Service.Dtos;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Service.Impl
{
    public class ProfileService : IProfileService
    {
        Task<List<Profile>> GetProfileList(List<dynamic> tables)
        {
            List<Profile> list = new List<Profile>();
            try
            {
                foreach (var table in tables)
                {
                    string tableType = table is ExtendedTMHIO ? "TMHIO" : "HCMIO";
                    string bhno = table.BHNO;
                    string cseq = table.CSEQ;
                    string name = table.NAME;
                    string stock = table.STOCK;
                    string stocknm = table.STOCKnm;
                    string mdate = table.TDATE;
                    string dseq = table.DSEQ;
                    string dno = table.DNO;
                    string ttype = table is ExtendedTMHIO ? "0" : table.TTYPE;
                    string ttypename = "";
                    string bstype = table.BTYPE;
                    string bstypename = bstype is "B" ? "買" : "賣";
                    string etype = table is ExtendedTMHIO ? table.ETYPE is "2" ? "1" : "0" : table.ETYPE;
                    decimal mprice = table.MPRICE;
                    decimal mqty = table.QTY;
                    decimal mamt = table is ExtendedTMHIO ? mprice * mqty : table.AMT;
                    decimal fee = table is ExtendedTMHIO ? mprice * 0.001425m : table.AMT;
                    decimal tax = table is ExtendedTMHIO ? mprice * mqty * 0.003m : table.TAX;
                    decimal netamt = table is ExtendedTMHIO ? ttype is "B" ? -(mamt + fee) : mamt - fee - tax : table.NETAMT;

                    switch (table, ttype, bstype, etype)
                    {
                        case (ExtendedHCMIO, "0", "B", _):
                            ttypename = "現買";
                            break;
                        case (ExtendedHCMIO, "0", "S", _):
                            ttypename = "現賣";
                            break;
                        case (ExtendedTMHIO, "0", "B", "0"):
                            ttypename = "現買";
                            break;
                        case (ExtendedTMHIO, "0", "B", "2"):
                            ttypename = "盤後零買";
                            break;
                        case (ExtendedTMHIO, "0", "B", "5"):
                            ttypename = "盤中零買";
                            break;
                        case (ExtendedTMHIO, "0", "S", "0"):
                            ttypename = "現賣";
                            break;
                        case (ExtendedTMHIO, "0", "S", "2"):
                            ttypename = "盤後零賣";
                            break;
                        case (ExtendedTMHIO, "0", "S", "5"):
                            ttypename = "盤中零賣";
                            break;
                        default:
                            return null;
                    }

                    Profile profile = new Profile()
                    {
                        bhno = bhno,
                        cseq = cseq,
                        name = name,
                        stock = stock,
                        stocknm = stocknm,
                        mdate = mdate,
                        dseq = dseq,
                        dno = dno,
                        ttype = ttype,
                        ttypename = ttypename,
                        bstype = bstype,
                        bstypename = bstypename,
                        etype = etype,
                        mprice = mprice,
                        mqty = mqty,
                        mamt = mamt,
                        fee = fee,
                        tax = tax,
                        netamt = netamt
                    };
                    list.Add(profile);
                }
                return Task.FromResult(list);
            }
            catch (Exception ex)
            {
                Logger.Log(4, "失敗", $"未實現損益-個股明細資料 (買入)獲取失敗，錯誤訊息：{ex.Message}");
                return null;
            }
        }
        Task<BillSum> GetBillSum(List<Profile> list)
        {
            try
            {
                var buyList = list.Where(t => t.bstype == "B").ToList();
                var sellList = list.Where(t => t.bstype == "S").ToList();

                decimal cnbamt = buyList.Sum(t => t.mamt);
                decimal cnsamt = sellList.Sum(t => t.mamt);
                decimal cnfee = list.Sum(t => t.fee);
                decimal cntax = list.Sum(t => t.tax);
                decimal cnnetamt = list.Sum(t => t.netamt);
                decimal bqty = buyList.Sum(t => t.mqty);
                decimal sqty = sellList.Sum(t => t.mqty);

                BillSum billSum = new BillSum()
                {
                    cnbamt = cnbamt, 
                    cnsamt = cnsamt, 
                    cnfee = cnfee,
                    cntax = cntax,
                    cnnetamt = cnnetamt,
                    bqty = bqty, 
                    sqty = sqty
                };
                return Task.FromResult(billSum);
            }
            catch (Exception)
            {
                return null;
            }
        }
        Task<ProfileSum> GetProfileSum(BillSum bill, List<Profile> list)
        {
            try
            {
                decimal netamt = list.Sum(t => t.netamt);
                decimal mamt = list.Sum(t => t.mamt);
                decimal fee = list.Sum(t => t.fee);
                decimal tax = list.Sum(t => t.tax);
                decimal mqty = list.Sum(t => t.mqty);
                BillSum billSum = bill;
                List<Profile> profile = list;

                ProfileSum profileSum = new ProfileSum()
                {
                    netamt = netamt,
                    mamt = mamt,
                    fee = fee,
                    tax = tax,
                    mqty = mqty,
                    billSum = billSum,
                    profile = profile
                };

                return Task.FromResult(profileSum);
            }
            catch (Exception)
            {
                return null;
            }
        }
        Task<ProfileSum> GetProfileSumFailed(string errcode, string errmsg)
        {
            ProfileSum profileSum = new ProfileSum()
            {
                errcode = errcode,
                errmsg = errmsg
            };

            return profileSum;
        }
        Task<List<ExtendedTMHIO>> GetTMHIOList(string bhno, string cseq, string sdate, string edate, string stockSymbol);
        Task<List<ExtendedHCMIO>> GetHCMIOList(string bhno, string cseq, string sdate, string edate, string stockSymbol);
    }
}
