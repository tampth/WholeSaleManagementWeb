using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WholeSaleManagementApp.Models;

namespace WholeSalerWeb.Models
{
    public class Cart
    {
        public Product Product { get; set; }
        public int? amount { get; set; }
    }
}
