using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeChat.ViewModels
{
    public class SignUpVM
    {
        [Required]
        [Display(Name="User Name")]
        //[RegularExpression(@"([\s]*[A-Za-z]+[\s]*)*", ErrorMessage = "Please provide valid Name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Mobile Number")]
        //[RegularExpression(@"[0][1][3-9]{1}[0-9]{8}", ErrorMessage = "Please provide valid Mobile Number")]
        [Remote(action: "IsNumberInUse", controller: "Home")]
        public string MobileNumber { get; set; }
        [Required]
        [Display(Name = "Mobile Number")]
        //[RegularExpression(@"[0][1][3-9]{1}[0-9]{8}", ErrorMessage = "Please provide valid Mobile Number")]
        public string UpdateMobileNumber { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        [Remote(action: "IsEmailInUse", controller: "Home")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string UpdateEmail { get; set; }
        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name="Confirm password")]
        [Compare("Password",ErrorMessage="Password and confirm password do not match")]
        public string ConfirmPassword { get; set; }
        public IFormFile PhotoPath { get; set; }
        public string ExistingPhoto { get; set; }
    }
}
