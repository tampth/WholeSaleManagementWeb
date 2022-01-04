using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int AccountID { get; set; }
        public int ContactID { get; set; }

        public int OwnerID { get; set; }

        public Contact.Contact Contact { get; set; }

        public Account Account { get; set; }

        public AppUser Saleperson { get; set; }

        public Stage? Stage { get; set; }

        public decimal EstimateCost { get; set; }

        public decimal ActualCost { get; set; }

        public ICollection<Quotation> Quotations { get; set; }
    }
}
