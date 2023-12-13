using System;
using System.Globalization;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using static TATPlugin_Teams.Teams;

namespace TATPlugin_Teams
{
    public class Resource
    {
        // Time format for Date Times when they get brought in - to keep things easy to read and in the same format
        public static string timeFmt = "yyyy-MM-dd HH:mm:ss.fff";

        // Put info about this Parsing DLL at the top of the text in TAT
        public static string GetHeaderInfo()
        {
            string strSeparator = "================================================";
            string strDLLInfo = "Teams Plugin for TextAnalysisTool";
            string strSupport = "Currently will parse:"; // Logs.txt, Main Diagnostic log, Calling Diagnostic log, Experience Renderer log and MediaStack log"
            string strFiles = "Logs.txt" + "\n" + "MSTeams Diagnostics Log [Date-Time].txt" + "\n" + "MSTeams Diagnostics Log [Date-Time]_Calling.txt"
                               + "\n" + "MSTeams Diagnostics Log [Date-Time]_Experience_Renderer.txt";

            string strHeader = strSeparator + "\n" + strDLLInfo + "\n" + "\n" + strSupport + "\n" + strFiles + "\n" + strSeparator;

            return strHeader;
        }
        // Well - this is the "footer" - but for all the stuff that gets placed at the top of the text for the file in question
        public static string GetFooterText()
        {
            string strSeparator = "================================================";
            string strFooterInfo = "Original file contents below";

            string strFooter = strSeparator + "\n" + strFooterInfo + "\n" + strSeparator + "\r\n";

            return strFooter;
        }

        /*  For parsing out VDI mode
            | **`vdiMode`**: | 2                      | 0                 | 2                | 0                            |
            | -------------- | ---------------------- | ----------------- | ---------------- | ---------------------------- |
            | **Meaning**    | **Provider**           | **AV Optimized**  | **Desktop Kind** | **Persistence of user data** |
            |                | 1 = Citrix Xen Desktop | 0 = Not optimized | 0 = Unknown      | 0 = Unknown                  |
            |                | 2 = Citrix XenApp      | 1 = Optimized     | 1 = Dedicated    | 1 = Persistent               |
            |                | 3 = VMWare             |                   | 2 = Shared       | 2 = Non persistent           |
            |                | 5 = WVD                |                   |                  |                              |
        */
        public static string[,] rgVdiProvider = new string[5, 2]
        {
            {"0", "None" },
            {"1", "Citrix Xen Desktop" },
            {"2", "Citrix XenApp" },
            {"3", "VMWare" },
            {"5", "WVD" }
        };

        public static string[,] rgVdiAVOpt = new string[2, 2]
        {
            {"0", "AV not optimized" },
            {"1", "AV optimized" }
        };

        public static string[,] rgVdiDesktopKind = new string[3, 2]
        {
            {"0", "Unknown Kind (Dedicated or Shared...)" },
            {"1", "Dedicated Desktop" },
            {"2", "Shared Desktop" }
        };

        public static string[,] rgVdiPersisted = new string[3, 2]
        {
            {"0", "Persisted Unknown" },
            {"1", "Persistent" },
            {"2", "Non Persistent" }
        };

        // Get the full VDI info
        public static string GetVdiModeDecoded(string strMode)
        {
            string strDecoded = "";

            if (strMode == "0")
                strDecoded += "Not using VDI";
            else
            {
                char[] rgChars = strMode.ToCharArray();
                strDecoded += GetFromArray(rgChars[0].ToString(), rgVdiProvider) + ", ";
                strDecoded += GetFromArray(rgChars[1].ToString(), rgVdiAVOpt) + ", ";
                strDecoded += GetFromArray(rgChars[2].ToString(), rgVdiDesktopKind) + ", ";
                strDecoded += GetFromArray(rgChars[3].ToString(), rgVdiPersisted);
            }

            return strDecoded;
        }

