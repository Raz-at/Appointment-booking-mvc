using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RegistrationAndLogin.Models
{
    [MetadataType(typeof(MessageMetadata))]
    public partial class message
    {

    }

    public class MessageMetadata
    {
       
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }

       
        [Display(Name = "From Which Country")]
        public string From { get; set; }

        
        [Display(Name = "To Which Country")]
        public string To { get; set; }

       
        [Display(Name = "Document")]
        public string Message1 { get; set; }

        
        [Display(Name = "Till When")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> EndDate { get; set; }
    }
}