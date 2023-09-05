using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using static TATPlugin_Teams.Teams;
using static TATPlugin_Teams.Resource;
using static TATPlugin_Teams.LogsTxt;
using static TATPlugin_Teams.WebLog;

namespace TATPlugin_Teams
{
    // For getting base information about the user and the device in use, Teams version, etc etc.
    class GetInfo
    {
        public static int TextPos(string strSource, string strFind, int iPos, bool bReverse)
        {
            if (strSource.Contains(strFind))
            {
                if (bReverse == true)
                {
                    iPos = strSource.LastIndexOf(strFind) + strFind.Length;
                }
                else
                {
                    iPos = strSource.IndexOf(strFind, iPos) + strFind.Length;
                }
            }
            else
            {
                iPos = -1;
            }

            return iPos;
        }  // TextPos

        


        // Get first and last name of the user who generated this log
        public static string GetName()
        {
            string strFirstName = "";
            string strLastName = "";
            string strFullName = "";

            string rxLast = @"""family_name"":\s*""(\w+)""";
            string rxFirst = @"""given_name"":\s*""(\w+)""";

            Match rxFMatch = Regex.Match(g_allText, rxFirst);
            strFirstName = rxFMatch.Groups[1].Value;

            Match rxLMatch = Regex.Match(g_allText, rxLast);
            strLastName = rxLMatch.Groups[1].Value;

            if (!string.IsNullOrEmpty(strFirstName))
                strFullName = strFirstName + " ";
            if (!string.IsNullOrEmpty(strLastName))
                strFullName += strLastName;
            if (string.IsNullOrEmpty(strFullName))
            {
                strFullName = "Not found";
            }

            return strFullName;
        } // GetName

        // Get the username (user@domain) of this user
        public static string GetUserName()
        {
            string strName = "";
            string rxUserName = @"\""userName\""\s*:\s*\""([^\""]+)";

            Match rxMatch = Regex.Match(g_allText, rxUserName);
            strName = rxMatch.Groups[1].Value;

            if (string.IsNullOrEmpty(strName))
                strName = "Not found";

            return strName;
        } // GetUserName

        // Get the user's UPN
        public static string GetUpn()
        {
            string strUpn = "";
            string rxUpn = @"\""upn\""\s*:\s*\""([^\""]+)";

            Match rxMatch = Regex.Match(g_allText, rxUpn);
            strUpn = rxMatch.Groups[1].Value;

            if (string.IsNullOrEmpty(strUpn))
                strUpn = "Not found";

            return strUpn;
        }

