using WebApplication1.Common.HCN;
using WebApplication1.Service.Dtos;

namespace WebApplication1.Common
{
    public class InMemoryCache
    {
        public List<MSTMB> MSTMBList { get; set; }
        public List<MSYS> MSYSList { get; set; }
        public Dictionary<string, MSTMB> MSTMBData { get; set; }

        public Dictionary<string, MSYS> MSYSData { get; set; }

        public InMemoryCache(MyDbContext context)
        {
            MSTMBList = context.MSTMBTable.Distinct().ToList();
            MSYSList = context.MSYSTable.Distinct().ToList();
            MSTMBData = MSTMBList
                .GroupBy(x => x.STOCK)
                .Select(g => g.First())
                .ToDictionary(x => x.STOCK, x => x);
            MSYSData = MSYSList
                .GroupBy(x => x.VARNAME)
                .Select(g => g.First())
                .ToDictionary(x => x.VARNAME, x => x);
        }
    }
}