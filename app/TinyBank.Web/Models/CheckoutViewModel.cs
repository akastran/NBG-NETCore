using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBank.Web.Models
{
    public class CheckoutViewModel
    {
        public Guid CardId { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public double Amount { get; set; }
    }
}
