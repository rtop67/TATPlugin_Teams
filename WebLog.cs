﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using static TATPlugin_Teams.GetInfo;
using static TATPlugin_Teams.Teams;
using static TATPlugin_Teams.Resource;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.Serialization;

namespace TATPlugin_Teams
{
    internal class WebLog
    {
        // All WebLog lines to be added to the text
        public static List<string> g_WebLogLines = new List<string>();

        //VideoIDs and StreamIDs List - they are coupled and sometimes only one is logged, and we might need the other
        public static List<Tuple<string, string>> g_VidStrIds = new List<Tuple<string, string>>();

        // Entries for below list
        public class g_CallDataEntry
        {
            public string strID {  get; set; }
            public DateTime datetime { get; set; }
            public string strProp { get; set; }
            public string strVal { get; set; }
        }

        // Call data entry list - stores all the Call Data lines that will be added to the output
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

        // Try to find CallID data with ParticipantID for this user, CallStates for each CallID, etc.
        public static void GetCallData()
        {
            string strCallID = "";
            string strRxPartIDs = @"(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z)\s+.*callId=([a-f\d]{8}(?:-[a-f\d]{4}){3}-[a-f\d]{12}).*threadId=([a-zA-Z0-9:_@.]+).*participantId=([a-f\d]{8}(?:-[a-f\d]{4}){3}-[a-f\d]{12})"; // Get CallID MeetingID and ParticipantID
            string strRxCallStates = @"(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z)\s+.*callId\s=\s(\S+).*state\s=\s(\w+),"; // gets date-time, callid, and state

            MatchCollection mcPartIDs = Regex.Matches(g_allText, strRxPartIDs);
            MatchCollection mcCallStates = Regex.Matches(g_allText, strRxCallStates);

            if (mcPartIDs.Count > 0)
            {
                for (int i = mcPartIDs.Count - 1; i >= 0; i--)  // gotta do reverse because weblog logs in reverse...
                {
                    string strDT = mcPartIDs[i].Groups[1].Value;
                    strCallID = mcPartIDs[i].Groups[2].Value;
                    string strMeetID = mcPartIDs[i].Groups[3].Value;
                    string strPartID = mcPartIDs[i].Groups[4].Value;

                    AddCallID(strCallID); // Add the CallID to the list of found CallIDs in the log

                    // Put this in a list to be added to the WebLogLines later after sorting by date
                    AddCallDataEntry(strCallID, strDT, "ParticipantID", strPartID);
                    if (strMeetID != "null")
                    {
                        if (!string.IsNullOrEmpty(strMeetID))
                        {
                            AddCallDataEntry(strCallID, strDT, "MeetingID", strMeetID);
                        }
                    }
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
                    GetCallIDChanged(strID);
                    GetOutVideoData(strID);
                    GetOutSS(strID);
                    GetInVideoData(strID);
                    GetCallEnd(strID);
                }
            }

            g_CallData.Sort(new DateTimeComparer()); // After adding entries, sort by DateTime for output to UI

        } // GetCallData

        // Sometimes the CallID changes - so log that.
        public static void GetCallIDChanged(string strCallID)
        {
            string strDT = "";
            string strNewID = "";

            string strRxCallChanged = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*_onCallIdChanged\s([a-f\d]{{8}}(?:-[a-f\d]{{4}}){{3}}-[a-f\d]{{12}})";

            MatchCollection mcChange = Regex.Matches(g_allText, strRxCallChanged);

            if (mcChange.Count > 0)
            {
                for (int i = mcChange.Count - 1; i >= 0; i--) // gotta do reverse...
                {
                    strDT = mcChange[i].Groups[1].ToString();
                    strNewID = mcChange[i].Groups[2].ToString();

                    if (strCallID != strNewID)
                        AddCallDataEntry(strCallID, strDT, "CallID Changed -", "From: " + strCallID + " To: " + strNewID);
                }
            }
        } // GetCallIDChanged
        
