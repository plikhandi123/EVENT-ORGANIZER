using EVENT_ORGANIZER.Repository.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EVENT_ORGANIZER.Repository.Services
{
    
    
        public class ErrorLogsRepository : IErrorLogs
        {
            private readonly IConfiguration _configuration;
            public ErrorLogsRepository(IConfiguration configuration)
            {
                _configuration = configuration;

            }

            private SqlConnection GetConnection()
            {
                return new SqlConnection(_configuration.GetConnectionString("IEOConnection"));
            }
            public void ErrorLog(string LogLevel, string Message, string StackTrace)
            {

                using (var con = GetConnection())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_ErrorLogs", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@LogLevel", LogLevel);
                    cmd.Parameters.AddWithValue("@Message", Message);
                    cmd.Parameters.AddWithValue("@StackTrace", StackTrace);
                    cmd.ExecuteNonQuery();


                }

            }
        }
    
}
