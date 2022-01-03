using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WholeSaleManagementApp.Models.Contact
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [Display(Name = "Contact Name")]
        public string FullName { get; set; }

        [Display(Name ="Công ty")]
        public int? AccountID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public DateTime DateSent { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [StringLength(10)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [StringLength(100)]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [ForeignKey("AccountID")]
        [Display(Name = "Công ty")]
        public Account Account { set; get; }

    }
}