        // Get the user's ObjectID
        public static string GetOID()
        {
            string strOID = "";
            string rxOID = @"\""oid\""\s*:\s*\""([a-fA-F\d]{8}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{12})\""";

            Match rxMatch = Regex.Match(g_allText, rxOID);
            strOID = rxMatch.Groups[1].Value;

            if (string.IsNullOrEmpty(strOID))
                strOID = "Not found";

            return strOID;
        } // GetOID

        // Get the user's TenantID
        public static string GetTenantID()
        {
            string strTenant = "";

            if (g_strFileType == "mainlog") // logs.txt
            {
                string strRX = @"(?<=Tenant Id set to\s)[a-fA-F0-9]{8}[-][a-fA-F0-9]{4}[-][a-fA-F0-9]{4}[-][a-fA-F0-9]{4}[-][a-fA-F0-9]{12}";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strTenant = rxMatch.Value;
            }
            else if (g_strFileType == "maindiag") // main web log
            {
                string strRX = @"\""tid\""\s*:\s*\""([a-fA-F\d]{8}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{12})\""";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strTenant = rxMatch.Groups[1].Value;
            }

            if (string.IsNullOrEmpty(strTenant))
                strTenant = "Not found";

            return strTenant;
        } // GetTenantID

        // Get Trouter connected state
        public static string GetTrouterConn()
        {
            string strConn = "";

            string strRX = @"\""connected\"":\s*(true|false)?,";
            Match rxMatch = Regex.Match(g_allText, strRX);
            strConn = rxMatch.Groups[1].Value;

            if (string.IsNullOrEmpty(strConn))
                strConn = "Not found";

            return strConn;
        }

        // Get Trouter URL
        public static string GetTrouterUrl()
        {
            string strUrl = "";

            string strRX = @"\""baseEndpointUrl\"":\s*\""([^\""]+)\""";
            Match rxMatch = Regex.Match(g_allText, strRX);
            strUrl = rxMatch.Groups[1].Value;

            if (string.IsNullOrEmpty(strUrl))
                strUrl = "Not found";

            return strUrl;
        }

        // Get the version of Teams in use
        public static string GetTeamsVer()
        {
            string strTeamsVer = "";

            if (g_strFileType == "mainlog") // logs.txt
            {
                string strRX = @"Starting app [\w\s]+, version (\d+\.\d+\.\d+\.\d+)";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strTeamsVer = rxMatch.Groups[1].Value;
            }
            else if (g_strFileType == "maindiag") // main web log
            {
                string strRX = @"\""appversion\""\s*:\s*\""(\d+\.\d+\.\d+\.(\d+))\""";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strTeamsVer = rxMatch.Groups[1].Value;
            }

            if (string.IsNullOrEmpty(strTeamsVer))
                strTeamsVer = "Not found";

            return strTeamsVer;
        } //GetTeamsVer

        // Get the slimcore version
        public static string GetSlimcoreVer()
        {
            string strSCVer = "";

            if (g_strFileType == "mainlog") // logs.txt
            {
                string strRX = @"slimcore version:\s*(\d+\.\d+\.\d+)";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strSCVer = rxMatch.Groups[1].Value;
            }
            else if (g_strFileType == "maindiag") // main web log
            {
                string strRX = @"slimcoreVersion.*(\d+\.\d+\.\d+)\""";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strSCVer = rxMatch.Groups[1].Value;
            }

            if (string.IsNullOrEmpty(strSCVer))
                strSCVer = "Not found";

            return strSCVer;
        } // GetSlimcoreVer

        // Get the build date 
        public static string GetBuildDate()
        {
            string strBuildDate = "";

            string strRX = @"\""buildDate\"": \""(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z)\""";
            Match rxMatch = Regex.Match(g_allText, strRX);
            strBuildDate = rxMatch.Groups[1].Value;

            if (!string.IsNullOrEmpty(strBuildDate))
            {
                strBuildDate = strBuildDate.Replace('T', ' ');
            }
            else
                strBuildDate = "Not found";

            return strBuildDate;
        }// GetBuildDate

        // Get the Teams Ring in use
        public static string GetRing()
        {
            string strRing = "";

            if (g_strFileType == "mainlog") // logs.txt
            {
                string strRX = @"\?ring=(\w+)";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strRing = rxMatch.Groups[1].Value;
            }
            else if (g_strFileType == "maindiag") // main web log
            {
                string strRX = @"""UserInfo\.Ring"": ""(\w+)"",";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strRing = rxMatch.Groups[1].Value;
            }

            if (string.IsNullOrEmpty(strRing))
                strRing = "Not found";

            return strRing;
        } // GetRing

        public static string GetClientType()
        {
            string strClientType = "";

            if (g_strFileType == "mainlog")
            {
                string strRX = @"AppInfo\.ClientType:\s*(\w+)";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strClientType = rxMatch.Groups[1].Value;
            }
            else if (g_strFileType == "maindiag")
            {
                string strRX = @"""AppInfo\.ClientType""\s*:\s*""(\w+)""";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strClientType = rxMatch.Groups[1].Value;
            }

            if (string.IsNullOrEmpty(strClientType))
                strClientType = "Not found";

            return strClientType;
        } // GetClientType

        public static string GetVdiMode()
        {
            string strVdiNum = "";
            string strVdiModeText;
            string strVDI = "";

            if (g_strFileType == "mainlog") // logs.txt
            {
                string strRX = @"vdiMode:\s*(\d+)";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strVdiNum = rxMatch.Groups[1].Value;

            }
            else if (g_strFileType == "maindiag") // main web log
            {
                string strRX = @"""vdiMode"":\s*""(\d+)""";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strVdiNum = rxMatch.Groups[1].Value;
            }

            if (!string.IsNullOrEmpty(strVdiNum))
            {
                strVdiModeText = GetVdiModeDecoded(strVdiNum);
                strVDI = strVdiNum + ": " + strVdiModeText;
            }
            else
                strVDI = "Not found";

            return strVDI;
        } // GetVdiMode

