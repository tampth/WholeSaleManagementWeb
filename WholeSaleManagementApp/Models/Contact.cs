using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WholeSaleManagementApp.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; }

        public string CompanyName { get; set; }

        public int? CompanyId { get; set; }
        
        public string FullName { get; set; }

        public string Phone { get; set; }

        public Company Company { get; set; }

        public ICollection<Deal> Deals { get; set; }
     }
}
