using WebApplication1.Service.Api;
using WebApplication1.Service.Dtos;

namespace WebApplication1.Service.Impl
{
    //public delegate void ErrorHandler(string errorCode, string errorMessage);
    public class ErrorService : IErrorService
    {
        public Task<ErrorDtos> GetErrorReturn(string errorMsg, string errorCode)
        {
            ErrorDtos error = new ErrorDtos()
            {
                errorCode = errorCode,
                errorMsg = errorMsg
            };
            return Task.FromResult(error);
        }
    }

}
