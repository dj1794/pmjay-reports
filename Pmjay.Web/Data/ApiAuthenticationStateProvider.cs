using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;

    public ApiAuthenticationStateProvider(IJSRuntime js)
    {
        _js = js;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "token");

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity())
            );
        }

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        // Normalize role and name claim types so Blazor's role checks work
        var claims = jwt.Claims.Select(c =>
        {
            if (string.Equals(c.Type, "role", StringComparison.OrdinalIgnoreCase)
                || c.Type?.EndsWith("/role", StringComparison.OrdinalIgnoreCase) == true)
            {
                return new Claim(ClaimTypes.Role, c.Value);
            }

            if (string.Equals(c.Type, "name", StringComparison.OrdinalIgnoreCase))
            {
                return new Claim(ClaimTypes.Name, c.Value);
            }

            if (string.Equals(c.Type, "sub", StringComparison.OrdinalIgnoreCase))
            {
                return new Claim(ClaimTypes.NameIdentifier, c.Value);
            }

            return c;
        }).ToList();

        var identity = new ClaimsIdentity(
            claims,
            "jwt",
            ClaimTypes.Name,
            ClaimTypes.Role
        );

        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }


    public async Task MarkUserAsAuthenticated(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", "token", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Logout()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "token");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}