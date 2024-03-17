
using System.ComponentModel.DataAnnotations;

namespace TimeShareProject.ViewModels
{
    public class ViewUserProfileModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool Sex { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        public string IdfrontImage { get; set; }

        public string IdbackImage { get; set; }

        public string BankAccountNumber { get; set; }

        public string BankAccountHolder { get; set; }

        public string BankName { get; set; }

        public bool Status { get; set; }
    }
}
