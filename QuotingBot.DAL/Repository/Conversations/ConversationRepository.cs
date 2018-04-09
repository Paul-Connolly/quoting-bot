using Dapper;
using QuotingBot.DAL.Models;
using QuotingBot.DAL.Repository.Errors;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QuotingBot.DAL.Repository.Conversations
{
    public class ConversationRepository
    {
        public ConversationRepository() { }

        public async void StoreConversation(string conversationId, string userId, string date, string conversationLog)
        {
            try
            {


                using (var connection = new SqlConnection("Server=DESKTOP-HL69CK9\\PCONNOLLY; Database=QuotingBot; Trusted_connection=true"))
                {
                    await connection.OpenAsync();
                    var conversation = new Conversation
                    (
                        conversationId,
                        userId,
                        date,
                        conversationLog
                    );

                    await connection.ExecuteAsync("usp_Add_Conversation", conversation, null, null, CommandType.StoredProcedure);
                }
            }
            catch(Exception exception)
            {
                var errorRepository = new ErrorRepository();
                errorRepository.LogError(conversationId, userId, DateTime.Now.ToString(), conversationLog, exception.InnerException.ToString());
                throw;
            }
        }
    }
}
