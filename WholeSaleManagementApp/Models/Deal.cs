using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WholeSaleManagementApp.Models
{
    public enum Stage
    {
        A, B, C, D
    }
    public class Deal
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Công ty")]
        public int AccountID { get; set; }
        public int ContactID { get; set; }

        [Display(Name = "Nhân viên")]
        public string OwnerID { set; get; }

        [ForeignKey("OwnerID")]
        [Display(Name = "Nhân viên")]
        public AppUser Owner { set; get; }

        public Contact.Contact Contact { get; set; }

        [ForeignKey("AccountID")]
        [Display(Name = "Công ty")]
        public Account Account { set; get; }

        public Stage? Stage { get; set; }

        public decimal EstimateCost { get; set; }

        public decimal ActualCost { get; set; }

        public ICollection<Quotation> Quotations { get; set; }
    }
}
