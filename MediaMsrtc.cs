using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using static TATPlugin_Teams.Teams;
using static TATPlugin_Teams.GetInfo;
using static TATPlugin_Teams.Resource;
using static TATPlugin_Teams.GetMediaInfo;

namespace TATPlugin_Teams
{
    internal class MediaMsrtc
    {
        public static List<string> g_MediaLines = new List<string>();

        public static void ParseMediaLog()
        {
            string strDateTime = "This log covers: ";
            string strBWCap = "Bandwidth Caps: ";
            string strECSBWCap = "ECS Bandwidth Cap: ";

            g_MediaLines.Add(GetHeaderInfo());
            g_MediaLines.Add(strDateTime += GetLogDateTimes());
            g_MediaLines.Add("");

            GetCallIDs();

            if (g_CallIDs.Count > 0)
            {
                g_MediaLines.Add("CallIDs Found:");
                foreach (string strID in g_CallIDs)
                {
                    g_MediaLines.Add(strID);
                }
                g_MediaLines.Add("");
            }
            if (g_CallIDs.Count == 0)
            {
                g_MediaLines.Add("No CallIds found.");
            }

            g_MediaLines.Add(strBWCap += GetBWCap());
            g_MediaLines.Add(strECSBWCap += GetECSBWCap());

            g_MediaLines.Add(GetFooterText());
        }

        public static void WriteMediaLog()
        {
            string strInfo = string.Join(Environment.NewLine, g_MediaLines.ToArray());

            File.WriteAllText(g_strParsedFile, strInfo + g_allText);
        }
    }
}
