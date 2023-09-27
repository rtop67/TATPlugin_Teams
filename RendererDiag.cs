using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static TATPlugin_Teams.Teams;
using static TATPlugin_Teams.GetInfo;
using static TATPlugin_Teams.Resource;

namespace TATPlugin_Teams
{
    class RendererDiag
    {
        public static List<string> g_RendererLines = new List<string>();

        public static void ParseRendererDiag()
        {
            string strDateTime = "This log covers: ";
            string strTZ = "Timezone: ";

            // !!!!!! NEED TO ADD GETTING CALLIDs HERE
            GetCrashLines();
            

            g_RendererLines.Add(GetHeaderInfo());
            g_RendererLines.Add(strDateTime += GetLogDateTimes());
            g_RendererLines.Add(strTZ + "Get time zone info from logs.txt or the main diagnostic log.");
            g_RendererLines.Add("");

            if (g_CallIDs.Count > 0)
            {
                g_RendererLines.Add("CallIDs Found:");
                foreach (string strID in g_CallIDs)
                {
                    g_RendererLines.Add(strID);
                }
                g_RendererLines.Add("");
            }

            if (g_CrashLines.Count > 0)
            {
                g_RendererLines.Add("Crash Info:");
                foreach (string strLine in g_CrashLines)
                {
                    g_RendererLines.Add(strLine);
                }
                g_RendererLines.Add("");
            }

            if (g_CallIDs.Count > 0 || g_CrashLines.Count > 0)
            {
                g_RendererLines.Add(GetFooterText());
            }
            else
            {
                g_RendererLines.Add("No CallIds or Crashes found.");
                g_RendererLines.Add(GetFooterText());

            }
        }

        public static void WriteRendererDiag()
        {
            string strInfo = string.Join(Environment.NewLine, g_RendererLines.ToArray());

            File.WriteAllText(g_strParsedFile, strInfo + g_allText);
        }
    }
}
