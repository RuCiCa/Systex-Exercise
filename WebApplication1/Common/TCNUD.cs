using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Common
{
    public class TCNUD
    {
        [MaxLength(8)]
        public string TDATE { get; set; }

        [MaxLength(4)]
        public string BHNO { get; set; }

        [MaxLength(7)]
        public string CSEQ { get; set; }

        [MaxLength(6)]
        public string STOCK { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal? PRICE { get; set; }

        [Column(TypeName = "decimal(12, 0)")]
        public decimal? QTY { get; set; }

        [Column(TypeName = "decimal(12, 0)")]
        public decimal? BQTY { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? FEE { get; set; }

        [Column(TypeName = "decimal(16, 2)")]
        public decimal? COST { get; set; }

        [MaxLength(5)]
        public string DSEQ { get; set; }

        [MaxLength(7)]
        public string DNO { get; set; }

        [MaxLength(8)]
        public string? ADJDATE { get; set; }

        [MaxLength(1)]
        public string WTYPE { get; set; }

        [MaxLength(8)]
        public string? TRDATE { get; set; }

        [MaxLength(6)]
        public string? TRTIME { get; set; }

        [MaxLength(8)]
        public string? MODATE { get; set; }

        [MaxLength(6)]
        public string? MODTIME { get; set; }

        [MaxLength(10)]
        public string? MODUSER { get; set; }

        [MaxLength(10)]
        public string? IOFLAG { get; set; }
    }
}
