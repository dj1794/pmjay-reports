namespace Pmjay.Web.Data
{
    using Microsoft.JSInterop;

    public class LoginStateService
    {
        private readonly IJSRuntime _js;

        public bool IsLoggedIn { get; private set; }

        public LoginStateService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task Login()
        {
            IsLoggedIn = true;
            await _js.InvokeVoidAsync("localStorage.setItem", "isLoggedIn", "true");
        }

        public async Task LoadState()
        {
            var value = await _js.InvokeAsync<string>("localStorage.getItem", "isLoggedIn");
            IsLoggedIn = value == "true";
        }

        public async Task Logout()
        {
            IsLoggedIn = false;
            await _js.InvokeVoidAsync("localStorage.removeItem", "isLoggedIn");
        }
    }
}
