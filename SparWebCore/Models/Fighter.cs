using System;

namespace SparWebCore.Models
{
    public class Fighter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public double Height { get; set; }
        public double Weight { get; set; }
        public bool IsSouthpaw { get; set; }
        public int NumberOfAmateurFights { get; set; }
        public int NumberOfProFights { get; set; }
        public int? GymId { get; set; }
        public int? TrainerId { get; set; }
        public string? AspNetUserId { get; set; }
        public bool ProfilePictureUploaded { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsDemo { get; set; }
        public decimal? Rate { get; set; }
        public string? Comments { get; set; }

        public ApplicationUser? SparIdentityUser { get; set; }
    }
}