        //  For getting VDI info in above function
        public static string GetFromArray(string strIn, string[,] strArray)
        {
            string strOut = "";
            int upperBound = strArray.GetUpperBound(0);

            strArray.GetUpperBound(1);

            for (int i = 0; i <= upperBound; i++)
            {
                if (strIn == strArray[i, 0])
                {
                    strOut = strArray[i, 1];
                    break;
                }
            }
            return strOut;
        }

        // OS version info
        public static string[,] rgOSVer = new string[75, 2]  // Will need updating as new OS builds release
        {
            {"10.15.0", "MacOS Catalina Released 10-7-2019" },
            {"10.15.1", "MacOS Catalina Released 10-29-2019" },
            {"10.15.2", "MacOS Catalina Released 12-10-2019" },
            {"10.15.3", "MacOS Catalina Released 1-28-2020" },
            {"10.15.4", "MacOS Catalina Released 3-24-2020" },
            {"10.15.5", "MacOS Catalina Released 5-26-2020" },
            {"10.15.6", "MacOS Catalina Released 7-15-2020" },
            {"10.15.7", "MacOS Catalina Released 9-24-2020" },
            {"10.16.0", "MacOS Big Sur Released 11-12-2020" },
            {"11.0", "MacOS Big Sur Released 11-17-2020" },
            {"11.0.1", "MacOS Big Sur Released 11-19-2020" },
            {"11.1", "MacOS Big Sur Released 12-14-2020" },
            {"11.2", "MacOS Big Sur Released 2-1-2021" },
            {"11.2.1", "MacOS Big Sur Released 2-9-2021" },
            {"11.2.2", "MacOS Big Sur Released 2-25-2021" },
            {"11.2.3", "MacOS Big Sur Released 3-8-2021" },
            {"11.3", "MacOS Big Sur Released 4-26-2021" },
            {"11.3.1", "MacOS Big Sur Released 5-3-2021" },
            {"11.4", "MacOS Big Sur Released 5-24-2021" },
            {"11.5", "MacOS Big Sur Released 7-21-2021" },
            {"11.5.1", "MacOS Big Sur Released 7-26-2021" },
            {"11.5.2", "MacOS Big Sur Released 8-11-2021" },
            {"11.6", "MacOS Big Sur Released 9-13-2021" },
            {"11.6.1", "MacOS Big Sur Released 10-25-2021" },
            {"11.6.2", "MacOS Big Sur Released 12-13-2021" },
            {"11.6.3", "MacOS Big Sur Released 1-26-2022" },
            {"11.6.4", "MacOS Big Sur Released 2-14-2022" },
            {"11.6.5", "MacOS Big Sur Released 3-14-2022" },
            {"11.7", "MacOS Big Sur Released 9-12-2022" },
            {"11.7.1", "MacOS Big Sur Released 10-24-2022" },
            {"11.7.2", "MacOS Big Sur Released 12-13-2022" },
            {"11.7.3", "MacOS Big Sur Released 1-23-2023" },
            {"11.7.4", "MacOS Big Sur Released 2-15-2023" },
            {"12.0", "MacOS Monterey Released 10-25-2021" },
            {"12.0.1", "MacOS Monterey Released 10-25-2021" },
            {"12.1.0", "MacOS Monterey Released 12-13-2021" },
            {"12.2.0", "MacOS Monterey Released 1-26-2022" },
            {"12.2.1", "MacOS Monterey Released 2-10-2022" },
            {"12.3.0", "MacOS Monterey Released 3-14-2022" },
            {"12.3.1", "MacOS Monterey Released 3-31-2022" },
            {"12.4.0", "MacOS Monterey Released 6-16-2022" },
            {"12.5.0", "MacOS Monterey Released 7-20-2022" },
            {"12.5.1", "MacOS Monterey Released 8-17-2022" },
            {"12.6.0", "MacOS Monterey Released 9-12-2022" },
            {"12.6.1", "MacOS Monterey Released 10-24-2022" },
            {"12.6.2", "MacOS Monterey Released 12-13-2022" },
            {"12.6.3", "MacOS Monterey Released 1-23-2023" },
            {"13.0.0", "MacOS Ventura Released 10-24-2022" },
            {"13.0.1", "MacOS Ventura Released 11-9-2022" },
            {"13.1.0", "MacOS Ventura Released 12-13-2022" },
            {"13.2.0", "MacOS Ventura Released 1-23-2023" },
            {"13.2.1", "MacOS Ventura Released 2-13-2023" },
            {"13.3", "MacOS Ventura Released 3-27-2023" },
            {"13.4", "MacOS Ventura Released 5-18-2023" },
            {"13.4.1", "MacOS Ventura Released 6-21-2023" },
            {"13.5", "MacOS Ventura Released 7-24-2023" },
            {"9200", "Windows 8 or Windows Server 2012" },
            {"9600", "Windows 8.1 or Windows Server 2012 R2" },
            {"10240", "Win10 TH1 Released 7-29-2015" },
            {"10586", "Win10 TH2 Released 11-10-2015" },
            {"14393", "Win10 RS1 Released 8-2-2016" },
            {"15063", "Win10 RS2 Released 4-5-2017" },
            {"16299", "Win10 RS3 Released 10-17-2017" },
            {"17134", "Win10 RS4 Released 4-30-2018" },
            {"17763", "Win10 RS5 Released 11-13-2018" },
            {"18362", "Win10 19H1 Released 5-21-2019" },
            {"18363", "Win10 19H2 Released 11-12-2019" },
            {"19041", "Win10 20H1 Released 5-27-2020" },
            {"19042", "Win10 20H2 Released 10-20-2020" },
            {"19043", "Win10 21H1 Released 5-18-2021" },
            {"19044", "Win10 21H2 Released 11-16-2021" },
            {"19045", "Win10 22H2 Released 10-18-2022" },
            {"22000", "Win11 21H2 Released 10-4-2021" },
            {"22621", "Win11 22H2 Released 9-20-2022" },
            {"22631", "Win11 23H2 Released 10-31-2022" }
        };

