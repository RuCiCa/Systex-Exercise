using WebApplication1.Common.HCN;
using WebApplication1.Service.Dtos;

namespace WebApplication1.Common
{
    public static class InMemoryCache
    {
        public static List<HCNRH> HCNRHData { get; set; }
        public static List<HCNTD> HCNTDData { get; set; }
        public static List<TCNUD> TCNUDData { get; set; }
        public static List<MSTMB> MSTMBData { get; set; }

        static InMemoryCache()
        {
            HCNRHData = new List<HCNRH>();
            HCNTDData = new List<HCNTD>();
            TCNUDData = new List<TCNUD>();
            MSTMBData = new List<MSTMB>();
        }
    }
}