        public static string GetOSVer()
        {
            string strOSVer = "";
            string strOSBuild = "";
            string strOSInfo;
            string strOS = "";

            if (g_strFileType == "mainlog") // logs.txt
            {
                string strRX = @"osversion\s+(\d+\.\d+(\.\d+)*)";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strOSVer = rxMatch.Groups[1].Value;
            }
            else if (g_strFileType == "maindiag") // main web log
            {
                string strRX = @"""osversion"":\s*""(\d+\.\d+(\.\d+)*)""";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strOSVer = rxMatch.Groups[1].Value;
            }

            if (!string.IsNullOrEmpty(strOSVer))
            {
                strOSBuild = GetOSBuild(strOSVer);
                strOSInfo = GetOSVerInfo(strOSBuild);

                strOS = strOSVer + " - " + strOSInfo;
            }
            else
                strOS = "Not found";

            return strOS;
        } // GetOSVer

        // Get the OS build number so we can look it up and get something more useful
        private static string GetOSBuild(string strOSVer)
        {
            string strOSBuild = "";

            if (strOSVer.StartsWith("10.1") || strOSVer.StartsWith("11.") || strOSVer.StartsWith("12.") || strOSVer.StartsWith("13."))
            {
                strOSBuild = strOSVer;
            }
            else if (strOSVer.StartsWith("6.2.") || strOSVer.StartsWith("6.3."))
            {
                strOSBuild = strOSVer.Substring(4);
            }
            /*else if (strOSVer.Contains("22000"))
            {
                strOSBuild = "22000"; 
            }*/
            else
            {
                strOSBuild = strOSVer.Substring(5);
                if (strOSBuild.Contains("."))
                {
                    int iDot = strOSBuild.IndexOf('.');
                    strOSBuild = strOSBuild.Substring(0, iDot);
                    strOSBuild = strOSBuild.Trim('.');
                }
            }

            return strOSBuild;
        } // GetOSBuild

