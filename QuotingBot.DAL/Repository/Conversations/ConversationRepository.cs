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
        private string Connection { get; }
        public ConversationRepository(string connection)
        {
            Connection = connection;
        }

        public async void StoreConversation(string conversationId, string userId, string conversationDate, string conversationLog)
        {
            try
            {
                using (var connection = new SqlConnection(Connection))
                {
                    await connection.OpenAsync();
                    var conversation = new Conversation
                    (
                        conversationId,
                        userId,
                        conversationDate,
                        conversationLog
                    );

                    await connection.ExecuteAsync(
                        "usp_Add_Conversation", 
                        conversation, 
                        null, null, 
                        CommandType.StoredProcedure);
                }
            }
            catch(Exception exception)
            {
                var errorRepository = new ErrorRepository();
                errorRepository.LogError(conversationId, userId, DateTime.Now.ToString(), conversationLog, exception.ToString());
                throw;
            }
        }
    }
}
