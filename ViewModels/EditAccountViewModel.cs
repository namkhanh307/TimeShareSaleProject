using System.ComponentModel.DataAnnotations;

namespace TimeShareProject.ViewModels

{
    public class EditAccountViewModel
    {
        [Required]
        public required string CurrentUsername { get; set; }

        [Required]
        public required string CurrentPassword { get; set; }

        [Required]
        public required string NewUsername { get; set; }

        [Required]
        public required string NewPassword { get; set; }
    }
}