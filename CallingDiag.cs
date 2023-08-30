using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static TATPlugin_Teams.Teams;
using static TATPlugin_Teams.GetInfo;
using static TATPlugin_Teams.Resource;


namespace TATPlugin_Teams
{
    class CallingDiag
    {
        public static List<string> g_allCallingLines = new List<string>();

        public static void ParseCallingDiag()
        {
            int iPos = 0;
            int iState = 0;
            int iTime = 0;
            int iComma = 0;
            int iCol = 0;
            int iRet = 0;
            bool EOL = false;
            string strState = "";
            string strTime = "";
            string strTR = "";

            GetCallIDs();

            g_allCallingLines.Add(GetHeaderInfo());
            g_allCallingLines.Add("");

            if (g_CallIDs.Count > 0)
            {
                g_allCallingLines.Add("CallIDs Found:");
                foreach (string strID in g_CallIDs)
                {
                    g_allCallingLines.Add(strID);
                }
                g_allCallingLines.Add("");
            }

            if (g_CallIDs.Count > 0)
            {
                g_allCallingLines.Add(GetFooterText());
            }
            else
            {
                g_allCallingLines.Add("No CallIds found.");
                g_allCallingLines.Add(GetFooterText());
            }

            foreach (string strLine in g_allLines)
            {
                string strCopy = strLine;

                if (strCopy.Contains("state") && strCopy.Contains("timestamp"))
                {
                    while (EOL == false)
                    {
                        iState = TextPos(strCopy, "\"state\":", iPos, false);
                        if (iState == -1 || iState < 20) // had to do < 20 because it would still be "found" sometimes, but not really found... wierd
                            break;
                        iComma = TextPos(strCopy, ",", iState, false);
                        iPos = iComma;
                        strState = strCopy.Substring(iState, iComma - (iState + 1));
                        strState = strState.Trim();
                        strState = GetCallState(strState);
                        strCopy = strCopy.Insert(iPos - 1, strState);

                        iTime = TextPos(strCopy, "\"timestamp\":", iPos, false);
                        strTime = strCopy.Substring(iTime, 13);
                        iPos = iTime + 13;
                        strTime = ConvertUnixTime(strTime);
                        strCopy = strCopy.Insert(iPos, " - " + strTime);
                        iPos = iPos + strTime.Length;
                    }
                }
                else if (strCopy.Contains("\"state\":"))
                {
                    iCol = TextPos(strCopy, ":", iPos, false);
                    iComma = TextPos(strCopy, ",", iCol, false);
                    strState = strCopy.Substring(iCol + 1, iComma - (iCol + 2));
                    strState = GetCallState(strState);
                    strCopy = strCopy.Insert(iComma - 1, strState);
                }
                else if (strCopy.Contains("\"timestamp\":") && !(strCopy.Contains("speaker")))
                {
                    iCol = TextPos(strCopy, ":", iPos, false);
                    iRet = strCopy.Length;
                    strTime = strCopy.Substring(iCol + 1, iRet - (iCol + 1));
                    if (strTime.Contains("."))
                        break;
                    if (strTime.Length == 13)
                    {
                        strTime = ConvertUnixTime(strTime);
                        iPos = iCol + 14;
                        strCopy = strCopy.Insert(iPos, " - " + strTime);
                    }
                }
                else if (strCopy.Contains("\"terminatedReason\":"))
                {
                    iCol = TextPos(strCopy, ":", iPos, false);
                    iComma = TextPos(strCopy, ",", iCol, false);
                    strTR = strCopy.Substring(iCol + 1, iComma - (iCol + 2));
                    strTR = GetTerminateReason(strTR);
                    strCopy = strCopy.Insert(iComma - 1, strTR);
                }

                iPos = iState = iTime = iComma = iCol = 0;
                g_allCallingLines.Add(strCopy);
            }
        } // ParseCallingDiag

        public static void WriteCallingDiag()
        {
            TextWriter tw = new StreamWriter(g_strParsedFile);
            foreach (string strLine in g_allCallingLines)
            {
                tw.WriteLine(strLine);
            }
            tw.Close();
        }
    }
}
