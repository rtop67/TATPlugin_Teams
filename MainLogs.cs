using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static TATPlugin_Teams.GetInfo;
using static TATPlugin_Teams.Teams;
using static TATPlugin_Teams.Resource;


namespace TATPlugin_Teams
{
    class MainLogs
    {
        public static List<string> g_MainInfoLines = new List<string>();

        public static void ParseMainLogs()
        {
            string strDateTime = "This log covers: ";
            string strName = "Name: ";
            string strUserName = "UserName: ";
            string strObjectID = "ObjectID: ";
            string strTenantID = "TenantID: ";
            string strTeamsVer = "Teams Version: ";
            string strSlimcoreVer = "Slimcore Version: ";
            string strTeamsRing = "Teams Ring: ";
            string strClientType = "Client Type: ";
            string strVdiMode = "VDI Mode: ";
            string strOSVer = "OS Version: ";
            string strTZ = "Timezone: ";
            string strHostname = "Hostname: ";
            string strIPAddr = "IP Address: ";
            string strDeviceInfo = "Device Info: ";
            string strCPU = "CPU: ";
            string strMem = "Total Memory: ";

            g_MainInfoLines.Add(GetHeaderInfo());

            if (g_strFileType == "mainlog" || g_strFileType == "maindiag")
            {
                g_MainInfoLines.Add(strDateTime += GetLogDateTimes());
                g_MainInfoLines.Add(strTZ += GetTimezone());
                g_MainInfoLines.Add("");
                g_MainInfoLines.Add(strName += GetName());
                g_MainInfoLines.Add(strUserName += GetUserName());
                g_MainInfoLines.Add(strObjectID += GetOID());
                g_MainInfoLines.Add(strTenantID += GetTenantID());
                g_MainInfoLines.Add("");
                g_MainInfoLines.Add(strTeamsVer += GetTeamsVer());
                g_MainInfoLines.Add(strSlimcoreVer += GetSlimcoreVer());
                g_MainInfoLines.Add(strTeamsRing += GetRing());
                g_MainInfoLines.Add(strClientType += GetClientType());
                g_MainInfoLines.Add(strVdiMode += GetVdiMode());
                g_MainInfoLines.Add("");
                g_MainInfoLines.Add(strOSVer += GetOSVer());
                g_MainInfoLines.Add(strIPAddr += GetIPAddr());
                g_MainInfoLines.Add("");
                GetCrashLines();
            }

            if (g_strFileType == "maindiag")
            {
                g_MainInfoLines.Add(strHostname += GetHostname());
                g_MainInfoLines.Add(strDeviceInfo += GetDeviceInfo());
                g_MainInfoLines.Add(strCPU += GetCPU());
                g_MainInfoLines.Add(strMem += GetMem());
                g_MainInfoLines.Add("");
                GetCallIDs();
            }

            if (g_CallIDs.Count > 0)
            {
                g_MainInfoLines.Add("CallIDs Found:");
                foreach (string strID in g_CallIDs)
                {
                    g_MainInfoLines.Add(strID);
                }
                g_MainInfoLines.Add("");
            }

            if (g_CrashLines.Count > 0)
            {
                g_MainInfoLines.Add("Crash Info:");
                foreach (string strLine in g_CrashLines)
                {
                    g_MainInfoLines.Add(strLine);
                }
                g_MainInfoLines.Add("");
            }

            g_MainInfoLines.Add(GetFooterText());
        }

        public static void WriteMainLog()
        {
            string strInfo = string.Join(Environment.NewLine, g_MainInfoLines.ToArray());

            File.WriteAllText(g_strParsedFile, strInfo + g_allText);
        }
    }
}
