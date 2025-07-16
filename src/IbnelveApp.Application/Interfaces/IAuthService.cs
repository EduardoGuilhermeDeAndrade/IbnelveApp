using IbnelveApp.Application.DTOs.Auth;
using IbnelveApp.Application.Responses;

namespace IbnelveApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> GerarTokenJwt(LoginDto loginDto);
    }
}