        // Get outgoing video feeds data
        public static void GetOutVideoData(string strCallID)
        {
            string strDT = "";
            string strDevice = "";
            string strVidname = "";
            string strStreaming = "";
            string strStatus = "";

            string strRxCam0 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*create local video ([\w-]+).*for device name: (.*?)(?=\s+path)";  // get video name and video device
            string strRxCam1 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*/VideoStream\[\d+:([\w-]+)\].*videoStatusChanged isStreaming: (\w+) status: (\d+)"; // video name, is streaming? and status
            string strRxCam2 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*/VideoStream\[\d+:([\w-]+)\].*state changed -> (\d)"; // streaming state changes
            string strRxCam3 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*/VideoStream\[\d+:([\w-]+)\].*stopVideo.*stop (.*?)(?=\scallVideoStop).*callVideoStop: (\w+)"; // stopping stream

            MatchCollection mcCam0 = Regex.Matches(g_allText, strRxCam0);
            MatchCollection mcCam1 = Regex.Matches(g_allText, strRxCam1);
            MatchCollection mcCam2 = Regex.Matches(g_allText, strRxCam2);
            MatchCollection mcCam3 = Regex.Matches(g_allText, strRxCam3);

            if (mcCam0.Count > 0)
            {
                for (int i = mcCam0.Count - 1; i >= 0; i--)  // gotta do reverse 
                {
                    strDT = mcCam0[i].Groups[1].ToString();
                    strVidname = mcCam0[i].Groups[2].ToString();
                    strDevice = mcCam0[i].Groups[3].ToString();

                    AddCallDataEntry(strCallID, strDT, "Outgoing Video -", "Opening Camera. Video Name: " + strVidname + ", Device: " + strDevice);
                }
            }

            if (mcCam1.Count > 0)
            {
                for (int i = mcCam1.Count - 1; i >= 0; i--)  // gotta do reverse 
                {
                    strDT = mcCam1[i].Groups[1].ToString();
                    strVidname = mcCam1[i].Groups[2].ToString();
                    strStreaming = mcCam1[i].Groups[3].ToString();
                    strStatus = mcCam1[i].Groups[4].ToString();

                    strStatus = GetVideoStatus(strStatus);

                    AddCallDataEntry(strCallID, strDT, "Outgoing Video -", "Video Name: " + strVidname + ", Streaming: " + strStreaming + ", Status: " + strStatus);
                }
            }

            if (mcCam2.Count > 0)
            {
                for (int i = mcCam2.Count - 1; i >= 0; i--)  // gotta do reverse 
                {
                    strDT = mcCam2[i].Groups[1].ToString();
                    strVidname = mcCam2[i].Groups[2].ToString();
                    strStatus = mcCam2[i].Groups[3].ToString();
                    
                    strStatus = GetVideoStatus(strStatus);

                    AddCallDataEntry(strCallID, strDT, "Outgoing Video -", "Video Name: " + strVidname + ", Status: " + strStatus);
                }
            }

            if (mcCam3.Count > 0)
            {
                for (int i = mcCam3.Count - 1; i >= 0; i--)  // gotta do reverse 
                {
                    strDT = mcCam3[i].Groups[1].ToString();
                    strVidname = mcCam3[i].Groups[2].ToString();
                    strStreaming = mcCam3[i].Groups[3].ToString();
                    strStatus = mcCam3[i].Groups[4].ToString();

                    AddCallDataEntry(strCallID, strDT, "Outgoing Video -", "Video Name: " + strVidname + ", Streaming: Stop " + strStreaming + ", Status: Call Video Stop = " + strStatus);
                }
            }
        } // GetOutVideoData

        
        // Get Outgoing screenshare feed data
        public static void GetOutSS(string strCallID)
        {
            string strDT = "";
            string strSS = "";
            string strWidth = "";
            string strHeight = "";
            string strRes = "";
            string strStatus = "";
            string strHwndD = "";
            string strHwndH = "";

            string strRxSS0 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*?\b(Start|Stop)ScreenSharing.*API called"; // Called to start or stop screenshare
            string strRxSS1 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*Selected region with optional crop.*width = (\d+) height = (\d+)"; // resolution
            string strRxSS2 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*?\b(Start|Stop)ScreenSharing.*success"; // Started or stopped screenshare
            string strRxSS3 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*videoObjectChanged status: (\d+)"; // get status
            string strRxSS4 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*screenshare region: undefined windowId: (\d+)"; // Get WindowID - this is an App being shared.
            // Not sure what happens when sharing a single app - if there is a good line to use to show that... So leaving this here as a note to add if there is something

            MatchCollection mcSS0 = Regex.Matches(g_allText, strRxSS0);
            MatchCollection mcSS1 = Regex.Matches(g_allText, strRxSS1);
            MatchCollection mcSS2 = Regex.Matches(g_allText, strRxSS2);
            MatchCollection mcSS3 = Regex.Matches(g_allText, strRxSS3);
            MatchCollection mcSS4 = Regex.Matches(g_allText, strRxSS4);


            if (mcSS0.Count > 0)
            {
                for (int i = mcSS0.Count - 1; i >= 0; i--)  // gotta do reverse 
                {
                    strDT = mcSS0[i].Groups[1].ToString();
                    strSS = mcSS0[i].Groups[2].ToString();

                    AddCallDataEntry(strCallID, strDT, "Outgoing VbSS -", "Called to " + strSS.ToLower() + " screensharing");
                }
            }

            if (mcSS1.Count > 0)
            {
                for (int i = mcSS1.Count - 1; i >= 0; i--)  // gotta do reverse 
                {
                    strDT = mcSS1[i].Groups[1].ToString();
                    strWidth = mcSS1[i].Groups[2].ToString();
                    strHeight = mcSS1[i].Groups[3].ToString();
                    strRes = strWidth + "x" + strHeight;

                    AddCallDataEntry(strCallID, strDT, "Outgoing VbSS -", "Sharing a full screen - resolution: " + strRes);
                }
            }

            if (mcSS2.Count > 0)
            {
                for (int i = mcSS2.Count - 1; i >= 0; i--)  // gotta do reverse 
                {
                    strDT = mcSS2[i].Groups[1].ToString();
                    strSS = mcSS2[i].Groups[2].ToString();

                    AddCallDataEntry(strCallID, strDT, "Outgoing VbSS -", "Call to " + strSS.ToLower() + " screensharing was successful");
                }
            }

            if (mcSS3.Count > 0)
            {
                for (int i = mcSS3.Count - 1; i >= 0; i--)  // gotta do reverse 
                {
                    strDT = mcSS3[i].Groups[1].ToString();
                    strStatus = mcSS3[i].Groups[2].ToString();

                    strStatus = GetVideoStatus(strStatus);

                    AddCallDataEntry(strCallID, strDT, "Outgoing VbSS -", "Screensharing status set to: " + strStatus);
                }
            }

            if (mcSS4.Count > 0)
            {
                for (int i = mcSS4.Count - 1; i >= 0; i--) // gotta do reverse
                {
                    strDT = mcSS4[i].Groups[1].ToString();
                    strHwndD = mcSS4[i].Groups[2].ToString();
                    strHwndH = Dec2Hex(strHwndD);

                    AddCallDataEntry(strCallID, strDT, "Outgoing VbSS -", "Sharing a specific app window, WindowID: " + strHwndD + "(dec), " + strHwndH + "(hex)");
                }
            }

        } // GetOutSS


