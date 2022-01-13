using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WholeSaleManagementApp.Models
{
    public class Deal
    {
        [Key]
        public int Id { get; set; }

        public ICollection<Quotation> Quotations  { get; set; }

        public decimal? Amount { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        public int ContactId { get; set; }

        public Contact Contact { get; set; }

        public int StatusId { get; set; }

        public DealStatus DealStatus { get; set; }

        public DateTime CloseDate { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
