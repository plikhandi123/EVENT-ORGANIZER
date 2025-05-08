using EVENT_ORGANIZER.Repository.Interface;
using EVENT_ORGANIZER.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;

namespace EVENT_ORGANIZER.Controllers
{
    public class LoginController : Controller
    {
        public readonly ILogger<LoginController> _logger;
        public readonly IErrorLogs _errorLogs;
        public readonly ILogin _login;
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration,IErrorLogs errorLogs, ILogger<LoginController> logger, ILogin login)
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

        public IActionResult Index()
        {
            LoginRequest obj = new LoginRequest();
            return View(obj);
        }
        [HttpPost]
        public IActionResult Index(LoginRequest request)
        {
            LoginRequest obj = new LoginRequest();
            obj = request;
            try
            {
                if (ModelState.IsValid)
                {
                    ModelState.Clear();
                    var Result = _login.GetLogin(obj);

                    if (Result.UserId > 0)
                    {
                        if (Result.Password == obj.Password)
                        {
                            //string stringSession = System.Text.Json.JsonSerializer.Serialize(Result);
                            // Store in session
                            //HttpContext.Session.SetString("LoginSession", stringSession);
                            HttpContext.Session.SetInt32("UserId", Result.UserId);
                            HttpContext.Session.SetString("UserType", Result.UserType);
                            HttpContext.Session.SetString("Email", Result.Email);
                            HttpContext.Session.SetString("UName", Result.FirstName);
                            HttpContext.Session.SetString("ULastName", Result.LastName);


                            if (Result.UserType == "Admin")
                            {
                                return RedirectToAction("AdminDashboard", "Admin");
                            }
                            else if (Result.UserType == "User")
                            {
                                return RedirectToAction("UserDashboard", "User");
                            }
                            else
                            {
                                ViewBag.Error = $"Invalid username & password";
                            }

                        }
                        else
                        {
                            ViewBag.Error = $"Invalid username & password";
                        }

                    }
                    else
                    {
                        ViewBag.Error = $"Invalid username & password";

                    }


                }


                return View(obj);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred");
                _errorLogs.ErrorLog(LogLevel.Error.ToString(), ex.Message, ex.StackTrace);
                return View(obj);
            }



        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LoginSession");

            return RedirectToAction("Index", "Login");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Register register)
        {
            Register objs = new Register();
            objs = register;

            if (string.IsNullOrEmpty(objs.UserType))
            {
                objs.UserType = "User";
            }

            if (ModelState.IsValid)
            {
                ModelState.Clear();
                try
                {
                    using (var con = _login.GetConnection())
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("SP_Register", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                       
                        cmd.Parameters.AddWithValue("@FirstName", objs.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", objs.LastName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", objs.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Address", objs.Address);
                        cmd.Parameters.AddWithValue("@Password", objs.Password);
                        cmd.Parameters.AddWithValue("@Email", objs.Email);

                        if (objs.UserType != null)
                        {
                            cmd.Parameters.AddWithValue("@UserType", objs.UserType);
                        }

                        // Execute SP
                        cmd.ExecuteNonQuery();
                        int i = 0;
                        if (i > 0)
                        {
                            ViewBag.SuccessMsg = $"Register Successfull. Please Login.";
                            ModelState.Clear();
                            return View();
                        }
                        else
                        {
                            ViewBag.ErrorMsg = "Data not inserted";
                            return View(register);
                        }

                        // ✅ Set session values after successful registration
                        HttpContext.Session.SetInt32("UserId", (int)objs.UserId);
                        HttpContext.Session.SetString("UName", objs.FirstName);
                        HttpContext.Session.SetString("ULastName", objs.LastName);
                        HttpContext.Session.SetString("IsRegistered", "true");

                        
                        return RedirectToAction("BookNow", "User");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMsg = ex.Message;
                }
            }

            return View(objs);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}