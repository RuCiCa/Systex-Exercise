namespace WebApplication1.service.dtos
{
    public class Profile
    {
        public string bhno { get; set; } 
        public string cseq { get; set; } 
        public string name { get; set; } 
        public string stock { get; set; } 
        public string stocknm { get; set; } 
        public string mdate { get; set; }  
        public string dseq { get; set; }  
        public string dno { get; set; }  
        public string ttype { get; set; }  
        public string ttypename { get; set; } 
        public string bstype { get; set; }
        public string bstypename { get; set; } 
        public string etype { get; set; } 
        public decimal mprice { get; set; } 
        public decimal mqty { get; set; } 
        public decimal mamt { get; set; }
        public decimal fee { get; set; }
        public decimal tax { get; set; } 
        public decimal netamt { get; set; } 
    }

    public class BillSum
    {
        public decimal cnbamt { get; set; }
        public decimal cnsamt { get; set; }
        public decimal cnfee { get; set; }
        public decimal cntax { get; set; }
        public decimal cnnetamt { get; set; }
        public decimal bqty { get; set; }
        public decimal sqty { get; set; }
    }
    public class ProfileSum
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public decimal netamt { get; set; }
        public decimal fee { get; set; }
        public decimal tax { get; set; }
        public decimal mqty { get; set; }
        public decimal mamt { get; set; }
        public BillSum billSum { get; set; }
        public List<Profile> profile { get; set; }
    }
}
