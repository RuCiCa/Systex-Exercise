namespace WebApplication1.Common
{
    public delegate void ErrorHandler(string errorCode, string errorMessage);

    public class ErrorReturn
    {
        public string errorCode { set; get; }
        public string errorMsg { set; get; } 
    }
}
