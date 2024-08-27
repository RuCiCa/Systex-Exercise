using System.Text.Json.Serialization;
using WebApplication1.Common.HCN;

namespace WebApplication1.Service.Dtos
{
    public class ProfitDetailBase
    {
        [JsonIgnore]
        public string stock { get; set; }
        [JsonIgnore]
        public string stocknm { get; set; }
        public string tdate { get; set; }
        public string dseq { get; set; }
        public string dno { get; set; }
        public decimal? mqty { get; set; }
        public decimal? cqty { get; set; }
        public string mprice { get; set; }
        public string mamt { get; set; }
        public decimal? cost { get; set; }
        public decimal? income { get; set; }
        public decimal? netamt { get; set; }
        public decimal? fee { get; set; }
        public decimal? tax { get; set; }
        public string adjdate { get; set; }
        public string ttype { get; set; }
        public string ttypename { get; set; }
        public string bstype { get; set; }
        public string wtype { get; set; }
        public decimal? profit { get; set; }
        public string pl_ratio { get; set; }
        public string ctype { get; set; }
        public string ttypename2 { get; set; }
    }
    public class ProfitDetail : ProfitDetailBase
    {
        public string ioflag { get; set; }
        public string ioname { get; set; }
    }
    public class ProfitDetailOut : ProfitDetailBase
    {
    }
    public class ProfitSum
    {
        public string bhno { get; set; }
        public string cseq { get; set; }
        public string tdate { get; set; }
        public string dseq { get; set; }
        public string dno { get; set; }
        public string ttype { get; set; }
        public string ttypename { get; set; }
        public string bstype { get; set; }
        public string stock { get; set; }
        public string stocknm { get; set; }
        public decimal? cqty { get; set; }
        public string mprice { get; set; }
        public decimal? fee { get; set; }
        public decimal? tax { get; set; }
        public decimal? cost { get; set; }
        public decimal? income { get; set; }
        public decimal? profit { get; set; }
        public string pl_ratio { get; set; }
        public string ctype { get; set; }
        public string ttypename2 { get; set; }
        public List<ProfitDetail> profit_detail { get; set; }
        public ProfitDetailOut profit_detail_out { get; set; }
    }
    public class ProfitAccsum
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public decimal? cqty { get; set; }
        public decimal? cost { get; set; }
        public decimal? income { get; set; }
        public decimal? profit { get; set; }
        public string pl_ratio { get; set; }
        public decimal? fee { get; set; }
        public decimal? tax { get; set; }
        public List<ProfitSum> profit_sum { get; set; }
    }
    public class ProfitDetailSet
    {
        public List<ProfitDetail> profitDetails { get; set; }
        public ProfitDetailOut profitDetailOut { get; set; }
    }
}