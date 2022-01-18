using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WholeSalerWeb.Models;

namespace WholeSaleManagementApp.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }


        [Display(Name = "Tác giả")]
        public string UserId { set; get; }

        [ForeignKey("AuthorId")]
        [Display(Name = "Tác giả")]
        public AppUser User { set; get; }
        public int? TransactStatusId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public bool? Deleted { get; set; }
        public bool? Paid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? PaymentId { get; set; }
        public string Note { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long? Total { get; set; }
        public virtual Transactstatus TransactStatus { get; set; }

        public virtual ICollection<Orderdetail> Orderdetails { get; set; }
    }
}
