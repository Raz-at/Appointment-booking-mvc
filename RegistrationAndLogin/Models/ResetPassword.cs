using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RegistrationAndLogin.Models
{
    public class ResetPassword
    {
        [Required(AllowEmptyStrings =false , ErrorMessage ="Password is required")]
        [DataType(DataType.Password)]
        public string NewPassword{get;set;}

        [Compare("NewPassword" , ErrorMessage ="Password doesnt match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword{get;set;}

        [Required]
        public string ResetCode{get;set;}
        
    }
}