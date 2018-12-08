using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelSys.Models
{
    public class Room
    {
        public int ID { get; set; }
        public int RoomPhone { get; set; }
        public roomType roomType { get; set; }
        public BedType bedType { get; set; }
        public Hotel hotel { get; set; }
        public decimal Price { get; set; }
    }

    public enum roomType
    {
        Small,
        Large
    }
    public enum BedType
    {
        Double,
        Single
    }
}