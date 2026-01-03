using Microsoft.JSInterop;
using Pmjay.Web.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


public class LoginStateService
{
    private readonly ApiAuthenticationStateProvider _authProvider;
    private readonly IJSRuntime _js;

    public LoginResponse? CurrentUser { get; private set; }
    public bool IsAuthenticated { get; private set; }

    public LoginStateService(
        ApiAuthenticationStateProvider authProvider,
        IJSRuntime js)
    {
        _authProvider = authProvider;
        _js = js;
    }

    public async Task SetLogin(LoginResponse user)
    {
        CurrentUser = user;
        IsAuthenticated = true;

        await _authProvider.MarkUserAsAuthenticated(user.AccessToken);
    }

    // 🔑 RESTORE FROM JWT
    public async Task LoadState()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "token");

        if (string.IsNullOrWhiteSpace(token))
            return;

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        CurrentUser = new LoginResponse
        {
            UserId = int.Parse(jwt.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value),
            FullName = jwt.Claims.First(x => x.Type == ClaimTypes.Name).Value,
            RoleName = jwt.Claims.First(x => x.Type == ClaimTypes.Role).Value,
            IsAuthenticated = true,
            AccessToken = token
        };

        IsAuthenticated = true;
    }

    public async Task Logout()
    {
        CurrentUser = null;
        IsAuthenticated = false;

        await _authProvider.Logout();
    }
}
