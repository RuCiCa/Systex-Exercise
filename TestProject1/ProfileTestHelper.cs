using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Common;

namespace TestProject1
{
    public static class ProfileTestHelper
    {
        public static List<ExtendedTMHIO> GetSampleExtendedTMHIO()
        {
            return new List<ExtendedTMHIO>
            {
                new ExtendedTMHIO
                {
                    TDATE = "20240903",
                    BHNO = "592S",
                    DSEQ = "i0344",
                    JRNUM = "01642962",
                    MTYPE = "T",
                    CSEQ = "0045579",
                    TTYPE = "0",
                    ETYPE = "0",
                    BSTYPE = "B",
                    STOCK = "3041",
                    QTY = 1000m,
                    PRICE = 17.9000m,
                    SALES = "0025",
                    ORIGN = "1",
                    MTIME = "124211977",
                    TRDATE = "20221017",
                    TRTIME = "124212",
                    MODDATE = "20221017",
                    MODTIME = "124212",
                    MODUSER = "REPLY"
                },
                new ExtendedTMHIO
                {
                    TDATE = "20240903",
                    BHNO = "592S",
                    DSEQ = "i0346",
                    JRNUM = "01650103",
                    MTYPE = "T",
                    CSEQ = "0045579",
                    TTYPE = "0",
                    ETYPE = "0",
                    BSTYPE = "S",
                    STOCK = "2426",
                    QTY = 1000m,
                    PRICE = 15.5000m,
                    SALES = "0025",
                    ORIGN = "1",
                    MTIME = "124351289",
                    TRDATE = "20221017",
                    TRTIME = "124351",
                    MODDATE = "20221017",
                    MODTIME = "124351",
                    MODUSER = "REPLY"
                }
            };
        }
        public static List<ExtendedHCMIO> GetSampleExtendedHCMIO()
        {
            return new List<ExtendedHCMIO>
            {
                new ExtendedHCMIO
                {
                    TDATE = "20221005",
                    BHNO = "592S",
                    CSEQ = "0045579",
                    DSEQ = "t0224",
                    DNO = "0000001",
                    WTYPE = "0",
                    STOCK = "2401",
                    TTYPE = "0",
                    ETYPE = "0",
                    BSTYPE = "B",
                    PRICE = 24.0000m,
                    QTY = 1000m,
                    FEE = 34.00m,
                    AMT = 24000.00m,
                    TAX = 0.00m,
                    RVINT = 0m,
                    NETAMT = -24034.00m,
                    DBFEE = 0m,
                    CRAMT = 0.00m,
                    DNAMT = 0.00m,
                    CRINT = 0m,
                    DNINT = 0m,
                    DLFEE = 0m,
                    BFINT = 0.00m,
                    OBAMT = 0m,
                    INTAX = 0,
                    SFCODE = "5920",
                    CDTDQTY = 0m,
                    ORIGN = "s",
                    SALES = "25",
                    DATAFLAG = null,
                    IOFLAG = null,
                    ADJCOST = 0m,
                    ADJDATE = null,
                    STINTAX = 0m,
                    HEALTHFEE = 0m,
                    TRDATE = "20221005",
                    TRTIME = "172751",
                    MODDATE = "20221005",
                    MODTIME = "174655",
                    MODUSER = "DBTRIN"
                },
                new ExtendedHCMIO
                {
                    TDATE = "20221012",
                    BHNO = "592S",
                    CSEQ = "0045579",
                    DSEQ = "i0099",
                    DNO = "0000001",
                    WTYPE = "0",
                    STOCK = "2401",
                    TTYPE = "0",
                    ETYPE = "0",
                    BSTYPE = "B",
                    PRICE = 22.0500m,
                    QTY = 1000m,
                    FEE = 31.00m,
                    AMT = 22050.00m,
                    TAX = 0.00m,
                    RVINT = 0m,
                    NETAMT = -22081.00m,
                    DBFEE = 0m,
                    CRAMT = 0.00m,
                    DNAMT = 0.00m,
                    CRINT = 0m,
                    DNINT = 0m,
                    DLFEE = 0m,
                    BFINT = 0.00m,
                    OBAMT = 0m,
                    INTAX = 0,
                    SFCODE = "5920",
                    CDTDQTY = 0m,
                    ORIGN = "s",
                    SALES = "25",
                    DATAFLAG = null,
                    IOFLAG = null,
                    ADJCOST = 0m,
                    ADJDATE = null,
                    STINTAX = 0m,
                    HEALTHFEE = 0m,
                    TRDATE = "20221012",
                    TRTIME = "172703",
                    MODDATE = "20221012",
                    MODTIME = "174525",
                    MODUSER = "DBTRIN"
                }
            };
        }
    }
}
