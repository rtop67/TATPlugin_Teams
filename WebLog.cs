using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using static TATPlugin_Teams.GetInfo;
using static TATPlugin_Teams.Teams;
using static TATPlugin_Teams.Resource;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TATPlugin_Teams
{
    internal class WebLog
    {
        // All WebLog lines to be added to the text
        public static List<string> g_WebLogLines = new List<string>();

        // Entries for below list
        public class g_CallDataEntry
        {
            public string strID {  get; set; }
            public DateTime datetime { get; set; }
            public string strProp { get; set; }
            public string strVal { get; set; }
        }

        // Call data entry list
        public static List<g_CallDataEntry> g_CallData = new List<g_CallDataEntry>();

        // For comparing Date-Times in the list for sorting  -  sort using:  g_CallData.Sort(new DateTimeComparer());
        public class DateTimeComparer : IComparer<g_CallDataEntry>
        {
            public int Compare(g_CallDataEntry x, g_CallDataEntry y)
            {
                return x.datetime.CompareTo(y.datetime);
            }
        }
        // Time format for Date Times when they get brought in - to keep things easy to read and in the same format
        public static string timeFmt = "yyyy-MM-dd HH:mm:ss.fff";

        public static void ParseWebLog()
        {
            string strDateTime = "This log covers: ";
            string strTZ = "Timezone: ";
            string strName = "Name: ";
            string strUserName = "UserName: ";
            string strUpn = "UPN: ";
            string strObjectID = "ObjectID: ";
            string strTenantID = "TenantID: ";
            string strTroutCon = "Trouter Connected: ";
            string strTroutUrl = "Trouter URL: ";
            string strTeamsVer = "Teams Version: ";
            string strSlimcoreVer = "Slimcore Version: ";
            string strBuildDate = "Build Date: ";
            string strTeamsRing = "Teams Ring: ";
            string strClientType = "Client Type: ";
            string strVdiMode = "VDI Mode: ";
            string strOSVer = "OS Version: ";
            string strIPAddr = "IP Address: ";
            string strHostname = "Hostname: ";
            string strDeviceInfo = "Device Info: ";
            string strCPU = "CPU: ";
            string strMem = "Total Memory: ";

            g_WebLogLines.Add(GetHeaderInfo());

            g_WebLogLines.Add(strDateTime += GetLogDateTimes());
            g_WebLogLines.Add(strTZ += GetTimezone());
            g_WebLogLines.Add("");
            g_WebLogLines.Add(strName += GetName());
            g_WebLogLines.Add(strUserName += GetUserName());
            g_WebLogLines.Add(strUpn += GetUpn());
            g_WebLogLines.Add(strObjectID += GetOID());
            g_WebLogLines.Add(strTenantID += GetTenantID());
            g_WebLogLines.Add(strTroutCon += GetTrouterConn());
            g_WebLogLines.Add(strTroutUrl += GetTrouterUrl());
            g_WebLogLines.Add("");
            g_WebLogLines.Add(strTeamsVer += GetTeamsVer());
            g_WebLogLines.Add(strSlimcoreVer += GetSlimcoreVer());
            g_WebLogLines.Add(strBuildDate += GetBuildDate());
            g_WebLogLines.Add(strTeamsRing += GetRing());
            g_WebLogLines.Add(strClientType += GetClientType());
            g_WebLogLines.Add(strVdiMode += GetVdiMode());
            g_WebLogLines.Add("");
            g_WebLogLines.Add(strOSVer += GetOSVer());
            g_WebLogLines.Add(strIPAddr += GetIPAddr());
            g_WebLogLines.Add("");
            g_WebLogLines.Add(strHostname += GetHostname());
            g_WebLogLines.Add(strDeviceInfo += GetDeviceInfo());
            g_WebLogLines.Add(strCPU += GetCPU());
            g_WebLogLines.Add(strMem += GetMem());
            g_WebLogLines.Add("");
            GetCallData();

            if (g_CallIDs.Count > 0)
            {
                g_WebLogLines.Add("CallIDs Found:");
                foreach (string strID in g_CallIDs)
                {
                    g_WebLogLines.Add("\r\n" + "CallID: " + strID);
                    if (g_CallData.Count > 0)
                    {
                        foreach (g_CallDataEntry entry in g_CallData)
                        {
                            if (entry.strID == strID)
                            {
                                string strDT = entry.datetime.ToString(timeFmt);
                                strDT = strDT + " GMT";
                                string strLine = strDT + " " + entry.strProp + " " + entry.strVal;
                                g_WebLogLines.Add(strLine);
                            }
                        }
                    }

                }
                
                g_WebLogLines.Add("");
            }

            g_WebLogLines.Add(GetFooterText());
        } // ParseWebLog

        public static void WriteWebLogInfoToLog()
        {
            string strInfo = string.Join(Environment.NewLine, g_WebLogLines.ToArray());

            File.WriteAllText(g_strParsedFile, strInfo + g_allText);
        }

        // Try to find CallIDs with ParticipantID for this user, CallStates for each CallID, and Call End data
        public static void GetCallData()
        {
            string strCallID = "";
            string strRxPartIDs = @"(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z)\s+.*callId=([a-f\d]{8}(?:-[a-f\d]{4}){3}-[a-f\d]{12}).*participantId=([a-f\d]{8}(?:-[a-f\d]{4}){3}-[a-f\d]{12})"; // Get CallID & ParticipantID
            string strRxCallStates = @"(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z)\s+.*callId\s=\s(\S+).*state\s=\s(\w+),"; // gets date-time, callid, and state

            MatchCollection mcPartIDs = Regex.Matches(g_allText, strRxPartIDs);
            MatchCollection mcCallStates = Regex.Matches(g_allText, strRxCallStates);

            if (mcPartIDs.Count > 0)
            {
                for (int i = mcPartIDs.Count - 1; i >= 0; i--)  // gotta do reverse because weblog logs in reverse...
                {
                    string strDT = mcPartIDs[i].Groups[1].Value;
                    strCallID = mcPartIDs[i].Groups[2].Value;
                    string strPartID = mcPartIDs[i].Groups[3].Value;

                    AddCallID(strCallID); // Add the CallID to the list of found CallIDs in the log

                    // Put this in a list to be added to the WebLogLines later after sorting by date
                    AddCallDataEntry(strCallID, strDT, "ParticipantID", strPartID);

                }
            }

            // Now go get Calls + CallStates
            if (mcCallStates.Count > 0)
            {
                for (int i = mcCallStates.Count - 1; i >= 0; i--)  // gotta do reverse because weblog logs in reverse...
                {
                    string strDT = mcCallStates[i].Groups[1].Value;
                    strCallID = mcCallStates[i].Groups[2].Value;
                    string strState = mcCallStates[i].Groups[3].Value;

                    AddCallID(strCallID);
                    strState = GetCallStateNum(strState);

                    // Put this in a list to be added to the WebLogLines later after sorting by date
                    AddCallDataEntry(strCallID, strDT, "Call State", strState);
                    
                }
            }
            if (mcPartIDs.Count > 0 || mcCallStates.Count > 0)
            {
                foreach (string strID in g_CallIDs)
                {
                    GetCallEnd(strID);
                }
            }

            g_CallData.Sort(new DateTimeComparer()); // After adding entries, sort by DateTime for output to UI

        } // GetCallData

        // Try to get Terminate reason and other end-of-call info
        public static void GetCallEnd(string strCallID)
        {
            string strDT = "";
            string strReason1 = "";
            string strTRReason0 = "";
            string strTRReason2 = "";
            string strCCCode = "";
            string strCCSubCode = "";
            string strReason2 = "";
            string strLine = "";

            string strRxEnd0 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*\[setCallState\]\ssuccess=changed\sto\s7,\sterminatedReason:\s(\d+)";
            string strRxEnd1 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*CallControlService:\sSending\sreportCallEnded.*{Regex.Escape(strCallID)}.*""reason"":(\d+)";
            string strRxEnd2 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*CallingTelemetryService: Finish start call scenarios.*callId={Regex.Escape(strCallID)}.*terminatedReason=(\d+).*callControllerCode=(\d+).*callControllerSubCode=(\d+).*reason=(\w+).*create_one_to_one_call";

            Match rxCallEnd0 = Regex.Match(g_allText, strRxEnd0);
            Match rxCallEnd1 = Regex.Match(g_allText, strRxEnd1);
            MatchCollection mcCallEnd2 = Regex.Matches(g_allText, strRxEnd2);

            if (rxCallEnd0.Success) // general case
            {
                strDT = rxCallEnd0.Groups[1].ToString();
                strTRReason0 = rxCallEnd0.Groups[2].ToString();
                strTRReason0 = GetTerminateReason(strTRReason0);

                // Put this in a list for displaying to the user
                AddCallDataEntry(strCallID, strDT, "Terminated Reason", strTRReason0);
            }

            if (rxCallEnd1.Success) // Also seems general... Not sure it's needed...
            {
                strDT = rxCallEnd1.Groups[1].ToString();
                strReason1 = rxCallEnd1.Groups[2].ToString();
                strReason1 = GetCallEndReason(strReason1);

                // Put this in a list for displaying to the user
                AddCallDataEntry(strCallID, strDT, "End Reason", strReason1);
            }

            if (mcCallEnd2.Count > 0) // More for P2P caller...
            {
                for (int i = mcCallEnd2.Count - 1; i >= 0; i--)
                {
                    strDT = mcCallEnd2[i].Groups[1].ToString();
                    strTRReason2 = mcCallEnd2[i].Groups[2].ToString();
                    strTRReason2 = GetTerminateReason(strTRReason2);
                    strCCCode = mcCallEnd2[i].Groups[3].ToString();
                    strCCSubCode = mcCallEnd2[i].Groups[4].ToString();

                    strLine = "Terminated Reason: " + strTRReason2 + ", CC Code: " + strCCCode + ", CC Subcode: " + strCCSubCode;

                    if (mcCallEnd2[i].Groups[5].ToString() != "undefined")
                    {
                        strReason2 = mcCallEnd2[i].Groups[5].ToString();
                        strLine = strLine + ", End Reason: " + strReason2;
                    }
                    AddCallDataEntry(strCallID, strDT, "Terminated Reason", strLine);
                }
            }

            g_CallData.Sort(new DateTimeComparer()); // After adding everything - sort it by DateTime for output to UI

        } // GetCallEnd

        // Try to prevent duplicate entries, and add entries that are unique
        public static void AddCallDataEntry(string strID, string strDT, string strProp, string strVal)
        {
            // Gotta do this to get the date time parsed successfully
            strDT = strDT.Replace('T', ' ');
            strDT = strDT.TrimEnd('Z');

            if (g_CallData.Count > 0)
            {
                foreach (g_CallDataEntry entry in g_CallData)
                {
                    if (entry.strID == strID)
                    {
                        if (entry.strProp == "ParticipantID" && entry.strVal == strVal)
                        {
                            return; // No need to log out the same ParticipantID for the CallID...
                        }

                        if (entry.strProp == strProp && entry.strVal == strVal)
                        {
                            return; // Try to avoid duplicate entries - which happens...
                        }
                    }
                }
            }
            // Now create and add the entry
            g_CallDataEntry callDataEntry = new g_CallDataEntry
            {
                strID = strID,
                datetime = DateTime.ParseExact(strDT, timeFmt, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
                strProp = strProp,
                strVal = strVal
            };
            g_CallData.Add(callDataEntry);
        }
    }
}
