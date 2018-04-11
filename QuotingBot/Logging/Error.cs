using Dapper;
using QuotingBot.DAL.Repository.Errors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QuotingBot.Logging
{
    public class Error
    {
        public Error() { }

        public void Log(string conversationId, string userId, string date, string conversationLog, string errorMessage)
        {
            using (var connection = new SqlConnection("Server=DESKTOP-HL69CK9\\PCONNOLLY; Database=QuotingBot; Trusted_connection=true"))
            {
                connection.OpenAsync();
                var quote = new DAL.Models.Error
                (
                    conversationId,
                    userId,
                    date,
                    conversationLog,
                    errorMessage
                );

                connection.ExecuteAsync("usp_Add_Error", quote, null, null, CommandType.StoredProcedure);
            }
        }

        public void Log(string date, string errorMessage)
        {
            var errorRepository = new ErrorRepository();
            errorRepository.LogError(DateTime.Now.ToString(), errorMessage);
        }
    }
}