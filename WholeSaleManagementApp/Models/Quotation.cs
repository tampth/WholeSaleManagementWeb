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

        public Deal Deal { get; set; }

        public string Status { get; set; }
    }
}