        // Get OS version
        public static string GetOSVerInfo(string strBuild) // To get the info above...
        {

            string strInfo = "";
            int upperBound = rgOSVer.GetUpperBound(0);
            rgOSVer.GetUpperBound(1);
            for (int i = 0; i <= upperBound; i++)
            {
                if (strBuild == rgOSVer[i, 0])
                {
                    strInfo = rgOSVer[i, 1];
                    break;
                }
            }
            return strInfo;
        }

        // Call state info
        public static string[,] rgCallState = new string[26, 2]
        {
            {"0", "None" },
            {"1", "Notified" },
            {"2", "Connecting" },
            {"3", "Connected" },
            {"4", "LocalHold" },
            {"5", "RemoteHold" },
            {"6", "Disconnecting" },
            {"7", "Disconnected" },
            {"8", "Observing" },
            {"9", "EarlyMedia" },
            {"10", "InLobby" },
            {"11", "Preheating" },
            {"12", "Preheated" },
            {"13", "Staging" },
            {"14", "Nego Encryption" },
            {"15", "Lobby Nego Encryption" },
            {"1000", "Reconnecting" },
            {"1001", "Transferring" },
            {"1002", "TransferFailed" },
            {"1003", "TransferSucceeded" },
            {"1004", "Escalating" },
            {"1005", "EscalationFailed" },
            {"1006", "EscalationSucceeded" },
            {"1007", "Parking" },
            {"1008", "ParkSucceeded" },
            {"1009", "ParkFailed" }
        };

