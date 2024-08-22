using System.Text.Json.Serialization;
using WebApplication1.Common;

namespace WebApplication1.Service.Dtos
{

    public class UnOffsetAccsum
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public decimal? bqty { get; set; }
        public decimal? cost { get; set; }
        public decimal? marketvalue { get; set; }
        public decimal? profit { get; set; }
        public string pl_ratio { get; set; }
        public decimal? fee { get; set; }
        public decimal? tax { get; set; }
        public decimal? estimateAmt { get; set; }
        public decimal? estimateFee { get; set; }
        public decimal? estimateTax { get; set; }
        public List<UnOffsetSum> unoffset_qtype_sum { get; set; }
    }
    public class UnOffsetSum
    {
        public string stock { get; set; }
        public string stocknm { get; set; }
        public string ttype { get; set; }
        public string ttypename { get; set; }
        public string bstype { get; set; }
        public decimal? bqty { get; set; }
        public decimal? cost { get; set; }
        public decimal? avgprice { get; set; }
        public decimal? lastprice { get; set; }
        public decimal? marketvalue { get; set; }
        public decimal? estimateAmt { get; set; }
        public decimal? estimateFee { get; set; }
        public decimal? estimateTax { get; set; }
        public decimal? profit { get; set; }
        public string pl_ratio { get; set; }
        public decimal? fee { get; set; }
        public decimal? tax { get; set; }
        public decimal? amt { get; set; }
        public List<UnOffsetDetail> unoffset_qtype_detail { get; set; }
    }
    public class UnOffsetDetail
    {
        [JsonIgnore]
        public string stock { get; set; }
        [JsonIgnore]
        public string stocknm { get; set; }
        public string tdate { get; set; }
        public string ttype { get; set; }
        public string ttypename { get; set; }
        public string bstype { get; set; }
        public string dseq { get; set; }
        public string dno { get; set; }
        public decimal? bqty { get; set; }
        public decimal? mprice { get; set; }
        public decimal? mamt { get; set; }
        public decimal? lastprice { get; set; }
        public decimal? marketvalue { get; set; }
        public decimal? fee { get; set; }
        public decimal? tax { get; set; }
        public decimal? cost { get; set; }
        public decimal? estimateAmt { get; set; }
        public decimal? estimateFee { get; set; }
        public decimal? estimateTax { get; set; }
        public decimal? profit { get; set; }
        public string pl_ratio { get; set; }
    }
    public class UnOffset
    {
        public TCNUD TCNUD { get; set; }
        public string CNAME { get; set; }
        public decimal? CPRICE { get; set; }
    }
}
