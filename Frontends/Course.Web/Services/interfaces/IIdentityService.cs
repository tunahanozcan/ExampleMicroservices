using Course.Shared.Dtos;
using Course.Web.Models;
using IdentityModel.Client;

namespace Course.Web.Services.interfaces
{
    public interface IIdentityService
    {
        Task<Response<bool>> SignIn(SigninInput signinInput);
        Task<TokenResponse> GetAccessTokenByRefreshToken();
        Task RevokeRefreshToken();
    }
}
