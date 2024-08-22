using WebApplication1.Service.Dtos;


namespace WebApplication1.Service.Api
{
    public interface IErrorService
    {
        public Task<ErrorDtos> GetErrorReturn(string errorMsg, string errorCode);
    }
}
