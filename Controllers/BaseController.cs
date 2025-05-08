using EVENT_ORGANIZER.Models;
using Microsoft.AspNetCore.Mvc;

namespace EVENT_ORGANIZER.Controllers
{
    public class BaseController : Controller
    {
        protected LoginResponse GetUserSession()
        {
            LoginResponse? Sessionobjs = new LoginResponse();
            // Retrieve JSON string from session
            string? jsonString = HttpContext.Session.GetString("LoginSession");

            if (!string.IsNullOrEmpty(jsonString))
            {
                // Deserialize to object
                Sessionobjs = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(jsonString);
            }
            return Sessionobjs;
        }
    }
}
