using System;

using System.Collections.Generic;

using System.Configuration;

using System.Globalization;

using System.Linq;

using System.Net.Http;

using System.Net.Http.Headers;

using System.Text;

using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;

using Microsoft.Bot.Builder.FormFlow;

using Microsoft.Bot.Builder.Luis.Models;

using Microsoft.Bot.Connector;

using Microsoft.Lync.Model.Conversation;

using Newtonsoft.Json;

namespace LyncBot.Core.Dialogs
{
    [Serializable]
    class CoreDialogs : IDialog<object>
    {
        static Random rnd = new Random();

        private int unknownUtteranceCount = 0;

        private int promptCount = 0;



        private static List<string> yesStrings = new List<string> { "ya", "yes", "y", "yea", "yeah", "yess", "ok", "correct", "Yes", "YES" };

        private static List<string> noStrings = new List<string> { "no", "n", "wrong", "stop", "cancel", "noo", "abort", "No", "NO" };

        static List<string> cliamNoPrompts = new List<string> { "Invalid claim number! please enter a valid claim number.", "Thats not a valid claim number! try to give again.", "Sorry, its not a valid claim number! please provide a valid number.", "Oops, its not a claim number. Enter a valid number to proceed." };

        static List<string> cliamNoLastPrompt = new List<string> { "Oops, I didn't understand that. Please ask something else :)", "Again a wrong claim number. Please proceed with any other queries!", "I didn't get that. Your request has been failed. Try to ask your queries again!", "Oops! too many attempts, please ask your queries again!" };

        static List<string> UnknownUtterances = new List<string> { "Very sorry, I didn't get that!", "I'm unable to understand your question. Please ask 'help'.", "I didn't get you, try to ask your queries again!", "Sorry, I am not aware of this!" };

        
        protected string userMsg = String.Empty;

        private static int count = 0;

        private static int ApiCallCount = 0;



        protected string lastIntent = String.Empty;

        protected int visitCount = 1;



        Dictionary<string, string> calenderMonths = new Dictionary<string, string> { { "jan", "january" }, { "feb", "february" }, { "mar", "march" }, { "apr", "april" }, { "may", "may" }, { "jun", "june" }, { "jul", "july" }, { "aug", "august" }, { "sep", "september" }, { "oct", "october" }, { "nov", "november" }, { "dec", "december" } };




        public async Task StartAsync(IDialogContext context)
        {

            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var activity = await result as Activity;

            userMsg = activity.Text;

            string userName = GetName(activity.From).Replace(",", "");

            await context.PostAsync("Hi " + userName +"Welcome to Field support .");

            try

            {

                int length = (userMsg ?? string.Empty).Length;
            }
            catch (Exception) {

            }

            

        }
        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<int> result)
        {
            var ticketNumber = await result;

            await context.PostAsync($"Thanks for contacting our support team. Your ticket number is {ticketNumber}.");
            context.Wait(this.MessageReceivedAsync);
        }

        public static string GetName(ChannelAccount from)

        {

            string name = string.Empty;

            if (string.IsNullOrEmpty(from.Name))

                return name;



            var res = from.Name.Split(' ');

            if (res.Length > 1)

            {

                return res[1];

            }

            else

            {

                return res[0];

            }

        }



        


        public class ContextConstants
        {

            public const string LastQuery = "LastQuery";

            public const string ClaimNumber = "ClaimNumber";



            public const string LastIntent = "LastIntent";



            public const string FromPropmt = "FromPropmt";

        }
    }
}


