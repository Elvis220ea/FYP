using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FYP.Models;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FYP.Controllers
{
    public class PetHotelController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<PHBooking> model = DBUtl.GetList<PHBooking>(
                                                 @"SELECT * FROM PHBooking 
                                                WHERE BookedBy = {0}",
                                                       userid);
            return View(model);
        }

        [Authorize]
        public IActionResult BookingAdd()
        {
            var petTypes = DBUtl.GetList("SELECT Id, Description " +
                "FROM PHPetType ORDER BY Description");
            ViewData["PetTypes"] = new SelectList(petTypes, "Id", "Description");

            ViewData["PostTo"] = "AddBooking";
            ViewData["ButtonText"] = "Add";
            return View("Booking");
        }

        [HttpPost]
        [Authorize]
        public IActionResult BookingAdd(PHBooking newBook)
        {
            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (ModelState.IsValid)
            {
                string sql = @"INSERT INTO PHBooking 
                                    (NRIC, OwnerName, PetName, Days, PetTypeId, 
                                     FeedFreq, FTCanned, FTDry, FtSoft, CheckInDate, BookedBy) 
                                    VALUES ('{0}', '{1}', '{2}', {3}, {4}, 
                                            {5},'{6}','{7}','{8}','{9}',{10})";

                if (DBUtl.ExecSQL(sql,
                                    newBook.NRIC, newBook.OwnerName, newBook.PetName,
                                    newBook.Days, newBook.PetTypeId, newBook.FeedFreq,
                                    newBook.FTCanned, newBook.FTDry, newBook.FTSoft,
                                    $"{newBook.CheckInDate:yyyy-MM-dd}", userid) == 1)
                    TempData["Msg"] = "New booking added.";
                else
                    TempData["Msg"] = "Failed to add new booking.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Msg"] = "Invalid information entered!";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public IActionResult BookingEdit(int Id)
        {

            // Implement your code here(Done)
            var petTypes = DBUtl.GetList("SELECT Id, Description FROM PHPetType ORDER BY Description");
            ViewData["petTypes"] = new SelectList(petTypes, "Id", "Description");

            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<PHBooking> lstBooking = DBUtl.GetList<PHBooking>("SELECT * FROM PHBooking WHERE Id = {0} and BookedBy={1}",
                Id, userid);

            if (lstBooking.Count > 0)
            {
                PHBooking model = lstBooking[0];
                return View(model);
            }
            else
            {
                TempData["Msg"] = $"Booking {Id} not found!";
                return RedirectToAction("Index");
            }

          
        }

        [HttpPost]
        [Authorize]
        public IActionResult BookingEdit(PHBooking uBook)
        {
            // Implement your code here(Done)
            if (ModelState.IsValid)
            {
                string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (DBUtl.ExecSQL(@"UPDATE PHBooking
                    SET NRIC='{0}', OwnerName='{1}', PetName='{2}', Days={3}, PetTypeId={4},
                    FeedFreq={5}, FTCanned='{6}', FTDry='{7}', FTSoft='{8}', CheckInDate='{9}', Cost={10}
                    WHERE Id = {11} AND BookedBy={12}",

                    uBook.NRIC, uBook.OwnerName, uBook.PetName, uBook.Days, uBook.PetTypeId,
                    uBook.FeedFreq, uBook.FTCanned, uBook.FTDry, uBook.FTSoft, $"{uBook.CheckInDate:dd MMMM yyyy}",
                    uBook.Cost, uBook.Id, userid) == 1)
                    TempData["Msg"] = $"Booking {uBook.Id} updated.";
                else
                    TempData["Msg"] = DBUtl.DB_Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Msg"] = "Invalid information entered!";
                return RedirectToAction("Index");
            }

        }


        [Authorize]
        public IActionResult BookingDelete(int Id)
        {
            // Implement your code here
            var packageTypes = DBUtl.GetList("SELECT Id, Description FROM PHPetType ORDER BY Description");
            ViewData["PackageTypes"] = new SelectList(packageTypes, "Id", "Description");

            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<PHBooking> lstBooking = DBUtl.GetList<PHBooking>("SELECT * FROM PHBooking WHERE Id = {0} AND BookedBy={1}", Id, userid);

            if (lstBooking.Count > 0)
            {
                PHBooking model = lstBooking[0];
                return View("BookingDelete", model);
            }
            else
            {
                TempData["Msg"] = $"Booking {Id} not found!";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [Authorize]
        public IActionResult BookingDelete(PHBooking uBook)
        {
            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (DBUtl.ExecSQL(@"DELETE PHBooking 
                                    WHERE Id = {0} AND BookedBy={1}",
                                uBook.Id, userid) == 1)
                TempData["Msg"] = $"Booking {uBook.Id} deleted.";
            else
                TempData["Msg"] = DBUtl.DB_Message;
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult ViewBookingsByPetTypes()
        {
            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewData["PetTypes"] =
                        DBUtl.GetList(@"SELECT * 
                                      FROM PHPetType 
                                     ORDER BY Id");

            List<PHBooking> model = DBUtl.GetList<PHBooking>("SELECT * FROM PHBooking WHERE BookedBy = {0}", userid);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SalesSummary(int ryear, int rmonth)
        {
            List<PHBooking> data = null;

            ViewData["ryear"] = ryear;
            ViewData["rmonth"] = rmonth;

            if (ryear <= 0)
            {
                data = DBUtl.GetList<PHBooking>(@"SELECT * FROM PHBooking");
                ViewData["reportheader"] = "Overall Sales Summary by Year";

                var model = data.GroupBy(b => b.CheckInDate.Year)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new
                    {
                        Group = g.Key,
                        Total = g.Sum(b => b.Cost),
                        Average = g.Average(b => b.Cost),
                        Lowest = g.Min(b => b.Cost),
                        Highest = g.Max(b => b.Cost)
                    }).ToExpandoList();

                return View(model);
    
            }
            else if (rmonth <= 0 || rmonth > 12)
            {
                data = DBUtl.GetList<PHBooking>(@"SELECT * FROM PHBooking
                                                            WHERE YEAR(CheckInDate) = {0}", ryear);
                ViewData["reportheader"] = $"Annual Sales Summary for {ryear} by Month";
          
                var model = data.GroupBy(b => b.CheckInDate.Month)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new
                    {
                        Group = g.Key,
                        Total = g.Sum(b => b.Cost),
                        Average = g.Average(b => b.Cost),
                        Lowest = g.Min(b => b.Cost),
                        Highest = g.Max(b => b.Cost)
                    }).ToExpandoList();

                return View(model);
            }
            else
            {
                data = DBUtl.GetList<PHBooking>(@"SELECT * FROM PHBooking
                                                            WHERE YEAR(CheckInDate) = {0}
                                                                    AND MONTH(CheckInDate) = {1}",
                                                                    ryear, rmonth);
                ViewData["reportheader"] = $"Monthly Sales for {ryear} Month {rmonth} by Day";
                var model = data.GroupBy(b => b.CheckInDate.Day)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new
                    {
                        Group = g.Key,
                        Total = g.Sum(b => b.Cost),
                        Average = g.Average(b => b.Cost),
                        Lowest = g.Min(b => b.Cost),
                        Highest = g.Max(b => b.Cost)
                    }).ToExpandoList();

                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Reports()
        {
            var data = DBUtl.GetList<PHBooking>
            (@"SELECT bpt.*, u.Name as BookedByName
                    FROM
                    (SELECT b.*, pt.Description as PetType
                    FROM PHBooking b, PHPetType pt
                    WHERE b.PetTypeId = pt.Id) bpt, PHUser u
                    WHERE bpt.BookedBy = u.Id");

            // Report 1 - Number of Bookings by Customer
            ViewData["report1"] = data.OrderBy(b => b.BookedByName).GroupBy(b => b.BookedByName).Select(g => new
            {
                Customer = g.Key,
                NumberOfBookings = g.Count()
            }).ToExpandoList();

            // Report 2 - Number of Bookings by Pet Type
            ViewData["report2"] = data.OrderBy(b => b.PetType).GroupBy(b => b.PetType).Select(g => new
            {
                PetType = g.Key,
                NumberOfBookings = g.Count()
            }).ToExpandoList();

            // Report 3 - Sales by Customer
            ViewData["report3"] = data.OrderBy(b => b.BookedByName).GroupBy(b => b.BookedByName).Select(g => new
            {
                Customer = g.Key,
                Sales = g.Sum(b => b.Cost),
                AvgSales = g.Average(b => b.Cost)
            }).ToExpandoList();

            // Report 4 - Sales by Pet Type
            ViewData["report4"] = data.OrderBy(b => b.PetType).GroupBy(b => b.PetType).Select(g => new
            {
                PetType = g.Key,
                Sales = g.Sum(b => b.Cost),
                AvgSales = g.Average(b => b.Cost)
            }).ToExpandoList();

            return View();
        }

        [AllowAnonymous]
        public IActionResult AboutUs()
        {
            return View();
        }

    }
    

}

//19041130 NG BRANDON MARK
