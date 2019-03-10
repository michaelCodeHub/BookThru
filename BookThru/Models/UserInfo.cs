using BookThru.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookThru.Models
{
    public class UserInfo
    {
        [Key]
        public string Id { get; set; }
        
        [Required]
        [Display(Name = "First Name"), StringLength(50, MinimumLength = 1, ErrorMessage = "FirstName cannot be longer than 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name"), StringLength(50, MinimumLength = 1, ErrorMessage = "LastName cannot be longer than 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You must provide a valid contact number")]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone number")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Browse Picture")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Only Image files allowed.")]
        public string PictureUrl { get; set; }
        
        [ForeignKey("Id")]
        public virtual BookThruUser BookThruUser { get; set; }
        

        public string GetName()
        {
            return FirstName + " " + LastName;
        }
    }
}
