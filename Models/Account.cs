using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebApi.Models
{
    public class Account

    {
        [Required(ErrorMessage = "Please enter FirstName.")]
        public string FirstName { get; set; }
      
        [Required(ErrorMessage = "Please enter LastName.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter Email.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter Country.")]
        public string Country { get; set; }
        public IEnumerable<SelectListItem> Countries { get; set; }

        [Required(ErrorMessage = "Please enter State.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please enter City.")]
        public string City { get; set; }
        [DisplayName("Street")]
        [Required(ErrorMessage = "Please enter Street.")]
        public string Street1 { get; set; }

        [Required(ErrorMessage = "Please enter ZipCode.")]
       // [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter a valid 6-digit zipcode.")]

        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Please enter PhoneNo.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit phone number.")]

        public string PhoneNo { get; set; }
    }
}