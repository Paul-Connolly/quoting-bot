using Dapper;
using QuotingBot.Models;
using System.Data;
using System.Data.SqlClient;

namespace QuotingBot.DAL.Quotes
{
    public class QuoteRepository
    {
        private string ConnectionString { get; }

        public QuoteRepository(string connectionString) {
            ConnectionString = connectionString;
        }

        public async void StoreQuote(string conversationId, string quoteId, string quoteInfo)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                var quote = new Quote
                (
                    conversationId,
                    quoteId,
                    quoteInfo
                );

                await connection.ExecuteAsync("usp_Add_Quote", quote, null, null, CommandType.StoredProcedure);
            }
        }
    }
}
