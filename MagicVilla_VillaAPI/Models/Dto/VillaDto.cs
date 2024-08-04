using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
        public static string Details { get; set; } = string.Empty;
        [Required]
        public double Rate { get; set; } = 0.0;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
