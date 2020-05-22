using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCWeb.Models
{
    public class Cupon
    {
        [Key] public string CouponCode { set; get; }
        public string Description { get; set; }
        public float Discount { get; set; }
    }
}
