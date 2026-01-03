namespace Pmjay.Web.Data
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsAuthenticated { get; set; }
        public string AccessToken { get; set; }
    }
}