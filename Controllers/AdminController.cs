using System.Data;
using EVENT_ORGANIZER.Models;
using EVENT_ORGANIZER.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace EVENT_ORGANIZER.Controllers
{
    public class AdminController : BaseController
    {
        public readonly ILogger<AdminController> _logger;
        public readonly IErrorLogs _errorLogs;
        public readonly ILogin _login;
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration, IErrorLogs errorLogs, ILogger<AdminController> logger, ILogin login)
        {
            _login = login;
            _errorLogs = errorLogs;
            _logger = logger;
            _configuration = configuration;
        }
        //Admin Dashboard Connection
        public IActionResult AdminDashboard()
        {
            //var userSession = GetUserSession();
            string? uid = HttpContext.Session.GetString("UserId");
            if (uid == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("IEOConnection"));
        }
        
        //Admin venue
        [HttpGet]
        public IActionResult Venues()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Venues(Venues venue)
        {
            if (ModelState.IsValid)
            {
                string connstr = _configuration.GetConnectionString("IEOConnection"); // Replace with your actual connection string

                using (SqlConnection conn = new SqlConnection(connstr))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_InsertVenues", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Adding parameters to match the stored procedure
                        
                        cmd.Parameters.AddWithValue("@VenueName", venue.VenueName);
                        cmd.Parameters.AddWithValue("@Location", venue.Location);
                        cmd.Parameters.AddWithValue("@Capacity", venue.Capacity);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

            }

            return View(venue);
        }

        public IActionResult VenueList()
        {
            List<Venues> venues = new List<Venues>(); // List to store all venues

            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_GetVenues", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            venues.Add(new Venues
                            {
                                VenueId =(int) rdr["VenueId"],
                                VenueName = rdr["VenueName"].ToString(),
                                Location = rdr["Location"].ToString(),
                                Capacity = Convert.ToInt32(rdr["Capacity"]),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return View(venues); // Return the list of venues
        }
        
        public IActionResult DeleteVenue(int id)
        {
            if (id > 0)
            {
                try
                {
                    using (var con = GetConnection())
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("SP_DeleteVenue", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmd.Parameters.AddWithValue("@VenueId", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("VenueList", "Admin");
                }
            }

            return RedirectToAction("VenueList", "Admin");
        }


        [HttpGet]
        public IActionResult EditVenue(int id)
        {
            Venues venue = null;
            using (var con = GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SP_GetVenueById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VenueId", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())  // Fetch the venue details
                        {
                            venue = new Venues
                            {
                                VenueId = Convert.ToInt32(reader["VenueId"]),
                                VenueName = reader["VenueName"].ToString(),
                                Location = reader["Location"].ToString(),
                                Capacity = Convert.ToInt32(reader["Capacity"]),
                            };
                        }
                    }
                }
            }

            if (venue == null)
            {
                return NotFound();  // Return 404 if venue not found
            }

            return View(venue);
        }

        [HttpPost]
        public IActionResult EditVenue(Venues venue)
        {
            if (!ModelState.IsValid)
            {
                return View(venue); // If model is invalid, return to form
            }

            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_UpdateVenue", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VenueId", venue.VenueId);
                        cmd.Parameters.AddWithValue("@VenueName", venue.VenueName);
                        cmd.Parameters.AddWithValue("@Location", venue.Location);
                        cmd.Parameters.AddWithValue("@Capacity", venue.Capacity);

                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Venue updated successfully!";
                return RedirectToAction("VenueList");  // Redirect to venue list after update
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the venue.");
                // Log the error if necessary: _logger.LogError(ex, "Error updating venue");
            }

            return View(venue); // Stay on the form if an error occurs
        }

        
        //Userlist
        public IActionResult UserList()
        {
            List<Register> users = new List<Register>(); // List to store all users

            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_GetAllUsers", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            users.Add(new Register
                            {
                                UserId = (int)rdr["UserId"],
                                FirstName = rdr["FirstName"].ToString(),
                                LastName = rdr["LastName"].ToString(),
                                PhoneNumber = rdr["PhoneNumber"].ToString(),
                                Address = rdr["Address"].ToString(),
                                UserType = rdr["UserType"].ToString(),
                                Email = rdr["Email"].ToString(),
                                Status = rdr["Status"].ToString()

                            });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return View("Error"); // Return an error view
            }

            return View(users); // Return the list of users
        }
        [HttpGet]
        public IActionResult ChangeStatus(int id, string status)
        {
            if (id > 0 && !string.IsNullOrEmpty(status))
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE [Users] SET Status = @Status WHERE UserId = @UserId", con);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@UserId", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("UserList");
        }


        //Admin Event
        public IActionResult Events()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Events(Events eventModel)
        {
            if (ModelState.IsValid)
            {
                string connstr = _configuration.GetConnectionString("IEOConnection"); // Ensure this matches your actual connection string

                using (SqlConnection conn = new SqlConnection(connstr))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_InsertEvent", conn)) // Stored procedure name
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Adding parameters to match the stored procedure
                        cmd.Parameters.AddWithValue("@EventName", eventModel.EventName);
                        cmd.Parameters.AddWithValue("@Description", eventModel.Description);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

            }

            return View(eventModel); // Return the view with model if validation fails
        }
        
        public IActionResult EventList()
        {
            List<Events> events = new List<Events>(); // List to store all events

            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_GetEvents", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            events.Add(new Events
                            {
                                EventId = Convert.ToInt32(rdr["EventId"]),
                                EventName = rdr["EventName"]?.ToString(),
                                Description = rdr["Description"]?.ToString(),
                                
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return View(events); // Return the list of events
        }

        [HttpGet]  
        public IActionResult DeleteEvent(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction("EventList", "Admin");
            }

            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_DeleteEvent", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EventId", id);
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("EventList", "Admin");
            }

            return RedirectToAction("EventList", "Admin");
        }

       
        [HttpGet]
        public IActionResult EditEvent(int id)
        {
            Events eventModel = new Events();
            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_GetEventById", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@EventId", id);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        eventModel.EventId = (int)rdr["EventId"];
                        eventModel.EventName = rdr["EventName"].ToString();
                        eventModel.Description = rdr["Description"].ToString();
                    }
                }
            }
            return View(eventModel);  // Pass data to the edit form
        }


        [HttpPost]
        public IActionResult EditEvent(Events eventModel)
        {
            if (ModelState.IsValid)
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_UpdateEvent", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@EventId", eventModel.EventId);
                    cmd.Parameters.AddWithValue("@EventName", eventModel.EventName);
                    cmd.Parameters.AddWithValue("@Description", eventModel.Description);

                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("EventList");  // Redirect to event list after update
            }
            return View(eventModel);  // If model is not valid, stay on the form
        }



        // Admin Services
        public IActionResult Services()
        {
            var model = new Services();
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Services(Services service)
        {
            if (ModelState.IsValid)
            {
                string connstr = _configuration.GetConnectionString("IEOConnection");

                    using (SqlConnection conn = new SqlConnection(connstr))
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_InsertService", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                            cmd.Parameters.AddWithValue("@Description", service.Description);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Redirect after successful insertion
                    return RedirectToAction("ServiceList");
                }
            return View(service);
        }

        public IActionResult ServiceList()
        {
            List<Services> services = new List<Services>(); // List to store all services

            try
            {
                using (var con = GetConnection()) // Ensure GetConnection() is implemented properly
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_GetAllServices", con) // Ensure stored procedure exists
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            services.Add(new Services
                            {
                                ServiceId = (int)rdr["ServiceId"],
                                ServiceName = rdr["ServiceName"].ToString(),
                                Description = rdr["Description"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               
                ViewBag.ErrorMessage = ex.Message;
            }

            return View(services); // Return the list of services
        }

        // GET: Edit Service
        public IActionResult EditService(int id)
        {
            Services service = new Services();
            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_GetServiceById", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@ServiceId", id);

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            service.ServiceId = (int)rdr["ServiceId"];
                            service.ServiceName = rdr["ServiceName"].ToString();
                            service.Description = rdr["Description"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(service);
        }

        // POST: Edit Service
        [HttpPost]
        public IActionResult EditService(Services service)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = GetConnection())
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("SP_UpdateService", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmd.Parameters.AddWithValue("@ServiceId", service.ServiceId);
                        cmd.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                        cmd.Parameters.AddWithValue("@Description", service.Description);
                        cmd.ExecuteNonQuery();
                    }
                    return RedirectToAction("ServiceList");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
            }
            return View(service);
        }

        [HttpGet]  
        public IActionResult DeleteService(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction("ServiceList", "Admin");
            }

            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_DeleteService", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ServiceId", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                
                return RedirectToAction("ServiceList", "Admin");
            }

            return RedirectToAction("ServiceList", "Admin");
        }

        // Customizations
        public IActionResult Customizations()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Customization(Customizations customizations)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_InsertCustomization", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EventId", customizations.EventId);
                cmd.Parameters.AddWithValue("@Theme", customizations.Theme);
                cmd.Parameters.AddWithValue("@SpecialRequests", customizations.SpecialRequests);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("CustomizationList");
        }
        public IActionResult GetCustomizations(int eventId)
        {
            List<Customization> customizations = new List<Customization>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_GetCustomizationByEvent", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EventId", eventId);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    customizations.Add(new Customization
                    {
                        CustomizationId = Convert.ToInt32(rdr["CustomizationId"]),
                        EventId = Convert.ToInt32(rdr["EventId"]),
                        Theme = rdr["Theme"].ToString(),
                        SpecialRequests = rdr["SpecialRequests"].ToString()
                    });
                }
            }
            return View(customizations);
        }
        public IActionResult DeleteCustomization(int customizationId)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("IEOConnection")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_DeleteCustomization", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomizationId", customizationId);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("CustomizationList");
        }



        public IActionResult Light()
        {
            return View();
        }
        public IActionResult Equipment()
        {
            return View();
        }
        public IActionResult Food()
        {
            return View();
        }
        public IActionResult Decoration()
        {
            return View();
        }

        // Bookings
        [HttpGet]
        public IActionResult Booking()
        {
            return View();
        }

        public IActionResult BookingList()
        {
            List<Booking> bookings = new List<Booking>();

            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_GetBookings", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        bookings.Add(new Booking   
                        {
                            BookingId = (int)rdr["BookingId"],
                            UserId = rdr["UserId"] != DBNull.Value ? (int)rdr["UserId"] : 0,
                            EventName = rdr["EventName"]?.ToString(),
                            Location = rdr["Location"]?.ToString(),
                            Venues = rdr["Venues"]?.ToString(),
                            Services = rdr["Services"]?.ToString(),
                            Capacity = rdr["Capacity"] != DBNull.Value ? Convert.ToInt32(rdr["Capacity"]) : 0,
                            Description = rdr["Description"]?.ToString(),
                            EventDate = (DateTime)(rdr["EventDate"] != DBNull.Value ? Convert.ToDateTime(rdr["EventDate"]) : (DateTime?)null),
                            BookingStatus = rdr["BookingStatus"]?.ToString()
                        });
                    }
                }
            }

            return View(bookings);
        }

        [HttpPost]
        public IActionResult PendingBookings()
        {
            List<Booking> bookings = new List<Booking>();

            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_GetPendingBookings", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        bookings.Add(new Booking
                        {
                            BookingId = (int)rdr["BookingId"],
                            UserId = rdr["UserId"] != DBNull.Value ? (int)rdr["UserId"] : 0,
                            EventName = rdr["EventName"]?.ToString(),
                            Location = rdr["Location"]?.ToString(),
                            Venues = rdr["Venues"]?.ToString(),
                            Services = rdr["Services"]?.ToString(),
                            Capacity = rdr["Capacity"] != DBNull.Value ? Convert.ToInt32(rdr["Capacity"]) : 0,
                            Description = rdr["Description"]?.ToString(),
                            EventDate = (DateTime)(rdr["EventDate"] != DBNull.Value ? Convert.ToDateTime(rdr["EventDate"]) : (DateTime?)null),
                            BookingStatus = rdr["BookingStatus"]?.ToString()
                        });
                    }
                }
            }

            return View(bookings);
        }

        public IActionResult CurrentBookings()
        {
            List<Booking> bookings = new List<Booking>();

            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_GetCurrentBookings", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        bookings.Add(new Booking
                        {
                            BookingId = (int)rdr["BookingId"],
                            UserId = rdr["UserId"] != DBNull.Value ? (int)rdr["UserId"] : 0,
                            EventName = rdr["EventName"]?.ToString(),
                            Location = rdr["Location"]?.ToString(),
                            Venues = rdr["Venues"]?.ToString(),
                            Services = rdr["Services"]?.ToString(),
                            Capacity = rdr["Capacity"] != DBNull.Value ? Convert.ToInt32(rdr["Capacity"]) : 0,
                            Description = rdr["Description"]?.ToString(),
                            EventDate = (DateTime)(rdr["EventDate"] != DBNull.Value ? Convert.ToDateTime(rdr["EventDate"]) : (DateTime?)null),
                            BookingStatus = rdr["BookingStatus"]?.ToString()
                        });
                    }
                }
            }

            return View(bookings);
        }

        public IActionResult BookingHistory()
        {
            List<Booking> bookings = new List<Booking>();

            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_GetBookingHistory", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        bookings.Add(new Booking
                        {
                            BookingId = (int)rdr["BookingId"],
                            UserId = rdr["UserId"] != DBNull.Value ? (int)rdr["UserId"] : 0,
                            EventName = rdr["EventName"]?.ToString(),
                            Location = rdr["Location"]?.ToString(),
                            Venues = rdr["Venues"]?.ToString(),
                            Services = rdr["Services"]?.ToString(),
                            Capacity = rdr["Capacity"] != DBNull.Value ? Convert.ToInt32(rdr["Capacity"]) : 0,
                            Description = rdr["Description"]?.ToString(),
                            EventDate = (DateTime)(rdr["EventDate"] != DBNull.Value ? Convert.ToDateTime(rdr["EventDate"]) : (DateTime?)null),
                            BookingStatus = rdr["BookingStatus"]?.ToString()
                        });
                    }
                }
            }

            return View(bookings);
        }






    }


}