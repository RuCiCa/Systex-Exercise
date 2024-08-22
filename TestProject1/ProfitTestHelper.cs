using System.Collections.Generic;
using WebApplication1.Common.HCN;
using WebApplication1.Service.Dtos;

public class ProfitTestHelper
{
    public static List<ExtendedHCNRH> GetSampleExtendedHCNRH()
    {
        return new List<ExtendedHCNRH>
        {
            new ExtendedHCNRH
            {
                BHNO = "001",
                TDATE = "20240731",
                RDATE = "20240801",
                CSEQ = "A12345",
                BDSEQ = "B001",
                BDNO = "N001",
                SDSEQ = "S001",
                SDNO = "SN001",
                STOCK = "2330",
                CQTY = 100m,
                BPRICE = 500m,
                BFEE = 10m,
                SPRICE = 520m,
                SFEE = 8m,
                TAX = 5m,
                INCOME = 52000m,
                COST = 50000m,
                PROFIT = 2000m,
                WTYPE = "W1",
                CNAME = "台積電"
            },
            new ExtendedHCNRH
            {
                BHNO = "001",
                TDATE = "20240731",
                RDATE = "20240801",
                CSEQ = "A12345",
                BDSEQ = "B002",
                BDNO = "N002",
                SDSEQ = "S002",
                SDNO = "SN002",
                STOCK = "2317",
                CQTY = 200m,
                BPRICE = 100m,
                BFEE = 5m,
                SPRICE = 110m,
                SFEE = 4m,
                TAX = 2m,
                INCOME = 22000m,
                COST = 20000m,
                PROFIT = 2000m,
                WTYPE = "W2",
                CNAME = "鴻海"
            }
        };
    }

    public static List<ExtendedHCNTD> GetSampleExtendedHCNTD()
    {
        return new List<ExtendedHCNTD>
        {
            new ExtendedHCNTD
            {
                BHNO = "001",
                TDATE = "20240801",
                CSEQ = "A12345",
                BDSEQ = "B001",
                BDNO = "N001",
                SDSEQ = "S001",
                SDNO = "SN001",
                STOCK = "2330",
                CQTY = 50m,
                BPRICE = 250m,
                BFEE = 5m,
                SPRICE = 260m,
                SFEE = 4m,
                TAX = 2.5m,
                INCOME = 26000m,
                COST = 25000m,
                PROFIT = 1000m,
                CNAME = "台積電"
            },
            new ExtendedHCNTD
            {
                BHNO = "001",
                TDATE = "20240802",
                CSEQ = "A12345",
                BDSEQ = "B002",
                BDNO = "N002",
                SDSEQ = "S002",
                SDNO = "SN002",
                STOCK = "2317",
                CQTY = 200m,
                BPRICE = 100m,
                BFEE = 5m,
                SPRICE = 110m,
                SFEE = 4m,
                TAX = 2m,
                INCOME = 22000m,
                COST = 20000m,
                PROFIT = 2000m,
                CNAME = "鴻海"
            }
        };
    }
}
