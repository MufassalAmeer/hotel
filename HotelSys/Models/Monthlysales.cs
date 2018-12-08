using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelSys.Models
{
    public class Monthlysales
    {
        public int year { get; set; }
        public int month { get; set; }
        public decimal Sum { get; set; }
    }
}