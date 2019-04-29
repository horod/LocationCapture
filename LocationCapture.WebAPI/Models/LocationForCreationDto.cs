using System.ComponentModel.DataAnnotations;

namespace LocationCapture.WebAPI.Models
{
    public class LocationForCreationDto
    {
        [StringLength(256, MinimumLength = 1)]
        [Required]
        public string Name { get; set; }
    }
}
