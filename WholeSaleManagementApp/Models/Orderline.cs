using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WholeSaleManagementApp.Models
{
    public class Orderline
    {
        [Key]
        public int Id { get; set; }

        public int QuotationId { get; set; }

        public int ProductId { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal  Discount { get; set; }

        public Product Product { get; set; }
    }
}
