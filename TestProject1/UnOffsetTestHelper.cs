using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Service.Dtos;

namespace TestProject1
{
    public class UnOffsetTestHelper
    {
        public static List<UnOffsetDetail> GetSampleUnOffsetDetails()
        {
            return new List<UnOffsetDetail>
        {
            new UnOffsetDetail
            {
                stock = "2330",
                stocknm = "台積電",
                tdate = "2024-08-01",
                ttype = "0",
                ttypename = "現買",
                bstype = "B",
                dseq = "001",
                dno = "01",
                bqty = 100,
                mprice = 500,
                cost = 48000,
                lastprice = 550,
                estimateAmt = 55000,
                estimateFee = 55000 * 0.001425m,
                estimateTax = 55000 * 0.003m,
                marketvalue = 55000 - 55000 * 0.001425m - 55000 * 0.003m,
                profit = (55000 - 55000 * 0.001425m - 55000 * 0.003m) - 48000,
                fee = 100,
                tax = 0,
                pl_ratio = (((55000 - 55000 * 0.001425m - 55000 * 0.003m) - 48000) / 48000 * 100).ToString() + "%"
            },
            new UnOffsetDetail
            {
                stock = "2330",
                stocknm = "台積電",
                tdate = "2024-08-02",
                ttype = "0",
                ttypename = "現買",
                bstype = "B",
                dseq = "002",
                dno = "01",
                bqty = 50,
                mprice = 520,
                cost = 26000,
                lastprice = 550,
                estimateAmt = 27500,
                estimateFee = 27500 * 0.001425m,
                estimateTax = 27500 * 0.003m,
                marketvalue = 27500 - 27500 * 0.001425m - 27500 * 0.003m,
                profit = (27500 - 27500 * 0.001425m - 27500 * 0.003m) - 26000,
                fee = 50,
                tax = 0,
                pl_ratio = (((27500 - 27500 * 0.001425m - 27500 * 0.003m) - 26000) / 26000 * 100).ToString() + "%"
            },
            new UnOffsetDetail
            {
                stock = "2317",
                stocknm = "鴻海",
                tdate = "2024-08-01",
                ttype = "0",
                ttypename = "現買",
                bstype = "B",
                dseq = "003",
                dno = "01",
                bqty = 150,
                mprice = 100,
                cost = 15000,
                lastprice = 110,
                estimateAmt = 16500,
                estimateFee = 16500 * 0.001425m,
                estimateTax = 16500 * 0.003m,
                marketvalue = 16500 - 16500 * 0.001425m - 16500 * 0.003m,
                profit = (16500 - 16500 * 0.001425m - 16500 * 0.003m) - 15000,
                fee = 75,
                tax = 0,
                pl_ratio = (((16500 - 16500 * 0.001425m - 16500 * 0.003m) - 15000) / 15000 * 100).ToString() + "%"
            }
        };
        }
        public static List<TMHIO> GetSampleTMHIO()
        {
            return new List<TMHIO>
            {
                new TMHIO
                {
                    TDATE = "20240903",
                    BHNO = "592S",
                    DSEQ = "i0015",
                    JRNUM = "00019137",
                    MTYPE = "T",
                    CSEQ = "0105192",
                    TTYPE = "0",
                    ETYPE = "2",
                    BSTYPE = "B",
                    STOCK = "2603",
                    QTY = 10m,
                    PRICE = 1.5000m,
                    SALES = "0025",
                    ORIGN = "1",
                    MTIME = "090009191",
                    TRDATE = "20221017",
                    TRTIME = "090010",
                    MODDATE = "20221017",
                    MODTIME = "090010",
                    MODUSER = "REPLY"
                },
                new TMHIO
                {
                    TDATE = "20240903",
                    BHNO = "592S",
                    DSEQ = "i0015",
                    JRNUM = "00021151",
                    MTYPE = "T",
                    CSEQ = "0105192",
                    TTYPE = "0",
                    ETYPE = "0",
                    BSTYPE = "B",
                    STOCK = "2603",
                    QTY = 10m,
                    PRICE = 17.5000m,
                    SALES = "0025",
                    ORIGN = "1",
                    MTIME = "090010122",
                    TRDATE = "20221017",
                    TRTIME = "090010",
                    MODDATE = "20221017",
                    MODTIME = "090010",
                    MODUSER = "REPLY"
                }
            };
        }
        public static List<TMHIO> GetRealTMHIO()
        {
            return new List<TMHIO>
            {
                new TMHIO
                {
                    TDATE = "20240903",
                    BHNO = "592S",
                    DSEQ = "i0015",
                    JRNUM = "00019137",
                    MTYPE = "T",
                    CSEQ = "0105192",
                    TTYPE = "0",
                    ETYPE = "0",
                    BSTYPE = "B",
                    STOCK = "2603",
                    QTY = 1000m,
                    PRICE = 147.5000m,
                    SALES = "0025",
                    ORIGN = "1",
                    MTIME = "090009191",
                    TRDATE = "20221017",
                    TRTIME = "090010",
                    MODDATE = "20221017",
                    MODTIME = "090010",
                    MODUSER = "REPLY"
                },
                new TMHIO
                {
                    TDATE = "20240903",
                    BHNO = "592S",
                    DSEQ = "i0015",
                    JRNUM = "00021151",
                    MTYPE = "T",
                    CSEQ = "0105192",
                    TTYPE = "0",
                    ETYPE = "0",
                    BSTYPE = "B",
                    STOCK = "2603",
                    QTY = 2000m,
                    PRICE = 147.5000m,
                    SALES = "0025",
                    ORIGN = "1",
                    MTIME = "090010122",
                    TRDATE = "20221017",
                    TRTIME = "090010",
                    MODDATE = "20221017",
                    MODTIME = "090010",
                    MODUSER = "REPLY"
                }
            };
        }
    }
}
