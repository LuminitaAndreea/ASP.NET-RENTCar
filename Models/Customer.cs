using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCWeb.Models
{
    public class Customer
    {
        [Key] public  int CustomerID { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Location {get;set;}
        
    }
}
