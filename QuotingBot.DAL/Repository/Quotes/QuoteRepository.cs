using Dapper;
using QuotingBot.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QuotingBot.DAL.Quotes
{
    public class QuoteRepository
    {
        public QuoteRepository() { }

        public async void StoreQuote(string conversationId, string quoteId, string quoteInfo)
        {
            using (var connection = new SqlConnection("Server=DESKTOP-HL69CK9\\PCONNOLLY; Database=QuotingBot; Trusted_connection=true"))
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
