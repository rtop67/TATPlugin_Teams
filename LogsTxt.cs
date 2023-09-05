using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static TATPlugin_Teams.GetInfo;
using static TATPlugin_Teams.Teams;
using static TATPlugin_Teams.Resource;

namespace TATPlugin_Teams
{
    internal class LogsTxt
    {
        public static List<string> g_LogsTxtLines = new List<string>();

        public static void ParseLogsTxt()
        {
            string strDateTime = "This log covers: ";
            string strTZ = "Timezone: ";
            string strName = "Name: ";
            string strUserName = "UserName: ";
            string strUpn = "UPN: ";
            string strObjectID = "ObjectID: ";
            string strTenantID = "TenantID: ";
            string strTeamsVer = "Teams Version: ";
            string strSlimcoreVer = "Slimcore Version: ";
            string strBuildDate = "Build Date: ";
            string strTeamsRing = "Teams Ring: ";
            string strClientType = "Client Type: ";
            string strVdiMode = "VDI Mode: ";
            string strOSVer = "OS Version: ";
            string strIPAddr = "IP Address: ";

            g_LogsTxtLines.Add(GetHeaderInfo());

            g_LogsTxtLines.Add(strDateTime += GetLogDateTimes());
            g_LogsTxtLines.Add(strTZ += GetTimezone());
            g_LogsTxtLines.Add("");
            g_LogsTxtLines.Add(strName += GetName());
            g_LogsTxtLines.Add(strUserName += GetUserName());
            g_LogsTxtLines.Add(strUpn += GetUpn());
            g_LogsTxtLines.Add(strObjectID += GetOID());
            g_LogsTxtLines.Add(strTenantID += GetTenantID());
            g_LogsTxtLines.Add("");
            g_LogsTxtLines.Add(strTeamsVer += GetTeamsVer());
            g_LogsTxtLines.Add(strSlimcoreVer += GetSlimcoreVer());
            g_LogsTxtLines.Add(strBuildDate += GetBuildDate());
            g_LogsTxtLines.Add(strTeamsRing += GetRing());
            g_LogsTxtLines.Add(strClientType += GetClientType());
            g_LogsTxtLines.Add(strVdiMode += GetVdiMode());
            g_LogsTxtLines.Add("");
            g_LogsTxtLines.Add(strOSVer += GetOSVer());
            g_LogsTxtLines.Add(strIPAddr += GetIPAddr());
            g_LogsTxtLines.Add("");
            GetCrashLines();

            if (g_CrashLines.Count > 0)
            {
                g_LogsTxtLines.Add("Crash Info:");
                foreach (string strLine in g_CrashLines)
                {
                    g_LogsTxtLines.Add(strLine);
                }
                g_LogsTxtLines.Add("");
            }

            g_LogsTxtLines.Add(GetFooterText());
        } // ParseLogsTxt

        public static void WriteLogsTxtInfoToLog()
        {
            string strInfo = string.Join(Environment.NewLine, g_LogsTxtLines.ToArray());

            File.WriteAllText(g_strParsedFile, strInfo + g_allText);
        }
    }
}
