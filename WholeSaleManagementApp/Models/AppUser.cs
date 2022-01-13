using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WholeSaleManagementApp.Models
{
    public class AppUser: IdentityUser
    {
        [MaxLength(100)]
        public string FullName { set; get; }

        [MaxLength(255)]
        public string Address { set; get; }

        [DataType(DataType.Date)]
        public DateTime? Birthday { set; get; }

        public string AccountName { get; set; }

        public Company Company { get; set; }

    }
}
