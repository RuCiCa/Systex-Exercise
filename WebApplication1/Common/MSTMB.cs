using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Common
{
    public class MSTMB
    {
        [Key]
        [MaxLength(6)]
        public string? STOCK { get; set; }

        [MaxLength(32)]
        public string? CNAME { get; set; }

        [MaxLength(30)]
        public string? ENAME { get; set; }

        [MaxLength(1)]
        public string? MTYPE { get; set; }

        [MaxLength(1)]
        public string? STYPE { get; set; }

        [MaxLength(2)]
        public string? SCLASS { get; set; }

        [MaxLength(8)]
        public string? TSDATE { get; set; }

        [MaxLength(8)]
        public string? TEDATE { get; set; }

        [MaxLength(10)]
        public string? CLDATE { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal? CPRICE { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal? TPRICE { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal? BPRICE { get; set; }

        [MaxLength(1)]
        public string? TSTATUS { get; set; }

        [MaxLength(4)]
        public string? BRKNO { get; set; }

        [MaxLength(8)]
        public string? IDATE { get; set; }

        [Column(TypeName = "decimal(8, 6)")]
        public decimal? IRATE { get; set; }

        [Column(TypeName = "decimal(3, 0)")]
        public decimal? IDAY { get; set; }

        [MaxLength(3)]
        public string? CURRENCY { get; set; }

        [MaxLength(3)]
        public string? COUNTRY { get; set; }

        [Column(TypeName = "decimal(6, 0)")]
        public decimal? SHARE { get; set; }

        [MaxLength(1)]
        public string? WARNING { get; set; }

        [MaxLength(1)]
        public string? TMARK { get; set; }

        [MaxLength(1)]
        public string? MFLAG { get; set; }

        [MaxLength(1)]
        public string? WMARK { get; set; }

        [MaxLength(1)]
        public string? TAXTYPE { get; set; }

        [MaxLength(1)]
        public string? PTYPE { get; set; }

        [MaxLength(8)]
        public string? DRDATE { get; set; }

        [MaxLength(8)]
        public string? PDRDATE { get; set; }

        [Column(TypeName = "decimal(13, 8)")]
        public decimal? CDIV { get; set; }

        [Column(TypeName = "decimal(14, 8)")]
        public decimal? SDIV { get; set; }

        [MaxLength(1)]
        public string? CNTDTYPE { get; set; }

        [MaxLength(8)]
        public string? TRDATE { get; set; }

        [MaxLength(6)]
        public string? TRTIME { get; set; }

        [MaxLength(8)]
        public string? MODDATE { get; set; }

        [MaxLength(6)]
        public string? MODTIME { get; set; }

        [MaxLength(10)]
        public string? MODUSER { get; set; }
    }
}
