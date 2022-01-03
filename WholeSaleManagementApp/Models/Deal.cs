using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WholeSaleManagementApp.Models
{
    public class Deal
    {
        [Key]
        public int Id { get; set; }

        public Contact.Contact Contact { get; set; }

        public Account Account { get; set; }

        public AppUser Salesperson { set; get; }

        public decimal EstimateCost { get; set; }

        public decimal ActualCost { get; set; }

        public ICollection<Quotation> Quotations { get; set; }
    }
}
