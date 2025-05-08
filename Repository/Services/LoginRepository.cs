using EVENT_ORGANIZER.Models;
using EVENT_ORGANIZER.Repository.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EVENT_ORGANIZER.Repository.Services
{
    public class LoginRepository : ILogin
    {
        private readonly IConfiguration _configuration;
        public LoginRepository(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("IEOConnection"));
        }


        public LoginResponse GetLogin(LoginRequest obj)
        {
            LoginResponse objs = new LoginResponse();
            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_Login", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", obj.Email);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {

                    objs.UserId = (int)rdr["UserId"];
                    objs.FirstName = rdr["FirstName"].ToString();
                    objs.LastName = rdr["LastName"].ToString();
                    objs.PhoneNumber = rdr["PhoneNumber"].ToString();
                    objs.Address = rdr["Address"].ToString();
                    objs.Password = rdr["Password"].ToString();
                    objs.UserType = rdr["UserType"].ToString();
                    objs.Email = rdr["Email"].ToString();

                }
            }
            return objs;
        }

        public string CreateOrUpdate(Register objs)
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
                cmd.Parameters.AddWithValue("@Email", objs.Email);
                cmd.Parameters.AddWithValue("@Address", objs.Address);
                cmd.Parameters.AddWithValue("@Password", objs.Password);
                cmd.ExecuteNonQuery();


            }
            return TransType;
        }
    }
}
    

