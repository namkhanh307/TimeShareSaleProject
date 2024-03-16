using System.ComponentModel.DataAnnotations;

namespace TimeShareProject.ViewModels

{
    public class EditAccountViewModel
    {
        [Required]
        public string CurrentUsername { get; set; }

        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewUsername { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}