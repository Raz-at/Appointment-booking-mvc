using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RegistrationAndLogin.Models.Extended
{
    [MetadataType(typeof(AdminLoginMetadata))]
    public partial class AdminLogin
    {
    }

    public class AdminLoginMetadata
    {
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }

        [Display(Name = "FirstName")]
        public string FirstName { get; set;}

        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
   
}