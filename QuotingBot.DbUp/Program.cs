using System;
using System.Linq;
using System.Reflection;
using DbUp;

namespace QuotingBot.DbUp
{
    public static class Program
    {
        // -s "QuotingBotAlias" -d "QuotingBot" -u "QuotingBotDeployment" -p "QuotingBotDeployment" "-create" "-createlogins" "-test"
        static int Main(string[] args)
        {
            var connectionString = args.FirstOrDefault()
                                   //?? "Server=PCONNOLLY\\SQL2014; Database=QuotingBot; Trusted_connection=true";
                                   ?? "Server=DESKTOP-HL69CK9\\PCONNOLLY; Database=QuotingBot; Trusted_connection=true";

            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges
                .To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
