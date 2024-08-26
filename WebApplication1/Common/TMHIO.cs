using System.Text.Json.Serialization;
using WebApplication1.Common.HCN;

namespace WebApplication1.Common
{
    public class TMHIO
    {
        public string TDATE { get; set; }  
        public string BHNO { get; set; }  
        public string DSEQ { get; set; }  
        public string JRNUM { get; set; }  
        public string MTYPE { get; set; }  
        public string CSEQ { get; set; }  
        public string TTYPE { get; set; }  
        public string STYPE { get; set; }  
        public string BSTYPE { get; set; }  
        public string STOCK { get; set; }  
        public decimal? QTY { get; set; }  
        public decimal PRICE { get; set; }  
        public string? SALES { get; set; }  
        public string? ORIGN { get; set; }  
        public string? MTIME { get; set; }  
        public string? TRDATE { get; set; }  
        public string? TRTIME { get; set; }  
        public string? MODDATE { get; set; }  
        public string? MODTIME { get; set; }  
        public string? MODUSER { get; set; }  
    }
    public class ExtendedTMHIO : TMHIO
    {
        [JsonIgnore]
        public string? CNAME { get; set; }
    }
}
