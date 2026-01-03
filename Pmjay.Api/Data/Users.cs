using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pmjay.Api.Data
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public int RoleId { get; set; }

        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // 🔥 Navigation property
        public Role Role { get; set; }
    }
}
