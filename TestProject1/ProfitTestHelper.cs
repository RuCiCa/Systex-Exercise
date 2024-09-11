using System.Collections.Generic;
using WebApplication1.Common.HCN;

public class ProfitTestHelper
{
    public static List<ExtendedHCNRH> GetSampleExtendedHCNRH()
    {
        return new List<ExtendedHCNRH>
        {
            new ExtendedHCNRH
            {
                BHNO = "592S",
                TDATE = "20210104",
                RDATE = "20201228",
                CSEQ = "0116453",
                BDSEQ = "i0495",
                BDNO = "0000025",
                SDSEQ = "t0146",
                SDNO = "0000005",
                STOCK = "2337",
                CQTY = 1000m,
                BPRICE = 43.8000m,
                BFEE = 62.00m,
                SPRICE = 41.8500m,
                SFEE = 60.00m,
                TAX = 126.00m,
                INCOME = 41664.00m,
                COST = 43862.00m,
                PROFIT = -2198.00m,
                BQTY = 2000m,
                SQTY = 5000m,
                STINTAX = 0m,
                TRDATE = null,
                TRTIME = null,
                MODDATE = null,
                MODTIME = null,
                MODUSER = null,
                ADJDATE = null,
                WTYPE = "0",
                IOFLAG = null,
                CNAME = null
            },
            new ExtendedHCNRH
            {
                BHNO = "592S",
                TDATE = "20210104",
                RDATE = "20201228",
                CSEQ = "0116453",
                BDSEQ = "j0367",
                BDNO = "0000020",
                SDSEQ = "t0146",
                SDNO = "0000005",
                STOCK = "2337",
                CQTY = 2000m,
                BPRICE = 43.4500m,
                BFEE = 123.00m,
                SPRICE = 41.8500m,
                SFEE = 119.00m,
                TAX = 251.00m,
                INCOME = 83330.00m,
                COST = 87023.00m,
                PROFIT = -3693.00m,
                BQTY = 2000m,
                SQTY = 5000m,
                STINTAX = 0m,
                TRDATE = null,
                TRTIME = null,
                MODDATE = null,
                MODTIME = null,
                MODUSER = null,
                ADJDATE = null,
                WTYPE = "0",
                IOFLAG = null,
                CNAME = null
            },
            new ExtendedHCNRH
            {
                BHNO = "592S",
                TDATE = "20210104",
                RDATE = "20201228",
                CSEQ = "0116453",
                BDSEQ = "j0491",
                BDNO = "0000024",
                SDSEQ = "t0146",
                SDNO = "0000005",
                STOCK = "2337",
                CQTY = 1000m,
                BPRICE = 43.9500m,
                BFEE = 62.00m,
                SPRICE = 41.8500m,
                SFEE = 60.00m,
                TAX = 125.00m,
                INCOME = 41665.00m,
                COST = 44012.00m,
                PROFIT = -2347.00m,
                BQTY = 1000m,
                SQTY = 5000m,
                STINTAX = 0m,
                TRDATE = null,
                TRTIME = null,
                MODDATE = null,
                MODTIME = null,
                MODUSER = null,
                ADJDATE = null,
                WTYPE = "0",
                IOFLAG = null,
                CNAME = null
            }
        };
    }

    public static List<ExtendedHCNTD> GetSampleExtendedHCNTD()
    {
        return new List<ExtendedHCNTD>
        {
            new ExtendedHCNTD
            {
                BHNO = "592S",
                TDATE = "20210104",
                CSEQ = "0029621",
                BDSEQ = "i0453",
                BDNO = "j0564",
                SDSEQ = "0000016",
                SDNO = "0000020",
                STOCK = "8069",
                CQTY = 1000m,
                BPRICE = 49.2500m,
                BFEE = 70m,
                SPRICE = 48.2500m,
                SFEE = 68m,
                TAX = 72m,
                INCOME = 48110m,
                COST = 49320m,
                PROFIT = -1210m,
                BQTY = 1000m,
                SQTY = 29000m,
                TRDATE = "20210104",
                TRTIME = "175407",
                MODDATE = null,
                MODTIME = null,
                MODUSER = null,
                CNAME = null
            },
            new ExtendedHCNTD
            {
                BHNO = "592S",
                TDATE = "20210104",
                CSEQ = "0029621",
                BDSEQ = "i0464",
                BDNO = "j0564",
                SDSEQ = "0000017",
                SDNO = "0000020",
                STOCK = "8069",
                CQTY = 1000m,
                BPRICE = 49.0500m,
                BFEE = 69m,
                SPRICE = 48.2500m,
                SFEE = 68m,
                TAX = 72m,
                INCOME = 48110m,
                COST = 49119m,
                PROFIT = -1009m,
                BQTY = 1000m,
                SQTY = 29000m,
                TRDATE = "20210104",
                TRTIME = "175407",
                MODDATE = null,
                MODTIME = null,
                MODUSER = null,
                CNAME = null
            },
            new ExtendedHCNTD
            {
                BHNO = "592S",
                TDATE = "20210104",
                CSEQ = "0029621",
                BDSEQ = "i0535",
                BDNO = "j0564",
                SDSEQ = "0000019",
                SDNO = "0000020",
                STOCK = "8069",
                CQTY = 1000m,
                BPRICE = 48.8500m,
                BFEE = 69m,
                SPRICE = 48.2500m,
                SFEE = 68m,
                TAX = 72m,
                INCOME = 48110m,
                COST = 48919m,
                PROFIT = -809m,
                BQTY = 1000m,
                SQTY = 29000m,
                TRDATE = "20210104",
                TRTIME = "175407",
                MODDATE = null,
                MODTIME = null,
                MODUSER = null,
                CNAME = null
            }
        };
    }
    public static List<ExtendedHCNTD> GetOneExtendedHCNTD()
    {
        return new List<ExtendedHCNTD>
        {
            new ExtendedHCNTD
            {
                BHNO = "592S",
                TDATE = "20210104",
                CSEQ = "0029621",
                BDSEQ = "i0453",
                BDNO = "j0564",
                SDSEQ = "0000016",
                SDNO = "0000020",
                STOCK = "8069",
                CQTY = 1000m,
                BPRICE = 49.2500m,
                BFEE = 70m,
                SPRICE = 48.2500m,
                SFEE = 68m,
                TAX = 72m,
                INCOME = 48110m,
                COST = 49320m,
                PROFIT = -1210m,
                BQTY = 1000m,
                SQTY = 29000m,
                TRDATE = "20210104",
                TRTIME = "175407",
                MODDATE = null,
                MODTIME = null,
                MODUSER = null,
                CNAME = null
            }
        };
    }
}
