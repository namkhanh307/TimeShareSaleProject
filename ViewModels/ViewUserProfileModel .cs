
using System.ComponentModel.DataAnnotations;

namespace TimeShareProject.ViewModels
{
    public class ViewUserProfileModel
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public bool Sex { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public required string PhoneNumber { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Address { get; set; }

        public string IdfrontImage { get; set; } = string.Empty;

        public string IdbackImage { get; set; } = string.Empty;

        public string BankAccountNumber { get; set; } = string.Empty;

        public string BankAccountHolder { get; set; } = string.Empty;

        public string BankName { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}
