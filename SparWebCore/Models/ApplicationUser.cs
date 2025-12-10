using Microsoft.AspNetCore.Identity;

namespace SparWebCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Additional profile fields carried from legacy app
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string GymName { get; set; }
    }
}
