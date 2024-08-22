﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication1.Common.HCN
{
    public class HCNTD
    {
        [Key]
        [MaxLength(4)]
        public string BHNO { get; set; }

        [MaxLength(8)]
        public string TDATE { get; set; }

        [MaxLength(7)]
        public string CSEQ { get; set; }

        [MaxLength(5)]
        public string BDSEQ { get; set; }

        [MaxLength(7)]
        public string SDSEQ { get; set; }

        [MaxLength(5)]
        public string BDNO { get; set; }

        [MaxLength(7)]
        public string SDNO { get; set; }

        [MaxLength(6)]
        public string STOCK { get; set; }

        [Column(TypeName = "decimal(12, 0)")]
        public decimal? CQTY { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal? BPRICE { get; set; }

        [Column(TypeName = "decimal(8, 0)")]
        public decimal? BFEE { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal? SPRICE { get; set; }

        [Column(TypeName = "decimal(8, 0)")]
        public decimal? SFEE { get; set; }

        [Column(TypeName = "decimal(8, 0)")]
        public decimal? TAX { get; set; }

        [Column(TypeName = "decimal(14, 0)")]
        public decimal? INCOME { get; set; }

        [Column(TypeName = "decimal(14, 0)")]
        public decimal? COST { get; set; }

        [Column(TypeName = "decimal(14, 0)")]
        public decimal? PROFIT { get; set; }

        [Column(TypeName = "decimal(12, 0)")]
        public decimal? BQTY { get; set; }

        [Column(TypeName = "decimal(12, 0)")]
        public decimal? SQTY { get; set; }

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
    public class ExtendedHCNTD : HCNTD
    {
        [JsonIgnore]
        public string? CNAME { get; set; }
    }
}