        // get call state text that goes with number
        public static string GetCallState(string strInt)
        {
            string strCallState = strInt;
            int upperBound = rgCallState.GetUpperBound(0);
            rgCallState.GetUpperBound(1);
            for (int i = 0; i <= upperBound; i++)
            {
                if (strInt == rgCallState[i, 0])
                {
                    strCallState = " - " + rgCallState[i, 1];
                    break;
                }
            }

            return strCallState;
        }

        // get call state number that goes with text
        public static string GetCallStateNum(string strState)
        {
            string strCallStateNum = strState;
            int upperBound = rgCallState.GetUpperBound(0);
            rgCallState.GetUpperBound(1);
            for (int i = 0; i <= upperBound; i++)
            {
                if (strState == rgCallState[i, 1])
                {
                    strCallStateNum = rgCallState[i, 0];
                    break;
                }
            }

            strCallStateNum = strCallStateNum + " - " + strState;

            return strCallStateNum;
        }

        // Call termination reason info
        public static string[,] rgTerminateReasons = new string[70, 2]
        {
            {"0", "Undefined" },
            {"1", "Success" },
            {"2", "NoNgcEndpoint" },
            {"3", "NetworkError" },
            {"4", "MediaDroppedError" },
            {"5", "BadRequest" },
            {"6", "CallEstablishmentTimeout" },
            {"7", "CallSetupError" },
            {"8", "NoPermission" },
            {"9", "Missed" },
            {"10", "Declined" },
            {"11", "Busy" },
            {"12", "Cancelled" },
            {"13", "Dropped" },
            {"14", "PstnInsufficientFunds" },
            {"15", "PstnSkypeoutAccountBlocked" },
            {"16", "PstnCouldNotConnectToSkypeProxy" },
            {"17", "PstnBlockedByUs" },
            {"18", "PstnBlockedRegulatoryIndia" },
            {"19", "PstnInvalidNumber" },
            {"20", "PstnNumberForbidden" },
            {"21", "PstnCallTerminated" },
            {"22", "PstnNumberUnavailable" },
            {"23", "PstnEmergencyCallDenied" },
            {"24", "CallNotFound" },
            {"25", "LocalError" },
            {"26", "NotAcceptableHere" },
            {"27", "CallForwarded" },
            {"28", "CallForwardedToVoicemail" },
            {"29", "SkypeTokenError" },
            {"30", "CallAccepted" },
            {"31", "LocalHttpStackError" },
            {"32", "UnknownError" },
            {"33", "PstnNoSubscriptionCover" },
            {"34", "SessionNotFound" },
            {"35", "SessionTimedOut" },
            {"36", "PstnCreditExpired" },
            {"37", "PstnCreditExpiredButEnough" },
            {"38", "RetargetNotSupported" },
            {"39", "EnterprisePstnInternalError" },
            {"40", "EnterprisePstnUnavailable" },
            {"41", "EnterprisePstnForbidden" },
            {"42", "EnterprisePstnInvalidNumber" },
            {"43", "EnterprisePstnMiscError" },
            {"44", "Kicked" },
            {"45", "NetworkRequestTimeoutError" },
            {"46", "CallDoesNotExist" },
            {"47", "MediaSetupFailure" },
            {"48", "ServiceUnavailable" },
            {"49", "SignalingError" },
            {"50", "ConversationEstablishmentFailed" },
            {"51", "TemporarilyUnavailable" },
            {"52", "CannotConnectToNetworkError" },
            {"53", "MediaRelayWhiteListingIssue" },
            {"54", "NoSignalingFromPeer" },
            {"55", "DeniedInLobby" },
            {"56", "TimedOutInLobby" },
            {"57", "CallFailedConflict" },
            {"58", "DevicePermissionDenied" },
            {"59", "ConfParticipantCountLimitReached" },
            {"60", "ActionNotAllowed" },
            {"61", "Abandoned" },
            {"62", "ForbiddenDueToPolicy" },
            {"63", "InsufficientCapabilitiesForCallee" },
            {"64", "UserBlocked" },
            {"65", "AccessDenied" },
            {"66", "AnonymousJoinDisabledByPolicy" },
            {"67", "NoLobbyForBroadcastJoin" },
            {"68", "NotAllowedDueToInformationBarrier" },
            {"69", "BroadcastLimitReached" }
        };

