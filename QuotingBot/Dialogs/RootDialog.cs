using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hi, I'm Ava - your friendly quoting bot!");
            await Respond(context);
            context.Wait(MessageReceivedAsync);
        }

        private static async Task Respond(IDialogContext context)
        {
            var userName = String.Empty;
            context.UserData.TryGetValue<string>("Name", out userName);

            if (string.IsNullOrEmpty(userName))
            {
                await context.PostAsync("What is your name?");
                context.UserData.SetValue<bool>("GetName", true);
            }
            else
            {
                await context.PostAsync(String.Format("Hi {0}, how can I help you today?", userName));
            }
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var userName = String.Empty;

            context.UserData.TryGetValue<string>("Name", out userName);
            context.UserData.TryGetValue<bool>("GetName", out bool getName);

            if(getName)
            {
                userName = activity.Text;
                context.UserData.SetValue<string>("Name", userName);
                context.UserData.SetValue<bool>("GetName", false);
            }

            await Respond(context);
            context.Done(activity);
        }
    }
}