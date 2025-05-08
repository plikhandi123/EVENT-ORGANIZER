using EVENT_ORGANIZER.Models;
using Microsoft.Data.SqlClient;

namespace EVENT_ORGANIZER.Repository.Interface
{
    public interface ILogin
    {
       
        LoginResponse GetLogin(LoginRequest obj);

        SqlConnection GetConnection();
        string CreateOrUpdate(Register objs);
    }
}
