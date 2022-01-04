using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WholeSaleManagementApp.Models.Contact;

namespace WholeSaleManagementApp.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [Display(Name = "Company name")]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Address { set; get; }

        public ICollection<Contact.Contact> Contacts { get; set; }

        public ICollection<Deal> Deals { get; set; }
    }
}
