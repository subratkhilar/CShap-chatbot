namespace LyncBot.Core.Dialogs
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class TechnicalIssueQuery
    {
        [Prompt("Do you have Software or Hardware Issue ?")]
        public string IssueType { get; set; }
        
        [Prompt("Please Enter the Operating system you are using?")]
        public string osType { get; set; }
        
        [Prompt("Please Enter the extact issue you are facing ?")]
        public int Query { get; set; }
    }
}