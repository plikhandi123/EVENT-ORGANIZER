using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EVENT_ORGANIZER.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace EVENT_ORGANIZER.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    [Obsolete]
    private SqlConnection GetConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("IEOConnection"));
    }
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_Login_CURD", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransType", "SELECT-ONE");
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Password", model.Password);

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read()) // If user found
                {
                    string email = rdr["Email"].ToString();
                    string userType = rdr["UserType"].ToString(); // Fetching UserType

                    // Store user info in Session (Optional)
                    HttpContext.Session.SetString("UserEmail", email);
                    HttpContext.Session.SetString("UserType", userType);

                    // Redirect based on UserType
                    if (userType.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("UserDashboard", "User");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid Email or Password!";
                    return View(model);
                }
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View(model);
        }
    }

    //public IActionResult Register()
    //{
    //    return View(new Register()); // Ensure it passes a model instance
    //}
    [HttpGet]
    public IActionResult Register(int id)
    {
        Register objs = new Register();
        if (id > 0)
        {
            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_Register_CURD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransType", "SELECT-ONE");
                    cmd.Parameters.AddWithValue("@UserId", id);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {

                        objs.UserId = (int)rdr["UserId"];
                        objs.FirstName = rdr["Firstname"].ToString();
                        objs.LastName = rdr["Lastname"].ToString();
                        objs.PhoneNumber = rdr["PhoneNumber"].ToString();
                        objs.Address = rdr["Address"].ToString();
                        objs.Password = rdr["Password"].ToString();
                        objs.UserType = rdr["UserType"].ToString();
                        objs.Email = rdr["Email"].ToString();

                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        return View(objs);
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public IActionResult Register(Register register)
    {
        Register objs = new Register();
        objs = register;

        if (ModelState.IsValid)
        {
            ModelState.Clear();
            try
            {
                string TransType = string.Empty;
                using (var con = GetConnection())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_Register_CURD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objs.UserId > 0)
                    {
                        TransType = "UPDATE";
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@UserId", objs.UserId);
                    }
                    else
                    {
                        TransType = "INSERT";
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@UserId", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@FirstName", objs.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", objs.LastName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", objs.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Address", objs.Address);
                    cmd.Parameters.AddWithValue("@Password", objs.Password);
                    cmd.Parameters.AddWithValue("@UserType", objs.UserType);
                    cmd.Parameters.AddWithValue("@Email", objs.Email);
                    cmd.ExecuteNonQuery();


                    ViewBag.SuccessMsg = $"Data {TransType} Done";
                    objs = new Register();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = ex.Message;
            }
        }

        return View(objs);
    }

    public IActionResult RegisterList()
    {
        Register objs = new Register();
        try
        {
            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_Register_CURD", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransType", "SELECT");
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    objs.RegisterList.Add(new Register
                    {
                        UserId = (int)rdr["UserId"],
                        FirstName = rdr["FirstName"].ToString(),
                        LastName = rdr["LastName"].ToString(),
                        PhoneNumber = rdr["PhoneNumber"].ToString(),
                        Address = rdr["Address"].ToString(),
                        Password = rdr["Password"].ToString(),
                        UserType = rdr["UserType"].ToString(),
                        Email = rdr["Email"].ToString()
                    });
                }
            }
        }
        catch (Exception ex)
        {

            throw;
        }

        return View(objs);
    }
    public IActionResult RegisterDelete(int id)
    {

        if (id > 0)
        {
            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_Register_CURD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransType", "DELETE");
                    cmd.Parameters.AddWithValue("@UserId", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                return RedirectToAction("RegisterList", "Login");
            }
        }

        return RedirectToAction("RegisterList", "Login");
    }
    //[HttpPost]
    //public IActionResult Register(Register model)
    //{

    //    Register objs = new Register();
    //    try
    //    {
    //        using (var con = GetConnection())
    //        {
    //            con.Open();
    //            SqlCommand cmd = new SqlCommand("SP_Registrations_CURD", con);
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.Parameters.AddWithValue("@TransType", "SELECT");
    //            SqlDataReader rdr = cmd.ExecuteReader();
    //            while (rdr.Read())
    //            {
    //                objs.RegistrationList.Add(new Register
    //                {
    //                    Id = (int)rdr["Id"],
    //                    Firstname = rdr["Firstname"].ToString(),
    //                    Lastname = rdr["Lastname"].ToString(),
    //                    Mobile = rdr["Mobile"].ToString(),
    //                    Email = rdr["Email"].ToString(),
    //                    Address = rdr["Address"].ToString(),
    //                    Gender = rdr["Gender"].ToString(),
    //                    Age = Convert.ToInt32(rdr["Age"]),
    //                    Password = rdr["Password"].ToString()
    //                });
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {

    //        throw;
    //    }
    //    if (ModelState.IsValid)
    //    {
    //        // Process registration (Save to DB, etc.)
    //        ViewBag.SuccessMsg = "Registration Successful!";
    //        return View(new Register()); // Clear the form
    //    }

    //    ViewBag.ErrorMsg = "Please fix the errors and try again.";
    //    return View(model);
    //}

    public IActionResult Forgotpassward()
    {
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Services()
    {
        return View();
    }
    public IActionResult Decoration()
    {
        return View();
    }
    public IActionResult Photography()
    {
        return View();
    }
    public IActionResult Videography()
    {
        return View();
    }
    public IActionResult Entertainment()
    {
        return View();
    }
    public IActionResult Security()
    {
        return View();
    }
    public IActionResult Catering()
    {
        return View();
    }
    public IActionResult About()
    {
        return View();
    }

    public IActionResult Venues()
    {
        return View();
    }
    public IActionResult Gallery()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult BookNow()
    {
        return View();
    }
    public IActionResult KnowMore()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
