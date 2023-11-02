using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pink_Panthers_Project.Models
{
    public class Account
    {
        public int ID { get; set; } //Primary Key

        [Required]
        [StringLength(30, ErrorMessage = "Must contain at least 2 characters", MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Must contain at least 2 characters", MinimumLength = 2)]
        public string? LastName { get; set; }

        [StringLength(100, ErrorMessage = "This field is required.", MinimumLength = 5)]
        [RegularExpression(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", ErrorMessage = "This is not a valid email")]
        public string? Email { get; set; } //Not required because its manually checked using other methods

		[StringLength(100, ErrorMessage = "Password must be at least 5 characters.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string? Password { get; set; } //Not required because its manually checked using other methods

        [StringLength(100, ErrorMessage = "Password must be at least 5 characters.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; } //Not required because its manually checked using other methods

		[Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [StringLength(50)]
        public string? AddressLine1 { get; set; }

        [StringLength(50)]
        public string? AddressLine2 { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(30)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? ZipCode { get; set; }

        public string? ProfileLink1 { get; set; }

        public string? ProfileLink2 { get; set; }

        public string? ProfileLink3 { get; set; }



        public string? Salt { get; set; } //Used for hashing and validating passwords. Unique per account

        public bool isTeacher { get; set; } //True for Teacher Account, false for Student Account+

        public double AmountToBePaid { get; set; }


        [NotMapped]
        public List<Notification>? Notifications { get; set; }
    }
}
