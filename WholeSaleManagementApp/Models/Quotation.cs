using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WholeSaleManagementApp.Models
{
    public enum Status
    {
        A, B, C
    }
    public class Quotation
    {
        [Key]
        public int Id { get; set; }
        public int DealId { get; set; }
        public int SalePersonId { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public Deal Deal { get; set; }

        public AppUser SalePerson { get; set; }

        public ICollection<Orderline> Orderlines { get; set; }
    }
}
