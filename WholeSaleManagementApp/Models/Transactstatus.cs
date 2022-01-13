using System;
using System.Collections.Generic;
using WholeSaleManagementApp.Models;

#nullable disable

namespace WholeSalerWeb.Models
{
    public partial class Transactstatus
    {
        public Transactstatus()
        {
            Orders = new HashSet<Order>();
        }

        public int TransactStatusId { get; set; }
        public string Status { get; set; }
        public int? Description { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
