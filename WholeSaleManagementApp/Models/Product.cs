using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace WholeSaleManagementApp.Models
{
    public partial class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public int? CatId { get; set; }
        public int? Price { get; set; }
        public string Thumb { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public int? UnitslnStock { get; set; }

        public virtual Category Cat { get; set; }
    }
}
