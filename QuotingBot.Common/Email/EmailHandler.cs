using System;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using QuotingBot.Common.RelayFullCycleMotorService;
using QuotingBot.Common.RelayHouseholdService;
using QuotingBot.Enums;

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

        private static SmtpClient SetupSmtpClient()
        {
            var mailServerAddress = ConfigurationManager.AppSettings["MailServerAddress"];
            var mailServerUser = ConfigurationManager.AppSettings["MailServerUser"];
            var mailServerPassword = ConfigurationManager.AppSettings["MailServerPassword"];

            var client =
                new SmtpClient(mailServerAddress)
                {
                    Credentials = new NetworkCredential(mailServerUser, mailServerPassword)
                };

            return client;
        }
        public static void SendEmailToUser(string toEmail, string toName, string body)
        {
            var emailSenderAddress = ConfigurationManager.AppSettings["SenderEmailAddress"];
            var emailSenderName = ConfigurationManager.AppSettings["EmailSenderName"];

            var emailFrom = new MailAddress(emailSenderAddress, emailSenderName);
            var emailTo = new MailAddress(toEmail, toName);
            var message = new MailMessage(emailFrom, emailTo)
            {
                Body = body,
                Subject = "Insurace Quote",
                IsBodyHtml = true
            };

            var client = SetupSmtpClient();
            // Set the method that is called back when the send operation ends.
            client.SendCompleted += new
            SendCompletedEventHandler(SendCompletedCallback);
            
            // The userState can be any object that allows your callback 
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            const string userState = "sending message";
            client.SendAsync(message, userState);
        }

        public static string BuildHomeEmailBodyForUser(HomeQuoteWebServiceResult[] responseQuotes,
            string firstName, string lastName, string contactNumber, string emailAddress, 
            string firstLineOfAddress, string town, string county, string propertyType,
            string residenceType, string yearBuilt, string numberOfBedrooms)
        {
            string body;

            body = $"Hi {firstName},<br><br>";
            body += "Thanks for getting your home insurance quote with us.<br><br>";
            body += "We've listed the quotes you received.<br><br>";
            body += "<table border=\"1\"><tbody>";
            body += "<tr><th>Insurer</th><th>Scheme</th><th>Total</th></tr>";

            foreach (var quote in responseQuotes)
            {
                if (quote.NetPremium > 0)
                {
                    body += $"<tr><td>{quote.InsurerName}</td><td>{quote.SchemeName}</td><td>€{quote.NetPremium}</td></tr>";
                }
            }

            body += "</tbody></table><br><br>";

            body += "Entered risk details:<br>";
            body += $"<strong>Name:</strong> {firstName} {lastName}<br>";
            body += $"<strong>Contact Number:</strong> {contactNumber}<br>";
            body += $"<strong>Email:</strong> {emailAddress}<br>";
            body += $"<strong>Address Line 1:</strong> {firstLineOfAddress}<br>";
            body += $"<strong>Town:</strong> {town}<br>";
            body += $"<strong>County:</strong> {county}<br>";
            body += $"<strong>Property:</strong> {propertyType}<br>";
            body += $"<strong>Residence:</strong> {residenceType}<br>";
            body += $"<strong>Year Built:</strong> {yearBuilt}<br>";
            body += $"<strong>No. of Bedrooms:</strong> {numberOfBedrooms}<br><br>";


            body += "Thanks,<br>";
            body += $"Ava - your friendly Quoting Bot {Emoji.GrinningFace}";

            return body;
        }

        public static string BuildMotorEmailBodyForUser(IrishMQResultsBreakdown[] quotes,
            string firstName, string lastName, string dateOfBirth, string contactNumber, string emailAddress,
            string vehicleRegistration, string vehicleDescription, string vehicleValue, string areaVehicleKept,
            string licenceType, string noClaimsDiscountYears)
        {
            string body;

            body = $"Hi {firstName},<br><br>";
            body += "Thanks for getting your home insurance quote with us.<br><br>";
            body += "We've listed the quotes you received.<br><br>";
            body += "<table border=\"1 solid\"><tbody>";
            body += "<tr><th>Insurer</th><th>Total</th></tr>";

            foreach (var quote in quotes)
            {
                if (quote.Premium.TotalPremium > 0)
                {
                    body += $"<tr><td>{quote.Premium.SchemeName}</td><td>€{quote.Premium.TotalPremium}</td></tr>";
                }
            }

            body += "</tbody></table><br><br>";

            body += "Entered risk details:<br>";
            body += $"<strong>Name:</strong> {firstName} {lastName}<br>";
            body += $"<strong>Date of Birth:</strong> {dateOfBirth}<br>";
            body += $"<strong>Contact Number:</strong> {contactNumber}<br>";
            body += $"<strong>Email:</strong> {emailAddress}<br>";
            body += $"<strong>Vehicle Registration:</strong> {vehicleRegistration}<br>";
            body += $"<strong>Vehicle Description:</strong> {vehicleDescription}<br>";
            body += $"<strong>Vehilce Value:</strong> €{vehicleValue}<br>";
            body += $"<strong>Area Vehilce Kept:</strong> {areaVehicleKept}<br>";
            body += $"<strong>Licence:</strong> {licenceType}<br>";
            body += $"<strong>No Claims Discount:</strong> {noClaimsDiscountYears}<br><br>";

            body += "Thanks,<br>";
            body += $"Ava - your friendly Quoting Bot {Emoji.GrinningFace}";

            return body;
        }

        public static void SendEmailToBroker(string toEmail, string toName, string body)
        {
            var emailSenderAddress = ConfigurationManager.AppSettings["SenderEmailAddress"];
            var emailSenderName = ConfigurationManager.AppSettings["EmailSenderName"];

            var emailFrom = new MailAddress(emailSenderAddress, emailSenderName);
            var emailTo = new MailAddress(toEmail, toName);
            var message = new MailMessage(emailFrom, emailTo)
            {
                Body = body,
                Subject = "Customer Insurace Quote",
                IsBodyHtml = true
            };

            var client = SetupSmtpClient();
            // Set the method that is called back when the send operation ends.
            client.SendCompleted += new
                SendCompletedEventHandler(SendCompletedCallback);

            // The userState can be any object that allows your callback 
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            const string userState = "sending message";
            client.SendAsync(message, userState);
        }

        public static string BuildMotorEmailBodyForBroker(IrishMQResultsBreakdown[] quotes,
            string firstName, string lastName, string dateOfBirth, string contactNumber, string emailAddress,
            string vehicleRegistration, string vehicleDescription, string vehicleValue, string areaVehicleKept,
            string licenceType, string noClaimsDiscountYears)
        {
            string body;

            body = $"Hi,<br><br>";
            body += "An insurance quote was given through the chatbot.<br><br>";
            body += "We've listed the quotes given.<br><br>";
            body += "<table border=\"1 solid\"><tbody>";
            body += "<tr><th>Insurer</th><th>Total</th></tr>";

            foreach (var quote in quotes)
            {
                if (quote.Premium.TotalPremium > 0)
                {
                    body += $"<tr><td>{quote.Premium.SchemeName}</td><td>€{quote.Premium.TotalPremium}</td></tr>";
                }
            }

            body += "</tbody></table><br><br>";

            body += "Entered risk details:<br>";
            body += $"<strong>Name:</strong> {firstName} {lastName}<br>";
            body += $"<strong>Date of Birth:</strong> {dateOfBirth}<br>";
            body += $"<strong>Contact Number:</strong> {contactNumber}<br>";
            body += $"<strong>Email:</strong> {emailAddress}<br>";
            body += $"<strong>Vehicle Registration:</strong> {vehicleRegistration}<br>";
            body += $"<strong>Vehicle Description:</strong> {vehicleDescription}<br>";
            body += $"<strong>Vehilce Value:</strong> €{vehicleValue}<br>";
            body += $"<strong>Area Vehilce Kept:</strong> {areaVehicleKept}<br>";
            body += $"<strong>Licence:</strong> {licenceType}<br>";
            body += $"<strong>No Claims Discount:</strong> {noClaimsDiscountYears}<br><br>";

            body += "Thanks,<br>";
            body += $"Ava - your friendly Quoting Bot {Emoji.GrinningFace}";

            return body;
        }

        public static string BuildHomeEmailBodyForBroker(HomeQuoteWebServiceResult[] responseQuotes,
            string firstName, string lastName, string contactNumber, string emailAddress,
            string firstLineOfAddress, string town, string county, string propertyType,
            string residenceType, string yearBuilt, string numberOfBedrooms)
        {
            string body;

            body = $"Hi,<br><br>";
            body += "An insurance quote was given through the chatbot.<br><br>";
            body += "We've listed the quotes given.<br><br>";
            body += "<table border=\"1\"><tbody>";
            body += "<tr><th>Insurer</th><th>Scheme</th><th>Total</th></tr>";

            foreach (var quote in responseQuotes)
            {
                if (quote.NetPremium > 0)
                {
                    body += $"<tr><td>{quote.InsurerName}</td><td>{quote.SchemeName}</td><td>€{quote.NetPremium}</td></tr>";
                }
            }

            body += "</tbody></table><br><br>";

            body += "Entered risk details:<br>";
            body += $"<strong>Name:</strong> {firstName} {lastName}<br>";
            body += $"<strong>Contact Number:</strong> {contactNumber}<br>";
            body += $"<strong>Email:</strong> {emailAddress}<br>";
            body += $"<strong>Address Line 1:</strong> {firstLineOfAddress}<br>";
            body += $"<strong>Town:</strong> {town}<br>";
            body += $"<strong>County:</strong> {county}<br>";
            body += $"<strong>Property:</strong> {propertyType}<br>";
            body += $"<strong>Residence:</strong> {residenceType}<br>";
            body += $"<strong>Year Built:</strong> {yearBuilt}<br>";
            body += $"<strong>No. of Bedrooms:</strong> {numberOfBedrooms}<br><br>";


            body += "Thanks,<br>";
            body += $"Ava - your friendly Quoting Bot {Emoji.GrinningFace}";

            return body;
        }
    }
}
