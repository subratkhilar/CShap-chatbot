﻿namespace LyncBot.Core.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;


    using System.Configuration;

    using System.Globalization;

    using System.Linq;

    using System.Net.Http;

    using System.Net.Http.Headers;

    using System.Text;

    using System.Text.RegularExpressions;

     using Microsoft.Bot.Builder.FormFlow;

    using Microsoft.Bot.Builder.Luis.Models;


    using Microsoft.Lync.Model.Conversation;

    using Newtonsoft.Json;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string SoftwareOption = "Software issue";

        private const string HardwareOption = "Hardware issue";

        static Random rnd = new Random();
        

        private static List<string> yesStrings = new List<string> { "ya", "yes", "y", "yea", "yeah", "yess", "ok", "correct", "Yes", "YES" };

        private static List<string> noStrings = new List<string> { "no", "n", "wrong", "stop", "cancel", "noo", "abort", "No", "NO" };

        static List<string> cliamNoPrompts = new List<string> { "Invalid claim number! please enter a valid claim number.", "Thats not a valid claim number! try to give again.", "Sorry, its not a valid claim number! please provide a valid number.", "Oops, its not a claim number. Enter a valid number to proceed." };

        static List<string> cliamNoLastPrompt = new List<string> { "Oops, I didn't understand that. Please ask something else :)", "Again a wrong claim number. Please proceed with any other queries!", "I didn't get that. Your request has been failed. Try to ask your queries again!", "Oops! too many attempts, please ask your queries again!" };

        static List<string> UnknownUtterances = new List<string> { "Very sorry, I didn't get that!", "I'm unable to understand your question. Please ask 'help'.", "I didn't get you, try to ask your queries again!", "Sorry, I am not aware of this!" };

        static List<string> greetingsFirstVisit =

            new List<string> { CurrentDateTime("IST").ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower() == "am" ? "Hey! @name, good morning.\nI'm Diana, Field Support Robotic Assistant \nThanks so much for reaching out ! Whats bring you to @fssupport today?" : "Hey! @name, good afternoon.\nI'm Diana, Field Support Robotic Assistant \nThanks so much for reaching out ! Whats bring you to @fssupport today?",

              CurrentDateTime("IST").ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower() == "am" ? "Good morning! @name\nI'm Diana, Field Support Robotic Assistant \nThanks so much for reaching out ! Whats bring you to @fssupport today?" : "Good afternoon! @name\nI'm Diana, Field Support Robotic Assistant \nThanks so much for reaching out ! Whats bring you to @fssupport today?",

            "Hi @name, I'm Diana, Field Support Robotic Assistant \nThanks so much for reaching out ! Whats bring you to @fssupport today?",

            "Hey! @name, Hope you are doing well. \nHow may I assist you?"};

        static List<string> greetingsNextVisits =

            new List<string>() { "Hey! @name, welcome back. Hope you are doing well.\nHow can I assist you?",

                 CurrentDateTime("CST").ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower() == "am" ? "Hi @name!, good morning." : "Hi @name!, good afternoon."+"\nTell me how can I help you?",

            CurrentDateTime("CST").ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower() == "am" ? "Welcome back @name! a very good morning." : "Welcome back @name! a very good afternoon."+"How may I help you?",

            CurrentDateTime("CST").ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower() == "am" ? "Good morning" : "Good afternoon"+" @name, nice to see you again!\nTell me how can I assis you today?"};

        protected string userMsg = String.Empty;

        private static int count = 0;

        private static int ApiCallCount = 0;

        protected string lastIntent = String.Empty;

        protected int visitCount = 1;

       

        Dictionary<string, string> calenderMonths = new Dictionary<string, string> { { "jan", "january" }, { "feb", "february" }, { "mar", "march" }, { "apr", "april" }, { "may", "may" }, { "jun", "june" }, { "jul", "july" }, { "aug", "august" }, { "sep", "september" }, { "oct", "october" }, { "nov", "november" }, { "dec", "december" } };



      


        public string issueType;
        public string query;
        public string osType;
        public  Task StartAsync(IDialogContext context)
        {
            /* Wait until the first message is received from the conversation and call MessageReceviedAsync 
             *  to process that message. */
            context.Wait(this.MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            /* When MessageReceivedAsync is called, it's passed an IAwaitable<IMessageActivity>. To get the message,
             *  await the result. */
            var message = await result;

            await this.SendWelcomeMessageAsync(context, result);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string userName = GetName(activity.From).Replace(",", "");

            await context.PostAsync(greetingsFirstVisit[rnd.Next(0, greetingsFirstVisit.Count - 1)].Replace("@name", userName));
           // PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { SoftwareOption, HardwareOption }, "please choose from below..?", "Not a valid option", 3);

            context.Call(new NameDialog(), this.NameDialogResumeAfter);
        }
        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case SoftwareOption:
                        context.Call(new NameDialog(), this.NameDialogResumeAfter);
                        break;

                    case HardwareOption:
                        context.Call(new NameDialog(), this.NameDialogResumeAfter);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
        }



        private async Task NameDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.issueType = await result;
                context.Call(new OSDialogs(this.issueType), this.OSDialogResumeAfter);

                // context.Call(new TechnicalIssueDialog(this.name), this.AgeDialogResumeAfter);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");

                await this.SendWelcomeMessageAsync(context, result);
            }
        }

        private async Task OSDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.osType = await result;

                context.Call(new TechnicalIssueDialog(this.issueType,this.osType), this.IssueDialogResumeAfter);


            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
            }
            finally
            {
                //  await this.SendWelcomeMessageAsync(context, result);
            }
        }

        private async Task IssueDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.query = await result;

                await context.PostAsync($"Your message { query } was registered. Once we resolve it; we will get back to you.");
                await context.PostAsync($"Thanks for contacting our support team");
                System.Console.WriteLine(" query >>"+ query +"os type"+osType +"issue type >>"+issueType);
                //rest call for service now ticket creation begin 
                CalltoAPI("", query);
                //rest call end
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
            }
            finally
            {
                //  await this.SendWelcomeMessageAsync(context, result);
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<int> result)
        {
            var ticketNumber = await result;

            await context.PostAsync($"Thanks for contacting our support team. Your ticket number is {ticketNumber}.");
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
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



        public static DateTime CurrentDateTime(string TimeZone)
        {

            DateTime datetimeUTC = DateTime.UtcNow;

            string timezoneId = string.Empty;

            switch (TimeZone)

            {

                case "IST":

                    timezoneId = "India Standard Time";

                    break;

                case "EST":

                    timezoneId = "Eastern Standard Time";

                    break;

                case "CST":

                    timezoneId = "Central Standard Time";

                    break;

                case "GMT":

                    return datetimeUTC;

            }

            return TimeZoneInfo.ConvertTimeFromUtc(datetimeUTC, TimeZoneInfo.FindSystemTimeZoneById(timezoneId));

        }



        private static async Task<string> CalltoAPI(string URL,string query)
        {

            string response = String.Empty;

            using (HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))

            {

                client.DefaultRequestHeaders.Accept.Add(

                new MediaTypeWithQualityHeaderValue("application/json"));
                ServiceRequest aServReq = new ServiceRequest { shortDesc = "Software-change", description = query };
                client.BaseAddress = new Uri("http://localhost:3019/");
                //var response = client.PostAsJsonAsync("api/person", ).Result;
               
                HttpResponseMessage msg = null;

                try

                {

                    msg = client.PutAsJsonAsync("/api/v1/incident", aServReq).Result; 

                }

                catch (Exception ex)
                {

                    
                }

                if (msg.IsSuccessStatusCode)

                {

                    try

                    {

                        response = await msg.Content.ReadAsStringAsync();



                    }

                    catch (Exception ex)

                    {

                        throw ex;

                    }

                }

            }

            return response;

        }

        public class ServiceRequest
        {
            public string shortDesc { get; set; }
            public string description { get; set; }
            public string category { get; set; }

            public string short_description { get; set; }
            public string contact_type { get; set; }
            public string email { get; set; }
            public string assignment_group { get; set; }
            public string caller_id { get; set; }
        }
    }
}
