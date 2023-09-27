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

            // !!!!!  NEED TO ADD GETTING CALLIDs HERE
            if (g_CallIDs.Count > 0)
            {
                g_MediaLines.Add("CallIDs Found:");
                foreach (string strID in g_CallIDs)
                {
                    g_MediaLines.Add(strID);
                }
                g_MediaLines.Add("");
            }
            else if (g_CallIDs.Count == 0)
            {
                g_MediaLines.Add("No CallIds found.");
            }

            g_MediaLines.Add(strBWCap += GetBWCap());
            g_MediaLines.Add(strECSBWCap += GetECSBWCap());
            g_MediaLines.Add("");

            GetVidDev();
            if (g_VideoDevices.Count > 0)
            {
                g_MediaLines.Add("Video Devices Used:");
                foreach (string strDev in g_VideoDevices)
                {
                    g_MediaLines.Add(strDev);
                }
                g_MediaLines.Add("");
            }
            else if (g_VideoDevices.Count == 0)
            {
                g_MediaLines.Add("No Video or Screen Sharing from this user.");
            }

            g_MediaLines.Add(GetFooterText());
        }

        public static void WriteMediaLog()
        {
            string strInfo = string.Join(Environment.NewLine, g_MediaLines.ToArray());

            File.WriteAllText(g_strParsedFile, strInfo + g_allText);
        }
    }
}
