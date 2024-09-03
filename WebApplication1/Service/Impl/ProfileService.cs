using System.Collections.Generic;
using WebApplication1.Common;
using WebApplication1.Common.HCN;
using WebApplication1.service.dtos;
using WebApplication1.Service.Api;
using WebApplication1.Service.Dtos;
using WebApplication1.Repositories.Api;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Eventing.Reader;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplication1.Service.Impl
{
    public class ProfileService : IProfileService
    {
        private readonly MyDbContext _context;
        private readonly ProfileSum _profileSum;
        private readonly IRepository _repository;

        public ProfileService(MyDbContext context, ProfileSum profileSum, IRepository repository)
        {
            _context = context;
            _profileSum = profileSum;
            _repository = repository;
        }
        public async Task<List<Profile>> GetProfileList(List<dynamic> tables)
        {
            List<Profile> list = new List<Profile>();
            try
            {
                foreach (var table in tables)
                {
                    string tableType = table is ExtendedTMHIO ? "TMHIO" : "HCMIO";
                    string bhno = table.BHNO;
                    string cseq = table.CSEQ;
                    string name = "空白";
                    string stock = table.STOCK;
                    string stocknm = table.CNAME;
                    string mdate = table.TDATE;
                    string dseq = table.DSEQ;
                    string dno = table is ExtendedTMHIO ? table.JRNUM : table.DNO;
                    string ttype = table is ExtendedTMHIO ? "0" : table.TTYPE;
                    string ttypename = "";
                    string bstype = table.BSTYPE;
                    string bstypename = bstype is "B" ? "買" : "賣";
                    string etype = table is ExtendedTMHIO ? table.ETYPE is "2" ? "1" : "0" : table.ETYPE;
                    decimal mprice = table.PRICE;
                    decimal mqty = table.QTY;
                    decimal mamt = table is ExtendedTMHIO ? mprice * mqty : table.AMT;
                    mamt = Math.Round(mamt);
                    decimal fee = table is ExtendedTMHIO ? mprice * 0.001425m : table.AMT;
                    if (fee < 20)
                    {
                        fee = 20;
                    }
                    else
                    {
                        fee = Math.Round(fee);
                    }
                    decimal tax = table is ExtendedTMHIO ? mprice * mqty * 0.003m : table.TAX;
                    tax = Math.Round(tax);
                    decimal netamt = table is ExtendedTMHIO ? ttype is "B" ? -(mamt + fee) : mamt - fee - tax : table.NETAMT;

                    switch (table, ttype, bstype, table.ETYPE)
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
                            Logger.Log(4, "失敗", $"對帳單 - 明細無法辨別的案例table:{table}, ttype:{ttype}, bstype:{bstype}, etype:{etype}");
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
                return list;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "失敗", $"對帳單 - 明細獲取失敗，錯誤訊息：{ex.Message}");
                return null;
            }
        }
        public async Task<BillSum> GetBillSum(List<Profile> list)
        {
            try
            {
                var buyList = list.Where(t => t.bstype == "B").ToList();
                var sellList = list.Where(t => t.bstype == "S").ToList();

                // 計算買入金額總和
                decimal cnbamt = buyList.Sum(t => t.mamt);
                Logger.Log(1, "資訊", $"買入金額總和 (cnbamt): {string.Join(", ", buyList.Select(t => $"mamt: {t.mamt}"))} -> 總和: {cnbamt}");

                // 計算賣出金額總和
                decimal cnsamt = sellList.Sum(t => t.mamt);
                Logger.Log(1, "資訊", $"賣出金額總和 (cnsamt): {string.Join(", ", sellList.Select(t => $"mamt: {t.mamt}"))} -> 總和: {cnsamt}");

                // 計算手續費總和
                decimal cnfee = list.Sum(t => t.fee);
                Logger.Log(1, "資訊", $"手續費總和 (cnfee): {string.Join(", ", list.Select(t => $"fee: {t.fee}"))} -> 總和: {cnfee}");

                // 計算稅金總和
                decimal cntax = list.Sum(t => t.tax);
                Logger.Log(1, "資訊", $"稅金總和 (cntax): {string.Join(", ", list.Select(t => $"tax: {t.tax}"))} -> 總和: {cntax}");

                // 計算淨金額總和
                decimal cnnetamt = list.Sum(t => t.netamt);
                Logger.Log(1, "資訊", $"淨金額總和 (cnnetamt): {string.Join(", ", list.Select(t => $"netamt: {t.netamt}"))} -> 總和: {cnnetamt}");

                // 計算買入數量總和
                decimal bqty = buyList.Sum(t => t.mqty);
                Logger.Log(1, "資訊", $"買入數量總和 (bqty): {string.Join(", ", buyList.Select(t => $"mqty: {t.mqty}"))} -> 總和: {bqty}");

                // 計算賣出數量總和
                decimal sqty = sellList.Sum(t => t.mqty);
                Logger.Log(1, "資訊", $"賣出數量總和 (sqty): {string.Join(", ", sellList.Select(t => $"mqty: {t.mqty}"))} -> 總和: {sqty}");

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
                return billSum;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "失敗", $"對帳單匯總獲取失敗，錯誤訊息：{ex.Message}");
                return null;
            }
        }
        public async Task<ProfileSum> GetProfileSum(BillSum bill, List<Profile> list)
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
                    errcode = "0000",
                    errmsg = "成功",
                    netamt = netamt,
                    mamt = mamt,
                    fee = fee,
                    tax = tax,
                    mqty = mqty,
                    billSum = billSum,
                    profile = profile
                };

                return profileSum;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "失敗", $"對帳單 - 彙總獲取失敗，錯誤訊息：{ex.Message}");
                return null;
            }
        }
        public async Task<ProfileSum> GetProfileSumFailed(string errcode, string errmsg)
        {
            ProfileSum profileSum = new ProfileSum()
            {
                errcode = errcode,
                errmsg = errmsg
            };

            return profileSum;
        }
        public async Task<List<ExtendedTMHIO>> GetTMHIOList(string bhno, string cseq, string sdate, string edate, string stockSymbol)
        {
            Logger.Log(1, "參數", $"獲取HCNTD table - bhno{bhno}, cseq{cseq}, sdate{sdate}, edate{edate}");
            try
            {
                string todayString = DateTime.Today.ToString("yyyyMMdd");
                if (string.Compare(todayString, sdate) >= 0 && string.Compare(todayString, edate) <= 0)
                {
                    var tmhioList = await _repository.GetByTwoKeyWithTimeForTMHIO(bhno, cseq, sdate, edate, stockSymbol);
                    var mstmbList = InMemoryCache.MSTMBData;
                    var result = (from tmhio in tmhioList
                                  join mstmb in mstmbList on tmhio.STOCK equals mstmb.STOCK
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
                                      CNAME = mstmb.CNAME ?? string.Empty
                                  }).ToList();

                    return result;
                }
                else
                {
                    Logger.Log(1, "參數", $"輸入日期不包含今日，不檢查TNHIO內容");
                    return new List<ExtendedTMHIO>();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"獲取特定分公司特定帳號在時間範圍內的歷史現股當沖失敗, 錯誤訊息: {ex.Message}");
                return null;
            }
        }
        public async Task<List<ExtendedHCMIO>> GetHCMIOList(string bhno, string cseq, string sdate, string edate, string stockSymbol)
        {
            Logger.Log(1, "參數", $"獲取HCNTD table - bhno{bhno}, cseq{cseq}, sdate{sdate}, edate{edate}");
            try
            {
                var hcmioList = await _repository.GetByTwoKeyWithTimeForHCMIO(bhno, cseq, sdate, edate, stockSymbol);
                var mstmbList = InMemoryCache.MSTMBData;
                var result = (from hcmio in hcmioList
                              join mstmb in mstmbList on hcmio.STOCK equals mstmb.STOCK
                              select new ExtendedHCMIO
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
                                  CNAME = mstmb.CNAME ?? string.Empty
                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "錯誤", $"獲取特定分公司特定帳號在時間範圍內的歷史現股當沖失敗, 錯誤訊息: {ex.Message}");
                return null;
            }
        }
    }
}
