using System.ComponentModel.DataAnnotations;

namespace LocationCapture.WebAPI.Models
{
    public class LocationForUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [StringLength(256, MinimumLength = 1)]
        [Required]
        public string Name { get; set; }
    }
}
