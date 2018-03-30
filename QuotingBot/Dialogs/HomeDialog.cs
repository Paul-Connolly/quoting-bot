using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public class HomeDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Fail(new NotImplementedException("Home insurance dialog is not implemented and is instead being used to show context.Fail"));
        }
    }
}