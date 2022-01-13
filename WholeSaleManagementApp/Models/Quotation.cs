using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WholeSaleManagementApp.Models
{
    public class Quotation
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int DealId { get; set; }

        public Deal Deal { get; set; }

        public int QuoteStatusId { get; set; }

        public decimal Discount { get; set; }

        public int Total { get; set; }

        public QuoteStatus QuoteStatus { get; set; }

        public ICollection<QuoteLine> QuoteLines { get; set; }
    }
}
