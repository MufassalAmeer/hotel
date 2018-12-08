using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelSys.Models
{
    public class Guest
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string NIC { get; set; }
        public int NoOFAdults { get; set; }
        public int NoOfChildren { get; set; }
    }
}