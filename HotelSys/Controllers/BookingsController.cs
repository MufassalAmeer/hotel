using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HotelSys.Models;

namespace HotelSys.Controllers
{
    public class BookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Bookings
        public ActionResult Index()
        {
            return View(db.Bookings.Include("guest").Include("room").Include("room.hotel").ToList());
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            var guets = db.Guests.ToList();
            var rooms = db.Rooms.Include("hotel").ToList();

            ViewBag.guests = new SelectList(guets, "ID", "Name");
            ViewBag.rooms = new SelectList(rooms, "ID", "hotel.HotelName");


            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Booking booking)
        {
            if (ModelState.IsValid)
            {

                var guets = db.Guests.ToList();
                var rooms = db.Rooms.Include("hotel").ToList();

                ViewBag.guests = new SelectList(guets, "ID", "Name");
                ViewBag.rooms = new SelectList(rooms, "ID", "hotel.HotelName");


                var gues = db.Guests.Find(booking.guest.ID);
                var room = db.Rooms.Find(booking.room.ID);


                booking.guest = gues;
                booking.room = room;

                //cost
                var days = (booking.BookingTo - booking.BookingFrom).TotalDays;

                var TotCost = room.Price * Convert.ToDecimal( days);
                booking.Cost = TotCost;


                var ValidateBooking = db.Bookings.Where(b => b.room.ID == booking.room.ID && b.BookingFrom <= booking.BookingFrom && b.BookingTo == booking.BookingTo).ToList();
                if(ValidateBooking.Count != 0)
                {
                    return View(booking);
                }


                



                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,BookingFrom,BookingTo")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult monthlyBookings()
        {
            var results = db.Database.SqlQuery<Monthlysales>("SELECT YEAR(BookingFrom) as year, MONTH(BookingFrom) as month, SUM(Cost) AS Sum    FROM Bookings GROUP BY YEAR(BookingFrom), MONTH(BookingFrom) ORDER BY YEAR(BookingFrom), MONTH(BookingFrom)");

            ViewBag.res = results;
            return View();
        }

         public ActionResult YearlyBookings()
        {
            var results = db.Database.SqlQuery<Monthlysales>("SELECT YEAR(BookingFrom) as year,    SUM(Cost) AS Sum    FROM Bookings GROUP BY YEAR(BookingFrom)");

            ViewBag.res = results;
            return View();
        }
        public ActionResult YearlyBookingsChart()
        {
            var results = db.Database.SqlQuery<Monthlysales>("SELECT YEAR(BookingFrom) as year,    SUM(Cost) AS Sum    FROM Bookings GROUP BY YEAR(BookingFrom)");
            List<string> years = results.Select(i => i.year.ToString()).ToList();
            ViewBag.years = years;

            List<string> sum = results.Select(i => i.Sum.ToString()).ToList();
            ViewBag.sum = sum;

            ViewBag.res = results;

            ViewBag.Data = "Value,Value1,Value2,Value3"; //list of strings that you need to show on the chart. as mentioned in the example from c-sharpcorner
            ViewBag.ObjectName = "Test,Test1,Test2,Test3";
            return View();
        }

        public JsonResult LineChartData()
        {

            var results = db.Database.SqlQuery<Monthlysales>("SELECT YEAR(BookingFrom) as year,    SUM(Cost) AS Sum    FROM Bookings GROUP BY YEAR(BookingFrom)");
            string[] years = results.Select(i => i.year.ToString()).ToArray();
          
           int[] sum = results.Select(i => Convert.ToInt32( i.Sum)).ToArray();
         


            Chart _chart = new Chart();
            //_chart.labels = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "Novemeber", "December" };
            _chart.labels = years;
            _chart.datasets = new List<Datasets>();
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = " Year",
                //data = new int[] { 28, 48, 40, 19, 86, 27, 90, 20, 45, 65, 34, 22 },
                data = sum,
                backgroundColor = new string[] { "#FF0000", "#800000", "#808000", "#008080", "#800080", "#0000FF", "#000080", "#999999", "#E9967A", "#CD5C5C", "#1A5276", "#27AE60" },
                borderColor = new string[] { "#FF0000", "#800000", "#808000", "#008080", "#800080", "#0000FF", "#000080", "#999999", "#E9967A", "#CD5C5C", "#1A5276", "#27AE60" },
                borderWidth = "1"
            });
            _chart.datasets = _dataSet;
            return Json(_chart, JsonRequestBehavior.AllowGet);
        }
        public JsonResult MultiLineChartData()
        {
            var results = db.Database.SqlQuery<Monthlysales>("SELECT YEAR(BookingFrom) as year,    SUM(Cost) AS Sum    FROM Bookings GROUP BY YEAR(BookingFrom)");

            string[] months = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "Novemeber", "December" };

            int[] sum = results.Select(i => Convert.ToInt32(i.Sum)).ToArray();


            Chart _chart = new Chart();
            _chart.labels = months;
            _chart.datasets = new List<Datasets>();
            List<Datasets> _dataSet = new List<Datasets>();
           
            _dataSet.Add(new Datasets()
            {
                label = "Current Year",
                data = new int[] { 28, 48, 40, 19, 86, 27, 90, 20, 45, 65, 34, 22 },
                borderColor = new string[] { "rgba(75,192,192,1)", "#800000", "#808000", "#008080", "#800080", "#0000FF", "#000080", "#999999", "#E9967A", "#CD5C5C", "#1A5276", "#27AE60" },
                backgroundColor = new string[] { "rgba(75,192,192,0.4)" , "#800000", "#808000", "#008080", "#800080", "#0000FF", "#000080", "#999999", "#E9967A", "#CD5C5C", "#1A5276", "#27AE60" },
                borderWidth = "1"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Last Year",
                data = new int[] { 65, 59, 80, 81, 56, 55, 40, 55, 66, 77, 88, 34 },
                borderColor = new string[] { "rgba(75,192,192,1)" },
                backgroundColor = new string[] { "rgba(75,192,192,0.5)" , "#800000", "#808000", "#008080", "#800080", "#0000FF", "#000080", "#999999", "#E9967A", "#CD5C5C", "#1A5276", "#27AE60" },
                borderWidth = "1"
            });
            _chart.datasets = _dataSet;
            return Json(_chart, JsonRequestBehavior.AllowGet);
        }

       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
