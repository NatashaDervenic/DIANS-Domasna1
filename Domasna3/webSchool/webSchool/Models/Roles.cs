using System.ComponentModel.DataAnnotations;

namespace webSchool.Models
{
    public class Roles
    {
        [Key]
        public int id { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }

        public Roles (string Role, string UserId)
        {
            this.Role = Role;
            this.UserId = UserId;
        }

        public Roles() { }
    }
}
