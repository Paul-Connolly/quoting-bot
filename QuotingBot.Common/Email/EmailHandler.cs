using System;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace QuotingBot.Common.Email
{
    public class EmailHandler
    {
        public EmailHandler() { }
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
        }
        public static void SendEmail(string toEmail, string toName, string body)
        {
            var mailServerAddress = ConfigurationManager.AppSettings["MailServerAddress"];
            var mailServerUser = ConfigurationManager.AppSettings["MailServerUser"];
            var mailServerPassword = ConfigurationManager.AppSettings["MailServerPassword"];
            var emailSenderAddress = ConfigurationManager.AppSettings["SenderEmailAddress"];
            var emailSenderName = ConfigurationManager.AppSettings["EmailSenderName"];

            SmtpClient client =
                new SmtpClient(mailServerAddress)
                {
                    Credentials = new NetworkCredential(mailServerUser, mailServerPassword)
                };

            var emailFrom = new MailAddress(emailSenderAddress, emailSenderName);
            var emailTo = new MailAddress(toEmail, toName);
            var message =
                new MailMessage(emailFrom, emailTo)
                {
                    Body = "Details of your insurance quote.",
                    Subject = "Insurace Quote"
                };
            
            // Set the method that is called back when the send operation ends.
            client.SendCompleted += new
            SendCompletedEventHandler(SendCompletedCallback);
            
            // The userState can be any object that allows your callback 
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            const string userState = "sending message";
            client.SendAsync(message, userState);
        }
    }
}
