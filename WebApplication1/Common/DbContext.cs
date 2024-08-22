using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Common.HCN;

namespace WebApplication1.Common
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<MSTMB> MSTMBTable { get; set; }
        public DbSet<TCNUD> TCNUDTable { get; set; }
        public DbSet<HCNRH> HCNRHTable { get; set; }
        public DbSet<HCNTD> HCNTDTable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TCNUD>()
                .HasKey(c => new { c.BHNO, c.CSEQ, c.TDATE, c.DSEQ, c.DNO });
            modelBuilder.Entity<HCNRH>()
                .HasKey(c => new { c.BHNO, c.TDATE, c.RDATE, c.CSEQ, c.BDSEQ, c.BDNO, c.SDSEQ, c.SDNO });
            modelBuilder.Entity<HCNTD>()
                .HasKey(c => new { c.BHNO, c.TDATE, c.CSEQ, c.BDSEQ, c.BDNO, c.SDSEQ, c.SDNO });

            modelBuilder.Entity<MSTMB>().ToTable("MSTMB");
            modelBuilder.Entity<TCNUD>().ToTable("TCNUD");
            modelBuilder.Entity<HCNRH>().ToTable("HCNRH");
            modelBuilder.Entity<HCNTD>().ToTable("HCNTD");
        }
    }

}
