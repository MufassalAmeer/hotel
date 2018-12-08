using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelSys.Models
{
    public class Booking
    {
        public int ID { get; set; }
        public Guest guest { get; set; }
        public Room room { get; set; }
        public DateTime BookingFrom { get; set; }
        public DateTime BookingTo { get; set; }
        public decimal Cost { get; set; }


    }
}