        // Get the timezone for this user location
        public static string GetTimezone()
        {
            string strTZ = "";

            if (g_strFileType == "mainlog") // logs.txt
            {
                string strRX = @"UserInfo\.TimeZone:\s*([+-]\d{2}:\d{2})";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strTZ = rxMatch.Groups[1].Value;
            }
            else if (g_strFileType == "maindiag") // main web log
            {
                string strRX = @"""timezone"":\s*(-?\d+)";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strTZ = rxMatch.Groups[1].Value;
            }

            if (!string.IsNullOrEmpty(strTZ))
            {
                if (strTZ.Length < 4)
                {
                    if (strTZ.Contains("-"))
                    {
                        strTZ = "GMT " + strTZ + ":00";
                    }
                    else
                    {
                        strTZ = "GMT +" + strTZ + ":00";
                    }
                }
                else
                {
                    if (strTZ.Contains("-"))
                    {
                        strTZ = "GMT " + strTZ;
                    }
                    else
                    {
                        strTZ = "GMT " + strTZ;
                    }
                }
            }
            else
            {
                strTZ = "Not found";
            }

            return strTZ;
        } // GetTimezone

        // Get local IP Address for this user's machine
        public static string GetIPAddr()
        {
            string strIPAddr = "";

            if (g_strFileType == "mainlog") // logs.txt
            {
                string strRX = @"""ipaddr"":""(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})""";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strIPAddr = rxMatch.Groups[1].Value;
            }

            if (g_strFileType == "maindiag") // main web log
            {
                string strRX = @"""ipaddr"":\s*""(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})""";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strIPAddr = rxMatch.Groups[1].Value;
            }

            if (string.IsNullOrEmpty(strIPAddr))
                strIPAddr = "Not found";

            return strIPAddr;
        } // GetIPAddr

        // Get the machine's hostname / machine name
        public static string GetHostname()
        {
            string strHost = "";

            if (g_strFileType == "maindiag")
            {
                string strRX = @"""hostname"":\s*""([^""]+)""";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strHost = rxMatch.Groups[1].Value;
            }

            if (string.IsNullOrEmpty(strHost))
                strHost = "Not found";

            return strHost;
        } // GetHostname

        // Get info about the device/machine
        public static string GetDeviceInfo()
        {
            string strOEM = "";
            string strSystem = "";
            string strSysInfo = "";

            if (g_strFileType == "maindiag")
            {
                string strRX = @"""DeviceInfo\.SystemManufacturer"":\s*""([^""]+)""";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strOEM = rxMatch.Groups[1].Value;

                strRX = @"""DeviceInfo\.SystemProductName"":\s*""([^""]+)""";
                rxMatch = Regex.Match(g_allText, strRX);
                strSystem = rxMatch.Groups[1].Value;
            }

            if (!string.IsNullOrEmpty(strOEM))
            {
                if (!string.IsNullOrEmpty(strSystem))
                {
                    strSysInfo = strOEM + ", " + strSystem;
                }
                else
                {
                    strSysInfo = strOEM + " - System device info was not found.";
                }
            }
            else
                strSysInfo = "Not found";

            return strSysInfo;
        }

        public static string GetCPU()
        {
            string strCPU = "";
            string strCores = "";
            string strCPUInfo = "";

            if (g_strFileType == "maindiag")
            {
                string strRX = @"""cpumodel"":\s*""([^""]+)""";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strCPU = rxMatch.Groups[1].Value;

                strRX = @"""cores"":\s*(\d+)";
                rxMatch = Regex.Match(g_allText, strRX);
                strCores = rxMatch.Groups[1].Value;
            }

            if (!string.IsNullOrEmpty(strCPU))
            {
                if (!string.IsNullOrEmpty(strCores))
                {
                    strCPUInfo = strCPU + ", " + strCores + " cores";
                }
                else
                {
                    strCPUInfo = strCPU;
                }
            }
            else
                strCPUInfo = "Not found";

            return strCPUInfo;
        } // GetCPU

        // Get Memory installed in the machine
        public static string GetMem()
        {
            string strMem = "";
            string strConvMem = "";

            if (g_strFileType == "maindiag")
            {
                string strRX = @"""totalMemory"":\s*(\d+)";
                Match rxMatch = Regex.Match(g_allText, strRX);
                strMem = rxMatch.Groups[1].Value;

                if (!string.IsNullOrEmpty(strMem))
                {
                    double iMem = double.Parse(strMem);
                    if (iMem > 0)
                    {
                        strConvMem = Bytes2GBs(iMem);
                        strConvMem = strMem + ": " + strConvMem;
                    }
                }
                else
                    strConvMem = "Not found";
            }

            return strConvMem;
        }

        public static void GetCallIDs()
        {
            int iSearch = 0;
            int iFirstPos = 0;
            //int iRet = 0;
            string strCallID = "";

            if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "callId =", 0, false);
                while (iSearch != -1)
                {
                    iFirstPos = iSearch;
                    iSearch = iSearch + 1;
                    strCallID = g_allText.Substring(iSearch, 36);
                    if (!g_CallIDs.Contains(strCallID))
                    {
                        if (!strCallID.Contains("no-call"))
                        {
                            g_CallIDs.Add(strCallID);
                        }
                    }
                    iSearch = TextPos(g_allText, "callId =", iSearch + 1, false);
                    if (iSearch <= iFirstPos)
                        break;
                }
            }

            if (g_strFileType == "callingdiag")
            {
                iSearch = TextPos(g_allText, "callId:", 0, false);
                while (iSearch != -1)
                {
                    iFirstPos = iSearch;
                    iSearch = iSearch + 1;
                    strCallID = g_allText.Substring(iSearch, 36);

                    if (strCallID.Contains(":"))
                    {
                        iSearch = TextPos(g_allText, "callId:", iSearch + 1, false);
                        if (iSearch <= iFirstPos)
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (!g_CallIDs.Contains(strCallID))
                    {
                        g_CallIDs.Add(strCallID);
                    }

                    iSearch = TextPos(g_allText, "callId:", iSearch + 1, false);
                    if (iSearch <= iFirstPos)
                        break;
                }

                iSearch = TextPos(g_allText, "\"callId\":", 0, false);
                while (iSearch != -1)
                {
                    iFirstPos = iSearch;
                    iSearch = iSearch + 2;
                    strCallID = g_allText.Substring(iSearch, 36);

                    if (strCallID.EndsWith("\""))
                    {
                        iSearch = iSearch - 1;
                        strCallID = g_allText.Substring(iSearch, 36);
                    }

                    if (!g_CallIDs.Contains(strCallID))
                    {
                        g_CallIDs.Add(strCallID);
                    }
                    iSearch = TextPos(g_allText, "\"callId\":", iSearch + 1, false);
                    if (iSearch <= iFirstPos)
                        break;
                }
            }

                if (g_strFileType == "rendererdiag")
            {
                iSearch = TextPos(g_allText, "\"callId\":", 0, false);
                while (iSearch != -1)
                {
                    iFirstPos = iSearch;
                    iSearch = iSearch + 2;
                    strCallID = g_allText.Substring(iSearch, 36);
                    if (!g_CallIDs.Contains(strCallID))
                    {
                        g_CallIDs.Add(strCallID);
                    }
                    iSearch = TextPos(g_allText, "\"callId\":", iSearch + 1, false);
                    if (iSearch <= iFirstPos)
                        break;
                }
            }

            if (g_strFileType == "mediamsrtc")
            {
                iSearch = TextPos(g_allText, "enter, CallId:", 0, false);
                while (iSearch != -1)
                {
                    iFirstPos = iSearch;
                    iSearch = iSearch + 1;
                    strCallID = g_allText.Substring(iSearch, 36);
                    
                    if (strCallID.Contains(",")) // sometimes it's blank and the first character would be a comma - or close to it anyway
                    {
                        iSearch = TextPos(g_allText, "enter, CallId:", iSearch + 1, false);
                        if (iSearch <= iFirstPos)
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                            
                    }
                    string strDateTime = GetDateTime(iSearch);
                    string strCallIDDT = strCallID + " - " + strDateTime;
                    bool bFound = false;

                    foreach (string strItem in g_CallIDs)
                    {
                        if (strItem.Contains(strCallID))
                        {
                            bFound = true;
                        }
                    }

                    if (bFound == false)
                    {
                        g_CallIDs.Add(strCallIDDT);
                    }

                    iSearch = TextPos(g_allText, "enter, CallId:", iSearch + 1, false);
                    if (iSearch <= iFirstPos)
                        break;
                }
            }
        } // GetCallIDs

        public static void GetCrashLines()
        {
            if (g_strFileType == "mainlog")
            {
                foreach (string strLine in g_allLines)
                {
                    if (strLine.Contains("-- error --") && strLine.Contains("crash"))
                    {
                        if (strLine.Contains("extraparameter to crashReporter:"))
                            break;
                        if (!g_CrashLines.Contains(strLine))
                        {
                            g_CrashLines.Add(strLine);
                        }
                    }
                }
            }

            if (g_strFileType == "maindiag" || g_strFileType == "rendererdiag")
            {
                foreach (string strLine in g_allLines)
                {
                    if (strLine.Contains("Z Err") && strLine.Contains("crash"))
                    {
                        if (!g_CrashLines.Contains(strLine))
                        {
                            g_CrashLines.Add(strLine);
                        }
                    }
                }
            }
        } // GetCrashLines

        public static string GetLogDateTimes()
        {
            string strReturn = "";
            string strStart = "";
            string strEnd = "";
            string strRxLogsTxt = @"(\w{3}\s\d+\s\d{4}\s\d{2}:\d{2}:\d{2})\s"; // Mar 14 2023 14:00:58
            string strRxRest = @"(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z)"; // 2023-03-14T18:01:41.146Z
            string strRxMedia = @"(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\.\d{3})\s"; // 2023-01-20 14:59:38.163 (BRB/Logjoint decoded)
            string strDTNow = GetCurrentDT();

            if (g_strFileType == "mainlog")
            {
                foreach (string strLine in g_allLines)
                {
                    if (strLine.Contains("GMT"))
                    {
                        Match rxDT = Regex.Match(strLine, strRxLogsTxt);
                        if (rxDT.Success)
                        {
                            strStart = rxDT.Groups[1].Value;
                            strStart = ConvertDateTime(strStart);
                        }
                        break;
                    }
                }

                foreach (string strLine in g_allLines.Reverse<string>())
                {
                    if (strLine.Contains("GMT"))
                    {
                        Match rxDT = Regex.Match(strLine, strRxLogsTxt);
                        if (rxDT.Success)
                        {
                            strEnd = rxDT.Groups[1].Value;
                            strEnd = ConvertDateTime(strEnd);
                        }
                        break;
                    }
                }
            }
            else if (g_strFileType == "maindiag" || g_strFileType == "rendererdiag")
            {
                foreach (string strLine in g_allLines.Reverse<string>())
                {
                    if (strLine.Contains("Z Inf") || strLine.Contains("Z War") || strLine.Contains("Z Err"))
                    {
                        Match rxDT = Regex.Match(strLine, strRxRest);
                        if (rxDT.Success)
                        {
                            strStart = rxDT.Groups[1].Value;
                            strStart = ConvertDateTime(strStart);
                        }
                        break;
                    }
                }

                foreach (string strLine in g_allLines)
                {
                    if (strLine.Contains("Z Inf") || strLine.Contains("Z War") || strLine.Contains("Z Err"))
                    {
                        Match rxDT = Regex.Match(strLine, strRxRest);
                        if (rxDT.Success)
                        {
                            strEnd = rxDT.Groups[1].Value;
                            strEnd = ConvertDateTime(strEnd);
                        }
                        break;
                    }
                }
            }

            else if (g_strFileType == "mediamsrtc")
            {
                foreach (string strLine in g_allLines)
                {
                    // For BRB decoded file
                    if (strLine.Contains("[#") && strLine.Contains("-S]"))
                    {
                        Match rxDT = Regex.Match(strLine, strRxMedia);
                        if (rxDT.Success)
                        {
                            strStart = rxDT.Groups[1].Value;
                            strStart = ConvertDateTime(strStart);
                        }
                        break;
                    }
                    // For Dev TAT decoded file
                    else if (strLine.Contains("TL_INFO") || strLine.Contains("TL_WARN") || strLine.Contains("TL_ERROR") || strLine.Contains("TL_FATAL"))
                    {
                        Match rxDT = Regex.Match(strLine, strRxRest);
                        if (rxDT.Success)
                        {
                            strStart = rxDT.Groups[1].Value;
                            strStart = ConvertDateTime(strStart);
                        }
                        break;
                    }
                }

                foreach (string strLine in g_allLines.Reverse<string>())
                {
                    // For BRB decoded file
                    if (strLine.Contains("[#") && strLine.Contains("-S]"))
                    {
                        Match rxDT = Regex.Match(strLine, strRxMedia);
                        if (rxDT.Success)
                        {
                            strEnd = rxDT.Groups[1].Value;
                            strEnd = ConvertDateTime(strEnd);
                        }
                        break;
                    }
                    // For Dev TAT decoded file
                    else if (strLine.Contains("TL_INFO") || strLine.Contains("TL_WARN") || strLine.Contains("TL_ERROR") || strLine.Contains("TL_FATAL"))
                    {
                        Match rxDT = Regex.Match(strLine, strRxRest);
                        if (rxDT.Success)
                        {
                            strEnd = rxDT.Groups[1].Value;
                            strEnd = ConvertDateTime(strEnd);
                        }
                        break;
                    }
                }
            }

            //TODO: Compare "Now" to Ending Date-Time and flag if over 20 days.

            strReturn = strStart + " to " + strEnd;

            return strReturn;

        } // GetLogDateTimes

        // Get Current date-time
        public static string GetCurrentDT()
        {
            string strRet = "";
            DateTime dt = DateTime.Now.ToUniversalTime();
            strRet = dt.ToString();
            return strRet;
        }

        // Make all DateTime values the same so we can see where things line up etc.
        public static string ConvertDateTime(string strDT)
        {
            string strRet = "";

            if (g_strFileType == "mainlog")
            {
                string strConvDT = DateTime.Parse(strDT).ToString();
                strConvDT = strConvDT + " User Local Time";
                strRet = strConvDT;
            }
            else
            {
                strDT = strDT.Replace('T', ' ');
                strDT = strDT.TrimEnd('Z');
                string strConvDT = DateTime.Parse(strDT).ToString();
                strConvDT = strConvDT + " GMT";
                strRet = strConvDT;
            }

            return strRet;
        }


        // For getting the Date-Time for a CallID. Hacky.
        public static string GetDateTime(int iPos)
        {
            iPos = iPos - 180;
            int iStart = TextPos(g_allText, "202", iPos, false);
            iStart = iStart - 2;
            int iEnd = TextPos(g_allText, "[", iStart, false);
            int iLength = iEnd - iStart;
            iStart--;

            string strDateTime = g_allText.Substring(iStart, iLength);

            // TAT decoded file behaves differently than BRB decoded - so must do the below.
            if (strDateTime.Contains("MEDIAMGR_API"))
            {
                int iSt = 0;
                int iEn = TextPos(strDateTime, "  ", 12, false);
                int iLen = (iEn - iSt);

                strDateTime = strDateTime.Substring(iSt, iLen);
            }


            return strDateTime;
        }
    }
}