        // get the call termination reason
        public static string GetTerminateReason(string strInt)
        {
            string strTermReason = strInt;
            int upperBound = rgTerminateReasons.GetUpperBound(0);
            rgTerminateReasons.GetUpperBound(1);
            for (int i = 0; i <= upperBound; i++)
            {
                if (strInt == rgTerminateReasons[i, 0])
                {
                    strTermReason = " - " + rgTerminateReasons[i, 1];
                    break;
                }
            }

            return strTermReason;
        }

        public static string[,] rgCallEndReason = new string[5, 2]
        {
            {"1", "Failed" },
            {"2", "RemoteEnded" },
            {"3", "Unanswered" },
            {"4", "AnsweredElsewhere" },
            {"5", "DeclinedElsewhere" }
        };

        // Get Call end reason from above enum
        public static string GetCallEndReason(string strInt)
        {
            string strCallEnd = strInt;
            int upperBound = rgCallEndReason.GetUpperBound(0);
            rgCallEndReason.GetUpperBound(1);
            for (int i = 0; i <= upperBound; i++)
            {
                if (strInt == rgCallEndReason[i, 0])
                {
                    strCallEnd += " - " + rgCallEndReason[i, 1];
                    break;
                }
            }

            return strCallEnd;
        }

        
        public static string[,] rgVideoStatus = new string[9, 2]
        {
            {"0", "NotAvailable" },
            {"1", "Available" },
            {"2", "Starting" },
            {"3", "Rejected" },
            {"4", "Running" },
            {"5", "Stopping" },
            {"6", "Paused" },
            {"7", "NotStarted" },
            {"9", "None" }
        };

        // Get status of sending video from above enum
        public static string GetVideoStatus(string strInt)
        {
            string strVidStatus = strInt;
            int upperBound = rgVideoStatus.GetUpperBound(0);
            rgVideoStatus.GetUpperBound(1);
            for (int i = 0; i <= upperBound; i++)
            {
                if (strInt == rgVideoStatus[i, 0])
                {
                    strVidStatus += " - " + rgVideoStatus[i, 1];
                    break;
                }
            }

            return strVidStatus;
        }

        public static string[,] rgRecvVidStatus = new string[5, 2]
        {
            {"0", "Stopped" },
            {"1", "Started" },
            {"2", "Active" },
            {"3", "Inactive" },
            {"5", "Terminated" }
        };

        // Get status of receiving video from above enum
        public static string GetRecvVideoStatus(string strInt)
        {
            string strRecvVidStatus = strInt;
            int upperBound = rgCallEndReason.GetUpperBound(0);
            rgCallEndReason.GetUpperBound(1);
            for (int i = 0; i <= upperBound; i++)
            {
                if (strInt == rgRecvVidStatus[i, 0])
                {
                    strRecvVidStatus += " - " + rgRecvVidStatus[i, 1];
                    break;
                }
            }

            return strRecvVidStatus;
        }

        // Teams Log Types
        public static string[,] rgFileType = new string[19, 2] 
        {
            {"logs.txt", "mainlog" },
            {"old_logs_", "mainlog" },
            {"SquirrelSetup-root", "squirrelroot" },
            {"SquirrelSetup-Teams", "squirrelteams" },
            {"teams-meeting-addin", "teamsaddin" },
            {"teams-meeting-addin-loader", "addinloader" },
            {"_calling", "callingdiag" },
            {"_cdl", "cdldiag" },
            {"_cdlWorker", "cdlworkerdiag" },
            {"_chatListData", "chatlistdiag" },
            {"_experience_renderer", "rendererdiag" },
            {"_extensibility", "extensibilitydiag" },
            {"_meeting_intelligence", "meetinteldiag" },
            {"_sync", "syncdiag" },
            {"debug-", "skylib" },
            {"Media.msrtc", "mediamsrtc" },  // disabled for now...
            {"MediaAgent.msrtc", "mediamsrtc" }, // disabled for now...
            {"Teams.msrtc", "teamsmsrtc" },
            {"MSTeams Diagnostics", "maindiag" } 
        };

