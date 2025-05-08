using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using EVENT_ORGANIZER.Models;
using EVENT_ORGANIZER.Repository.Interface;
using Microsoft.Data.SqlClient;
using System.Data;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EVENT_ORGANIZER.Controllers
{
    public class UserController : BaseController
    {
        public readonly ILogger<UserController> _logger;
        public readonly IErrorLogs _errorLogs;
        public readonly ILogin _login;
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration, IErrorLogs errorLogs, ILogger<UserController> logger, ILogin login)
        {
            _login = login;
            _errorLogs = errorLogs;
            _logger = logger;
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("IEOConnection"));
        }


        public IActionResult UserDashboard()
        {

            //var userSession = GetUserSession();
            int? uid = HttpContext.Session.GetInt32("UserId");
            if (uid == null)
            {
                return RedirectToAction("Index", "Login");
            }
            string? firstName = HttpContext.Session.GetString("UName");
            string? lastName = HttpContext.Session.GetString("ULastName");

            ViewBag.FullName = $"{firstName} {lastName}";

            List<EventBookingModel> bookings = new List<EventBookingModel>();

            // Example: fetch data from database (replace with your own logic)
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_GetUserBookings", conn); 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", uid);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookings.Add(new EventBookingModel
                    {
                        EventName = reader["EventName"].ToString(),
                        Location = reader["Location"].ToString(),
                        Venues = reader["Venues"].ToString(),
                        Services = reader["Services"].ToString(),
                        Capacity = Convert.ToInt32(reader["Capacity"]),
                        EventDate = Convert.ToDateTime(reader["EventDate"]),
                        BookingStatus = reader["BookingStatus"].ToString()
                    });
                }
            }

            return View(bookings); // ✅ This is the part you’re missing!
        }
        public IActionResult Services()
        {
            List<Services> services = new List<Services>();

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllServices", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            services.Add(new Services
                            {
                                ServiceId = Convert.ToInt32(reader["ServiceId"]),
                                ServiceName = reader["ServiceName"].ToString(),
                                Description = reader["Description"].ToString()
                            });
                        }
                    }
                }
            }

            return View(services); 
        }

        // GET: BookNow
        public IActionResult BookNow()
        {
            // Get user name for display
            string? firstName = HttpContext.Session.GetString("UName");
            string? lastName = HttpContext.Session.GetString("ULastName");
            ViewBag.FullName = $"{firstName} {lastName}";

            // Load dropdown data
            ViewBag.EventList = GetEventList();
            ViewBag.LocationList = GetLocationList();
            ViewBag.VenueList = GetVenueList();
            ViewBag.ServiceList = GetServiceList();
            ViewBag.CapacityList = GetCapacityList();

            //        // Load venue data
            //        List<VenueModel> availableVenues = new();
            //        List<BookedVenueModel> bookedVenues = new();

            //        string connectionString = _configuration.GetConnectionString("IEOConnection");
            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    conn.Open();

            //            // Get Available Venues
            //            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Venue WHERE Id NOT IN (SELECT VenueId FROM Booking)", conn))
            //            using (SqlDataReader reader = cmd.ExecuteReader())
            //            {
            //                while (reader.Read())
            //                {
            //                    availableVenues.Add(new VenueModel
            //                    {
            //                        VenueId = (int)reader["VenueId"],
            //                        VenueName = reader["VenueName"].ToString(),
            //                        Location = reader["Location"].ToString(),
            //                        Capacity = Convert.ToInt32(reader["Capacity"])
            //                    });
            //                }
            //            }


            //            // Get Booked Venues
            //            using (SqlCommand cmd = new SqlCommand(@"SELECT B.EventDate, V.Name AS VenueName, U.Username AS BookedBy 
            //                                             FROM Booking B 
            //                                             JOIN Venue V ON B.VenueId = V.Id 
            //                                             JOIN Users U ON B.UserId = U.Id", conn))
            //    using (SqlDataReader reader = cmd.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            bookedVenues.Add(new BookedVenueModel
            //            {
            //                VenueName = reader["VenueName"].ToString(),
            //                BookedDate = Convert.ToDateTime(reader["EventDate"]),
            //                BookedBy = reader["BookedBy"].ToString()
            //            });
            //        }
            //    }
            //}

            //var viewModel = new BookNowViewModel
            //{
            //    AvailableVenues = availableVenues,
            //    BookedVenues = bookedVenues,
            //    Booking = new EventBookingModel() 
            //};

            return View();
        }




        ////  Fetch Events List
        private List<SelectListItem> GetEventList()
        {
            List<SelectListItem> eventList = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT  EventName FROM Events", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            eventList.Add(new SelectListItem
                            {
                                //Value = reader["EventId"].ToString(),
                                Text = reader["EventName"].ToString()
                            });
                        }
                    }
                }
            }
            return eventList;
        }

        //  Fetch Locations List
        private List<SelectListItem> GetLocationList()
        {
            List<SelectListItem> locationList = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT  Location FROM Venues", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            locationList.Add(new SelectListItem
                            {
                                //Value = reader["LocationId"].ToString(),
                                Text = reader["Location"].ToString()
                            });
                        }
                    }
                }
            }
            return locationList;
        }

        //  Fetch Venues List
        private List<SelectListItem> GetVenueList()
        {
            List<SelectListItem> venueList = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT  VenueName FROM Venues", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            venueList.Add(new SelectListItem
                            {
                                //Value = reader["VenueId"].ToString(),
                                Text = reader["VenueName"].ToString()
                            });
                        }
                    }
                }
            }
            return venueList;
        }

        //  Fetch Services List
        private List<SelectListItem> GetServiceList()
        {
            List<SelectListItem> serviceList = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT  ServiceName FROM Services", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            serviceList.Add(new SelectListItem
                            {
                                //Value = reader["ServiceId"].ToString(),
                                Text = reader["ServiceName"].ToString()
                            });
                        }
                    }
                }
            }
            return serviceList;
        }

        //  Fetch Capacity List
        private List<SelectListItem> GetCapacityList()
        {
            return new List<SelectListItem>
        {
            new SelectListItem { Value = "50", Text = "50 People" },
            new SelectListItem { Value = "100", Text = "100 People" },
            new SelectListItem { Value = "200", Text = "200 People" },
            new SelectListItem { Value = "500", Text = "500 People" },
            new SelectListItem { Value = "1000", Text = "1000 People" }

        };
        }

        [HttpPost]
        public IActionResult BookNow(EventBookingModel model)
        {
            // Check if user is logged in
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("BookNow", "User");
            }

            
            model.UserId = userId;
            ModelState.Remove("AvailableVenues");
            ModelState.Remove("BookedVenues");
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
                    {

                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SP_BookEvent", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@EventName", model.EventName);
                            cmd.Parameters.AddWithValue("@Location", model.Location);
                            cmd.Parameters.AddWithValue("@Venues", model.Venues);
                            cmd.Parameters.AddWithValue("@Services", model.Services);
                            cmd.Parameters.AddWithValue("@Capacity", model.Capacity);
                            cmd.Parameters.AddWithValue("@EventDate", model.EventDate);
                            cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@UserId", model.UserId);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    TempData["SuccessMessage"] = "Event booked successfully!";
                    return RedirectToAction("BookingList");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error while booking event: " + ex.Message);
                }
            }

            // If validation fails, reload dropdowns
            ViewBag.EventList = GetEventList();
            ViewBag.LocationList = GetLocationList();
            ViewBag.VenueList = GetVenueList();
            ViewBag.ServiceList = GetServiceList();
            ViewBag.CapacityList = GetCapacityList();

            
            ViewBag.FullName = $"{HttpContext.Session.GetString("UName")} {HttpContext.Session.GetString("ULastName")}";

            return View(model);
        }



        //[HttpPost]
        //public IActionResult Booking(Booking model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (SqlConnection conn = new SqlConnection("IEOConnection"))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("SP_InsertBooking", conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@EventName", model.EventName);
        //                cmd.Parameters.AddWithValue("@Location", model.Location);
        //                cmd.Parameters.AddWithValue("@Venues", model.Venues);
        //                cmd.Parameters.AddWithValue("@Services", model.Services);
        //                cmd.Parameters.AddWithValue("@Capacity", model.Capacity);
        //                cmd.Parameters.AddWithValue("@Description", model.Description);
        //                cmd.Parameters.AddWithValue("@EventDate", model.EventDate);
        //                cmd.Parameters.AddWithValue("@BookingStatus", model.BookingStatus);

        //                conn.Open();
        //                cmd.ExecuteNonQuery();
        //            }
        //        }

        //        return RedirectToAction("BookingList");
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //public IActionResult BookEvent(EventBookingModel model)
        //{
        //    if (HttpContext.Session.GetInt32("UserId") != null)
        //    {
        //        model.UserId = HttpContext.Session.GetInt32("UserId");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
        //            {
        //                conn.Open();
        //                using (SqlCommand cmd = new SqlCommand("SP_BookEvent", conn))
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.Parameters.AddWithValue("@EventName", model.EventName);  
        //                    cmd.Parameters.AddWithValue("@Location", model.Location);
        //                    cmd.Parameters.AddWithValue("@Venues", model.Venues);
        //                    cmd.Parameters.AddWithValue("@Services", model.Services);
        //                    cmd.Parameters.AddWithValue("@Capacity", model.Capacity);
        //                    cmd.Parameters.AddWithValue("@EventDate", model.EventDate);
        //                    cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
        //                    cmd.Parameters.AddWithValue("@UserId", model.UserId); // Get logged-in user ID

        //                    cmd.ExecuteNonQuery();
        //                }
        //            }

        //            TempData["SuccessMessage"] = "Event booked successfully!";
        //            return RedirectToAction("BookingList"); // Redirect to booking list page
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", "Error while booking event: " + ex.Message);
        //        }
        //    }

        //    // If validation fails, reload the event form
        //    ViewBag.EventList = GetEventList();
        //    ViewBag.LocationList = GetLocationList();
        //    ViewBag.VenueList = GetVenueList();
        //    ViewBag.ServiceList = GetServiceList();
        //    ViewBag.CapacityList = GetCapacityList();
        //    return View(model);
        //}
        [HttpGet]
        public JsonResult GetVenuesByLocation(string location)
        {
            var allVenues = new Dictionary<string, List<string>>()
    {
        { "Bhubneshwar", new List<string> {
            "Ekamara Greens , Chanrashekarpur",
            "Blue orchid, KIIT Rd, Patia",
            "The Urmila Resort., Mancheshwar"
        }},
        { "Cuttuck", new List<string> {
            "Barabati Palace, Bijupattnaik Colony",
            "Cuttuck Garden, Balisahi",
            "Royal Garden, Mahammadia Bazar"
        }}
    };

            var venues = allVenues.ContainsKey(location) ? allVenues[location] : new List<string>();
            return Json(venues);
        }


        [HttpGet]
        public JsonResult GetEventDetails(int eventId)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
        SELECT EventName, 
               Location, 
               Venues AS Venue, 
               Services AS Service
        FROM Bookings
        WHERE BookingId = @BookingId", conn))  // ✅ Updated column name
                {
                    cmd.Parameters.AddWithValue("@BookingId", eventId);  // ✅ Updated parameter

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var eventDetails = new
                            {
                                eventName = reader["EventName"].ToString(),
                                location = reader["Location"].ToString(),
                                venue = reader["Venues"].ToString(),
                                service = reader["Services "].ToString()
                            };
                            return Json(eventDetails);
                        }
                    }
                }
            }
            return Json(null);


        }

        public IActionResult BookingList()
        {
            List<EventBookingModel> bookings = new List<EventBookingModel>();

            // Ensure the user is logged in
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            int userId = (int)HttpContext.Session.GetInt32("UserId");

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Bookings WHERE UserId = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bookings.Add(new EventBookingModel
                                {
                                    BookingId = Convert.ToInt32(reader["BookingId"]),
                                    EventName = reader["EventName"].ToString(),
                                    Location = reader["Location"].ToString(),
                                    Venues = reader["Venues"].ToString(),
                                    Services = reader["Services"].ToString(),
                                    Capacity = Convert.ToInt32(reader["Capacity"]),
                                    EventDate = Convert.ToDateTime(reader["EventDate"]),
                                    Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null
                                });
                            }
                        }   
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error while fetching bookings: " + ex.Message);
            }

            return View(bookings);
        }

        //public IActionResult BookingList()
        //{
        //    List<EventBookingModel> bookings = new List<EventBookingModel>();

        //    using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand("SELECT * FROM Bookings WHERE UserId = @UserId", conn))
        //        {
        //            cmd.Parameters.AddWithValue("@UserId", User.FindFirstValue(ClaimTypes.NameIdentifier));
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    bookings.Add(new EventBookingModel
        //                    {
        //                        BookingId = Convert.ToInt32(reader["BookingId"]),
        //                        EventName = reader["EventName"].ToString(),
        //                        Location = reader["Location"].ToString(),
        //                        Venues = reader["Venues"].ToString(),
        //                        Services = reader["Services"].ToString(),
        //                        Capacity = Convert.ToInt32(reader["Capacity"]),
        //                        EventDate = Convert.ToDateTime(reader["EventDate"]),
        //                        Description = reader["Description"].ToString()
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    return View(bookings);
        //}

        //[HttpGet]
        //public IActionResult EditBooking(int id)
        //{
        //    Booking booking = null;

        //    using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("SELECT * FROM Bookings WHERE BookingId = @BookingId", conn))
        //        {
        //            cmd.Parameters.AddWithValue("@BookingId", id);
        //            conn.Open();

        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    booking = new Booking
        //                    {
        //                        BookingId = Convert.ToInt32(reader["BookingId"]),
        //                        EventName = reader["EventName"].ToString(),
        //                        Location = reader["Location"].ToString(),
        //                        Venues = reader["Venues"].ToString(),
        //                        Services = reader["Services"].ToString(),
        //                        Capacity = Convert.ToInt32(reader["Capacity"]),
        //                        Description = reader["Description"].ToString(),
        //                        EventDate = Convert.ToDateTime(reader["EventDate"])
        //                    };
        //                }
        //            }
        //        }
        //    }

        //    return View(booking);
        //}





        public IActionResult EditBooking(int id)
        {
            Booking booking = null;

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SP_GetBookingById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BookingId", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            booking = new Booking
                            {
                                BookingId = Convert.ToInt32(reader["BookingId"]),
                                EventName = reader["EventName"].ToString(),
                                Location = reader["Location"].ToString(),
                                Venues = reader["Venues"].ToString(),
                                Services = reader["Services"].ToString(),
                                Capacity = Convert.ToInt32(reader["Capacity"]),
                                EventDate = Convert.ToDateTime(reader["EventDate"]),
                                Description = reader["Description"].ToString()
                            };
                        }
                    }
                }
            }

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [HttpPost]
        public IActionResult EditBooking(Booking model)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("SP_UpdateBooking", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BookingId", model.BookingId);
                    cmd.Parameters.AddWithValue("@EventName", model.EventName);
                    cmd.Parameters.AddWithValue("@Location", model.Location);
                    cmd.Parameters.AddWithValue("@Venues", model.Venues);
                    cmd.Parameters.AddWithValue("@Services", model.Services);
                    cmd.Parameters.AddWithValue("@Capacity", model.Capacity);
                    cmd.Parameters.AddWithValue("@Description", model.Description);
                    cmd.Parameters.AddWithValue("@EventDate", model.EventDate);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("BookingList");
        }
        public IActionResult CancelBooking(int id)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CancelBooking", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BookingId", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("BookingList");
        }




    }
}
