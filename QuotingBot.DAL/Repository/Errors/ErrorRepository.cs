using Dapper;
using QuotingBot.DAL.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QuotingBot.DAL.Repository.Errors
{
    public class ErrorRepository
    {
        public ErrorRepository() { }

        public async void LogError(string conversationId, string userId, string conversationDate, string conversationLog, string errorMessage)
        {
            using (var connection = new SqlConnection("Server=DESKTOP-HL69CK9\\PCONNOLLY; Database=QuotingBot; Trusted_connection=true"))
            {
                await connection.OpenAsync();
                var error = new Error
                (
                    conversationId,
                    userId,
                    conversationDate,
                    conversationLog,
                    errorMessage
                );

                await connection.ExecuteAsync("usp_Add_Error", error, null, null, CommandType.StoredProcedure);
            }
        }

        public async void LogError(string conversationDate, string errorMessage)
        {
            using (var connection = new SqlConnection("Server=DESKTOP-HL69CK9\\PCONNOLLY; Database=QuotingBot; Trusted_connection=true"))
            {
                await connection.OpenAsync();
                var quote = new Error
                (
                    Guid.Empty.ToString(),
                    Guid.Empty.ToString(),
                    conversationDate,
                    string.Empty,
                    errorMessage
                );

                await connection.ExecuteAsync("usp_Add_Error", quote, null, null, CommandType.StoredProcedure);
            }
        }
    }
}
