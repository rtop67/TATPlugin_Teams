using System;


namespace TATPlugin_Teams
{
    public class Resource
    {

        /*
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

        public static string[,] rgOSVer = new string[58, 2]  // Will need updating as new OS builds release
        {
            {"10.13.0", "MacOS High Sierra Released 9-25-2017" },
            {"10.14.0", "MacOS Mojave Released 9-24-2018" },
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
            {"22000", "Win11 21H2" }
        };

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

        public static string[,] rgCallState = new string[13, 2]
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
            {"12", "Preheated" }
        };

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

        public static string[,] rgFileType = new string[18, 2]  // Which Teams log is being loaded?
        {
            {"logs.txt", "mainlog" },
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
            {"Media.msrtc", "mediamsrtc" },
            {"MediaAgent.msrtc", "mediamsrtc" },
            {"Teams.msrtc", "teamsmsrtc" },
            {"MSTeams Diagnostics", "maindiag" }
        };

        public static string GetFileType(string strFile) // Function to get the above info so we know how to parse it.
        {
            string strFileType = "Invalid";
            // if we already parsed the file it will have this in the file name - so don't process it
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

        public static string ConvertUnixTime(string strTime)
        {
            double unixTime = Double.Parse(strTime);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddMilliseconds(unixTime);

            return dt.ToString();
        }

        public static string Byteps2Mbps(int iBps)
        {
            int iMbps = iBps / 125000;
            string strMbps = iMbps.ToString();
            strMbps = strMbps + " Mbps";
            return strMbps;
        }

        public static string Bitsps2Mbps(int ibps)
        {
            int iMbps = ibps / 1000000;
            string strMbps = iMbps.ToString();
            strMbps = strMbps + " Mbps";
            return strMbps;
        }

        public static string GetHeaderInfo()
        {
            string strSeparator = "================================================";
            string strDLLInfo = "Teams Plugin for TextAnalysisTool";
            string strSupport = "Currently works on:"; // Logs.txt, Main Diagnostic log, Calling Diagnostic log, Experience Renderer log and MediaStack log"
            string strFiles = "Logs.txt" + "\n" + "MSTeams Diagnostics Log [Date-Time].txt" + "\n" + "MSTeams Diagnostics Log [Date-Time]_Calling.txt" 
                               + "\n" + "MSTeams Diagnostics Log [Date-Time]_Experience_Renderer.txt" + "\n" + "Decoded Media.msrtc-xxxxx Log";
            
            string strHeader = strSeparator + "\n" + strDLLInfo + "\n" + "\n" + strSupport + "\n" + strFiles + "\n" + strSeparator;

            return strHeader;
        }

        public static string GetFooterText()
        {
            string strSeparator = "================================================";
            string strFooterInfo = "Original file contents below";

            string strFooter = strSeparator + "\n" + strFooterInfo + "\n" + strSeparator + "\r\n";

            return strFooter;
        }

    }
}