        // Get incoming video feeds data
        public static void GetInVideoData(string strCallID)
        {
            string strDT = "";
            string strRemotePartID = "";
            string strRendering = "";
            string strVidname = "";
            string strStreamID = "";
            string strVideoID = "";
            string strOldStatus = "";
            string strNewStatus = "";
            string strWidth = "";
            string strHeight = "";
            string strRes = "";
            string strPinned = "";
            bool bIncomingVid = false;

            DateTime dtConn = GetDateTime(strCallID, "3 - Connected");
            DateTime dtDisconn = GetDateTime(strCallID, "6 - Disconnecting");

            g_VidStrIds.Clear();

            string strRxCam0 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*RemoteStream\(\d+\)\[(\d+):(\d+):\d+:\d+:\d+\].*participantId: ([a-f\d]{{8}}(?:-[a-f\d]{{4}}){{3}}-[a-f\d]{{12}}).*label: ([\w-]+)"; // StreamID, VideoId, Remote PartID, label
            string strRxCam1 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*Receiver\[\d+:\d+\]\[(\d+):(\d+):.*_subscribeReceiver"; // StreamID, VideoId & SubscribeRequest
            string strRxCam2 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*Receiver\[\d+:\d+\]\[(\d+):(\d+):.*subscribeStream.*done"; // StreamID, VideoId & Subscribe done
            string strRxCam3 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*Receiver\[\d+:\d+\]\[\d+:(\d+):#\d+\]\[(\d+):\d+\].*receiver status-changed event (\d) -> (\d)"; // Show video stream status
            string strRxCam4 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*compositorManager:.*FirstFrame isRendering: (\w+) invoked for streamId: (\d+).* frameWidth: (\d+).*frameHeight: (\d+)"; //rendering status, StreamID, Width and Height
            string strRxCam5 = $@"(\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{3}}Z)\s+.*callingAgents:\sslimcore-calling.*{Regex.Escape(strCallID)}.*Receiver.*subscribeStream.*new stream:\s\d+:(\d+).*\""isFocusEnabled\"":(\w+)"; // VideoId & Pinned/Spotlighted

            MatchCollection mcCam0 = Regex.Matches(g_allText, strRxCam0);
            MatchCollection mcCam1 = Regex.Matches(g_allText, strRxCam1);
            MatchCollection mcCam2 = Regex.Matches(g_allText, strRxCam2);
            MatchCollection mcCam3 = Regex.Matches(g_allText, strRxCam3);
            MatchCollection mcCam4 = Regex.Matches(g_allText, strRxCam4);
            MatchCollection mcCam5 = Regex.Matches(g_allText, strRxCam5);


            if (mcCam0.Count > 0)
            {
                for (int i = mcCam0.Count - 1; i >= 0; i--)  // gotta do reverse 
                {
                    strDT = mcCam0[i].Groups[1].ToString();
                    strStreamID = mcCam0[i].Groups[2].ToString();
                    strVideoID = mcCam0[i].Groups[3].ToString();
                    strRemotePartID = mcCam0[i].Groups[4].ToString();
                    strVidname = mcCam0[i].Groups[5].ToString();

                    if (strVideoID == "4294967294") // Clean this up. In the Media log videoID will be "-2" for a P2P call where video is shared
                        strVideoID = "-2";

                    if (!g_VidStrIds.Any(t => t.Item1 == strVideoID)) // Add StreamID and VideoID to a Tuple list - for retrieval later as needed
                    {
                        g_VidStrIds.Add(new Tuple<string, string>(strVideoID, strStreamID));
                    }

                    AddCallDataEntry(strCallID, strDT, "Incoming Video/VbSS -", "Video/VbSS Name: " + strVidname + ", VideoId: " + strVideoID + ", Remote ParticipantID: " + strRemotePartID);
                    bIncomingVid = true;
                }
            }

            if (mcCam1.Count > 0)
            {
                for (int i = mcCam1.Count - 1; i >= 0; i--)  // gotta do reverse 
                {
                    strDT = mcCam1[i].Groups[1].ToString();
                    strStreamID = mcCam1[i].Groups[2].ToString();
                    strVideoID = mcCam1[i].Groups[3].ToString();

                    if (strVideoID == "4294967294") // Clean this up. In the Media log videoID will be "-2" for a P2P call where video is shared
                        strVideoID = "-2";

                    if (!g_VidStrIds.Any(t => t.Item1 == strVideoID)) // Add StreamID and VideoID to a Tuple list - for retrieval later as needed
                    {
                        g_VidStrIds.Add(new Tuple<string, string>(strVideoID, strStreamID));
                    }

                    AddCallDataEntry(strCallID, strDT, "Incoming Video/VbSS -", "VideoId: " + strVideoID + " - Subscribe request sent.");
                    bIncomingVid = true;
                }
            }

            if (mcCam2.Count > 0)
            {
                for (int i = mcCam2.Count - 1; i >= 0; i--)  // gotta do reverse
                {
                    strDT = mcCam2[i].Groups[1].ToString();
                    strStreamID = mcCam2[i].Groups[2].ToString();
                    strVideoID = mcCam2[i].Groups[3].ToString();

                    if (strVideoID == "4294967294") // Clean this up. In the Media log videoID will be "-2" for a P2P call where video is shared
                        strVideoID = "-2";

                    if (!g_VidStrIds.Any(t => t.Item1 == strVideoID)) // Add StreamID and VideoID to a Tuple list - for retrieval later as needed
                    {
                        g_VidStrIds.Add(new Tuple<string, string>(strVideoID, strStreamID));
                    }

                    AddCallDataEntry(strCallID, strDT, "Incoming Video/VbSS -", "VideoId: " + strVideoID + " - Subscribe completed.");
                    bIncomingVid = true;
                }
            }

            if (mcCam3.Count > 0)
            {
                for (int i = mcCam3.Count - 1; i >= 0; i--)  // gotta do reverse
                {
                    strDT = mcCam3[i].Groups[1].ToString();
                    strVideoID = mcCam3[i].Groups[2].ToString();
                    strStreamID = mcCam3[i].Groups[3].ToString();
                    strOldStatus = mcCam3[i].Groups[4].ToString();
                    strNewStatus = mcCam3[i].Groups[5].ToString();

                    if (strVideoID == "4294967294") // Clean this up. In the Media log videoID will be "-2" for a P2P call where video is shared
                        strVideoID = "-2";

                    strOldStatus = GetRecvVideoStatus(strOldStatus);
                    strNewStatus = GetRecvVideoStatus(strNewStatus);

                    if (strVideoID == "0")
                    {
                        for (int n = 0; n < g_VidStrIds.Count; n++)  // Pull VideoId out of the Tuple list for the output
                        {
                            if (g_VidStrIds[n].Item2 == strStreamID)
                            {
                                strVideoID = g_VidStrIds[n].Item1;
                            }
                        }
                    }

                    AddCallDataEntry(strCallID, strDT, "Incoming Video/VbSS -", "VideoId: " + strVideoID + " - changed from " + strOldStatus + " to " + strNewStatus);
                }
            }

            if (bIncomingVid == true)  // This line of output has no CallID to correlate to, so have to use additional means to get the output right
            {
                if (mcCam4.Count > 0)
                {
                    for (int i = mcCam4.Count - 1; i >= 0; i--)  // gotta do reverse
                    {
                        strDT = mcCam4[i].Groups[1].ToString();
                        strRendering = mcCam4[i].Groups[2].ToString();
                        strStreamID = mcCam4[i].Groups[3].ToString();
                        strWidth = mcCam4[i].Groups[4].ToString();
                        strHeight = mcCam4[i].Groups[5].ToString();
                        strRes = strWidth + "x" + strHeight;
                        DateTime dtEntry = ConvertDT(strDT);

                        if (dtEntry < dtDisconn && dtEntry > dtConn)  // check to see if the time on these correlates to the time of the actual call here...
                        {

                            for (int n = 0; n < g_VidStrIds.Count; n++)  // Pull VideoId out of the Tuple list for the output
                            {
                                if (g_VidStrIds[n].Item2 == strStreamID)
                                {
                                    strVideoID = g_VidStrIds[n].Item1;
                                }
                            }

                            AddCallDataEntry(strCallID, strDT, "Incoming Video/VbSS -", "VideoId: " + strVideoID + " - Rendering: " + strRendering + ", Resolution: " + strRes);
                        }
                    }
                }
            }

            if (mcCam5.Count > 0)
            {
                for (int i = mcCam5.Count - 1; i >= 0; i--)  // gotta do reverse
                {
                    strDT = mcCam5[i].Groups[1].ToString();
                    strVideoID = mcCam5[i].Groups[2].ToString();
                    strPinned = mcCam5[i].Groups[3].ToString();

                    if (strVideoID == "4294967294") // Clean this up. In the Media log videoID will be "-2" for a P2P call where video is shared
                        strVideoID = "-2";

                    AddCallDataEntry(strCallID, strDT, "Incoming Video/VbSS -", "VideoId: " + strVideoID + " - Is focussed/pinned: " + strPinned);
                }
            }
        } // GetInVideoData

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

                    strLine = strTRReason2 + ", CC Code: " + strCCCode + ", CC Subcode: " + strCCSubCode;

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
            DateTime dtIn = DateTime.ParseExact(strDT, timeFmt, CultureInfo.InvariantCulture, DateTimeStyles.None);

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

                        if (entry.datetime == dtIn && entry.strProp == strProp && entry.strVal == strVal)
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
                datetime = dtIn,
                strProp = strProp,
                strVal = strVal
            };
            g_CallData.Add(callDataEntry);
        } // AddCallDataEntry

        // Get DateTime for certain CallDataEntries so we can determine if they are in the correct place.
        public static DateTime GetDateTime(string strCallID, string strState)
        {
            DateTime dtRet = DateTime.MinValue;

            foreach (g_CallDataEntry entry in g_CallData)
            {
                if (entry.strID == strCallID && entry.strVal == strState)
                {
                    dtRet = entry.datetime;
                }
            }

            return dtRet;
        } // GetDateTime
    }
}