        // Get the file type so we will know what data to parse or try to parse out of it
        public static string GetFileType(string strFile) 
        {
            string strFileType = "Invalid";
            // if we already parsed the file it will have this in the file name - so don't re-process it
            if (strFile.Contains("-PARSED"))
            {
                return strFileType;
            }

            int upperBound = rgFileType.GetUpperBound(0);
            rgFileType.GetUpperBound(1);
            for (int i = 0; i <= upperBound; i++)
            {
                if (strFile.Contains(rgFileType[i, 0]))
                {
                    strFileType = rgFileType[i, 1];
                    break;
                }
            }
            if (string.IsNullOrEmpty(strFileType))
            {
                strFileType = "Invalid";
            }

            return strFileType;
        }

        // For converting Unix date/time that is used in our logging > to a readable date/time in GMT
        public static string ConvertUnixTime(string strTime)
        {
            string strRet = "";
            long unixTime = long.Parse(strTime);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);
            dt = dt.AddMilliseconds(unixTime);
            strRet = dt.ToString();
            // strRet = DateTime.Parse(strRet).ToString();
            strRet = strRet + " GMT";

            return strRet;
        }

        // Convert Bytes per second to Megabits per second
        public static string Byteps2Mbps(int iBps)
        {
            int iMbps = iBps / 125000;
            string strMbps = iMbps.ToString();
            strMbps = strMbps + " Mbps";
            return strMbps;
        }

        // Convert Bits per second to Megabits per second
        public static string Bitsps2Mbps(int ibps)
        {
            int iMbps = ibps / 1000000;
            string strMbps = iMbps.ToString();
            strMbps = strMbps + " Mbps";
            return strMbps;
        }

        //Convert b-CT: Bandwidth output to Mbps
        public static string BW2Mbps(string strBW)
        {
            double iBW = Int32.Parse(strBW);
            double iMbps = iBW / 1000;
            string strMbps = iMbps.ToString("N2");
            strMbps = strBW + " == " + strMbps + " Mbps";
            return strMbps;
        }

        //Convert Bytes to Gigabytes
        public static string Bytes2GBs(double iBytes)
        {
            double iGB = Math.Round(iBytes / 1073741824, 2);
            string strGBs = iGB.ToString();
            strGBs = strGBs + " GB";
            return strGBs;
        }

        // add callid to global list if not there
        public static void AddCallID(string strCallID)
        {
            if (!g_CallIDs.Contains(strCallID))
            {
                if (!strCallID.Contains("no-call"))
                {
                    g_CallIDs.Add(strCallID);
                }
            }
        } // AddCallID

        public static DateTime ConvertDT(string strDT)
        {
            strDT = strDT.Replace('T', ' ');
            strDT = strDT.TrimEnd('Z');
            DateTime dtOut = DateTime.ParseExact(strDT, timeFmt, CultureInfo.InvariantCulture, DateTimeStyles.None);

            return dtOut;
        } // ConvertDT

        // < 0 dt1 earlier, 0 equal, > 0 dt1 later
        public static int CompareDateTimes(DateTime dt1, DateTime dt2)
        {
            int iResult = 0;
            iResult = DateTime.Compare(dt1, dt2);
            return iResult;
        } // CompareDateTimes

        // Decimal to Hex Converter
        public static string Dec2Hex(string strDec)
        {
            int iDec = Int32.Parse(strDec);
            return iDec.ToString("X");
        }
    }
}