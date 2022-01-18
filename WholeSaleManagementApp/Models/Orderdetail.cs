﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WholeSaleManagementApp.Models;

#nullable disable

namespace WholeSalerWeb.Models
{
    public partial class Orderdetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }
    }
}
