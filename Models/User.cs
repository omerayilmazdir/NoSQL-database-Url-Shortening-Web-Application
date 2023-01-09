using System.ComponentModel.DataAnnotations;

namespace CoreIdentityWithMongoDB.Models
{
    public class User
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage ="Geçerisiz Email")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
