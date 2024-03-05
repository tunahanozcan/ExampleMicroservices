using Course.Shared.Dtos;
using Course.Web.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace Course.Web.Services.interfaces
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;

        public IdentityService(IOptions<ServiceApiSettings> serviceApiSettings, IOptions<ClientSettings> clientSettings, IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _serviceApiSettings = serviceApiSettings.Value;
            _clientSettings = clientSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }

        public Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            throw new NotImplementedException();
        }

        public Task RevokeRefreshToken()
        {
            throw new NotImplementedException();
        }

        public async Task<Response<bool>> SignIn(SigninInput signinInput)
        {
            var disco=await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.BaseUrl,
                Policy =
                {
                    RequireHttps = false
                }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var passwordTokenRequest=new PasswordTokenRequest
            {
                ClientId = _clientSettings.WebClientUser.ClientId,
                ClientSecret = _clientSettings.WebClientUser.ClientSecret,
                UserName = signinInput.Email,
                Password = signinInput.Password,
                Address = disco.TokenEndpoint
            };

            var token=await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);

            if(token.IsError)
            {
                var responseContent=await token.HttpResponse.Content.ReadAsStringAsync();
                var errorDto=JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Response<bool>.Fail(errorDto.Errors, 400);
            }

            var userInfoRequest=new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = token.AccessToken
            };

            var userInfo=await _httpClient.GetUserInfoAsync(userInfoRequest);

            if(userInfo.IsError)
            {
                throw userInfo.Exception;
            }

            ClaimsIdentity claimsIdentity=new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");
            ClaimsPrincipal claimsPrincipal=new ClaimsPrincipal(claimsIdentity);

            var authenticationProperties=new AuthenticationProperties();

            authenticationProperties.StoreTokens(new List<AuthenticationToken>
            {
                new AuthenticationToken{Name=OpenIdConnectParameterNames.AccessToken, Value=token.AccessToken},
                new AuthenticationToken{Name=OpenIdConnectParameterNames.RefreshToken, Value=token.RefreshToken},
                new AuthenticationToken{Name=OpenIdConnectParameterNames.ExpiresIn, Value=DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            });

            authenticationProperties.IsPersistent=signinInput.IsRemember;

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);

            return Response<bool>.Success(200);

        }
    }
}
