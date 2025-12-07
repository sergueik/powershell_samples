using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TryJira
{
    class Program
    {
        static void Main(string[] args)
        {
            Jira objJira = new Jira();
            objJira.Url = "YOUR_JIRA_URL";
            objJira.JsonString = @"{""fields""      :     {
                                    ""project""     :     {
                                    ""key""         :       ""YOUR_JIRA_PROJECT_KEY""                                  },
                                    ""summary""     :       ""ISSUE_SUMMARY""                                           ,
                                    ""description"" :       ""ISSUE_DESCRIPTION""                                       ,
                                    ""issuetype""   :     { 
                                    ""name""        :       ""ISSUE_TYPE""                                          }}}";
            objJira.UserName="YOUR_JIRA_USERNAME";
            objJira.Password="YOUR_JIRA_PASSWORD";
            objJira.filePaths = new List<string>() { "FILEPATH1","FILEPATH2" };
            objJira.AddJiraIssue();
        }
    }
}
