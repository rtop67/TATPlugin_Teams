using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using static TATPlugin_Teams.Teams;
using static TATPlugin_Teams.GetInfo;
using static TATPlugin_Teams.Resource;
using System.Runtime.InteropServices;


namespace TATPlugin_Teams
{
    class CallingDiag
    {
        public static List<string> g_allCallingLines = new List<string>();

        public static void ParseCallingDiag()
        {
            g_allCallingLines.Add(GetHeaderInfo());
            g_allCallingLines.Add("");
            g_allCallingLines.Add("CallIDs Summary");
            g_allCallingLines.Add("===============");

            GetCallDiagData();

            g_allCallingLines.Add("");

            g_allCallingLines.Add(GetFooterText());

        } // ParseCallingDiag


        public static void GetCallDiagData()
        {
            int iUnixTimeLen = 13;

            string strState = "";
            string strTime = "";
            string strTR = "";
            string strCallID = "";
            string strCallType = "";
            string strPartID = "";
            string strConvID = "";
            string strPhone = "";
            string strTenantID = "";
            string strOrgID = "";

            string strRxPartID1 = @"\""participantId\"": \""([a-fA-F0-9-]+)\""";
            string strRxPartID2 = @"participantId: ([a-fA-F0-9-]+)";
            string strRxPhone = @"\""mri\""\s*:\s*\""4:(\S+)\""";
            string strRxCallType = @"\""callType\""\s*:\s*\""(\w+)\""";
            string strRxConvID = @"\""conversationId\"": \""(19:[^\""]+|[^\""]+@thread\.v2)\""";
            string strRxTenantID = @"\""tenantId\"": \""([a-fA-F0-9-]+)\""";
            string strRxOrgID = @"\""organizerId\"": \""([a-fA-F0-9-]+)\""";
            string strRxState = @"""state"":\s*(\d+)";
            string strRxTime = @"""timestamp"":\s*(\d+)";
            string strRxStateTime = @"{\""state\"":(\d+),\""timestamp\"":(\d+)}";  // when both are stuck together in the same line...
            string strRxTR = @"""terminatedReason"":\s*(\d+)";

            foreach (string strLine in g_allLines)
            {
                // there's some lines that "find" things in a false-positive way... so ignore those.
                if (strLine.StartsWith("Active calls info"))
                    continue;
                if (strLine.StartsWith("teamsCallId:"))
                    continue;
                if (strLine.StartsWith("setupArgs:"))
                    continue;
                if (strLine.StartsWith("Dominant Speaker:"))
                    continue;

                if (strLine.Contains("\"callId\":") || strLine.Contains("CallId:"))
                {
                    strCallID = GetCallingCallID(strLine);

                    // reset other strings since a new callid was found - new data for that call can be collected
                    strPartID = "";
                    strConvID = "";
                    strCallType = "";
                    strPhone = "";
                    strTenantID = "";
                    strOrgID = "";
                    strState = "";
                    strTime = "";
                    strTR = "";

                    AddCallID(strCallID); // add callid to global list
                    g_allCallingLines.Add("");
                    g_allCallingLines.Add("CallID: " + strCallID); // Add to local AllCallingLines
                }

                if (strLine.Contains("\"participantId\":") || strLine.Contains("participantId:"))
                {
                    if (string.IsNullOrEmpty(strPartID))
                    {
                        Match rxMatch = Regex.Match(strLine, strRxPartID1);
                        if (rxMatch.Success)
                        {
                            strPartID = rxMatch.Groups[1].Value;
                        }
                        else
                        {
                            rxMatch = Regex.Match(strLine, strRxPartID2);
                            if (rxMatch.Success)
                            {
                                strPartID = rxMatch.Groups[1].Value;
                            }
                        }

                        g_allCallingLines.Add("ParticipantID: " + strPartID);
                    }
                }

                if (strLine.Contains("\"conversationId\":"))
                {
                    if (strLine.Contains("null,"))
                        continue;

                    if (string.IsNullOrEmpty(strConvID))
                    {
                        Match rxMatch = Regex.Match(strLine, strRxConvID);
                        strConvID = rxMatch.Groups[1].Value;

                        if (!string.IsNullOrEmpty(strConvID))
                        {
                            if (!strConvID.Contains("meeting"))
                            {
                                if (strConvID.Contains("19:preview"))
                                    strConvID = strConvID + " (PSTN Call)";
                                else
                                    strConvID = strConvID + " (P2P Call)";
                            }
                            g_allCallingLines.Add("ConversationID: " + strConvID);
                        }
                    }
                }

                if (strLine.Contains("\"mri\":"))
                {
                    if (string.IsNullOrEmpty(strPhone))
                    {
                        Match rxMatch = Regex.Match(strLine, strRxPhone);
                        strPhone = rxMatch.Groups[1].Value;

                        if (!string.IsNullOrEmpty(strPhone))
                            g_allCallingLines.Add("Outgoing Phone#: " + strPhone);
                    }
                }

                if (strLine.Contains("\"callType\":"))
                {
                    if (string.IsNullOrEmpty(strCallType))
                    {
                        Match rxMatch = Regex.Match(strLine, strRxCallType);
                        strCallType = rxMatch.Groups[1].Value;

                        if (!string.IsNullOrEmpty(strCallType))
                            g_allCallingLines.Add("Call Type: " + strCallType);
                    }
                }

                if (strLine.Contains("\"tenantId\":"))
                {
                    if (string.IsNullOrEmpty(strTenantID))
                    {
                        Match rxMatch = Regex.Match(strLine, strRxTenantID);
                        strTenantID = rxMatch.Groups[1].Value;

                        if (!string.IsNullOrEmpty(strTenantID))
                            g_allCallingLines.Add("TenantID: " + strTenantID);
                    }
                }

                if (strLine.Contains("\"organizerId\":"))
                {
                    if (string.IsNullOrEmpty(strOrgID))
                    {
                        Match rxMatch = Regex.Match(strLine, strRxOrgID);
                        strOrgID = rxMatch.Groups[1].Value;

                        if (!string.IsNullOrEmpty(strOrgID))
                            g_allCallingLines.Add("Organizer ObjectID: " + strOrgID);
                    }
                }

                if (strLine.Contains("state") && strLine.Contains("timestamp")) // the logging sometimes has call states in one long unreadable line
                {
                    MatchCollection mcMatches = Regex.Matches(strLine, strRxStateTime);

                    if (mcMatches.Count > 0)
                    {
                        foreach (Match mcMatch in mcMatches)
                        {
                            strState = mcMatch.Groups[1].Value;
                            strTime = mcMatch.Groups[2].Value;

                            strState = strState + GetCallState(strState);
                            strTime = ConvertUnixTime(strTime);

                            g_allCallingLines.Add(strTime + " " + "Call State: " + strState);
                        }
                    }
                }
                else if (strLine.Contains("\"state\":"))
                {
                    Match rxMatch = Regex.Match(strLine, strRxState);
                    strState = rxMatch.Groups[1].Value;
                    if (!string.IsNullOrEmpty(strState))
                    {
                        strState = strState + GetCallState(strState);
                    }
                }
                else if (strLine.Contains("\"timestamp\":"))
                {
                    Match rxMatch = Regex.Match(strLine, strRxTime);
                    strTime = rxMatch.Groups[1].Value;
                    if (strTime.Contains(".") || strTime.Length != iUnixTimeLen)
                        break;
                    if (!string.IsNullOrEmpty(strTime))
                    {
                        strTime = ConvertUnixTime(strTime);
                    }

                    g_allCallingLines.Add(strTime + " " + "Call State: " + strState);
                }

                if (strLine.Contains("\"terminatedReason\":"))
                {
                    if (string.IsNullOrEmpty(strTR))
                    {
                        Match rxMatch = Regex.Match(strLine, strRxTR);
                        strTR = rxMatch.Groups[1].Value;
                        if (!string.IsNullOrEmpty(strTR))
                        {
                            strTR = GetTerminateReason(strTR);
                            
                            g_allCallingLines.Add("Terminated Reason: " + strTR);
                        }
                    }
                }
            }
        } // GetCallDiagData

        // Get the CallID from a line of text
        public static string GetCallingCallID(string strLine)
        {
            string strCallID = "";
            string strRxCall1 = @"\""callId\"": \""([a-fA-F0-9-]+)\""";
            string strRxCall2 = @"CallId: ([a-fA-F0-9-]+)";

            Match rxMatch = Regex.Match(strLine, strRxCall1);
            if (rxMatch.Success)
            {
                strCallID = rxMatch.Groups[1].Value;
            }
            else
            {
                rxMatch = Regex.Match(strLine, strRxCall2);
                if (rxMatch.Success)
                {
                    strCallID = rxMatch.Groups[1].Value;
                }
            }

            return strCallID;
        }

        public static void WriteCallingDiag()
        {
            string strInfo = string.Join(Environment.NewLine, g_allCallingLines.ToArray());

            File.WriteAllText(g_strParsedFile, strInfo + g_allText);
        }
    }
}
