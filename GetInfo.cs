using System;
using System.Linq;
using static TATPlugin_Teams.Teams;
using static TATPlugin_Teams.Resource;

namespace TATPlugin_Teams
{
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

        public static string GetName()
        {
            int iSearch = 0;
            string strName = "";
            int iQuote = 0;
            int iPosLast = 0;
            string strReturn = "Not found";

            if (g_strFileType == "mainlog")
            {
                do
                {
                    iSearch = TextPos(g_allText, "\"name\":", iSearch, false);
                    if (iSearch > iPosLast)
                    {
                        strName = g_allText.Substring(iSearch + 1, 50);
                    }
                    else
                    {
                        return strReturn;
                    }
                    iPosLast = iSearch;
                } while (strName.Contains("slimcore") || strName.Contains("media-hid") || strName.Contains("V8 Proxy") || strName.Contains("Audio Service") || strName.Contains("Video Capture") || strName.Contains("Network Service") || strName.Contains("Error"));

                strName = strName.Trim();
                strName = strName.TrimStart('"');
                iQuote = strName.IndexOf('"');
                strName = strName.Substring(0, iQuote);

                if (strName.Contains("%20"))
                {
                    strName = strName.Replace("%20", " ");
                }

                strReturn = strName;
            }
            else if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "\"name\":", 0, false);
                if (iSearch != -1)
                {
                    strName = g_allText.Substring(iSearch + 2, 50);
                    iQuote = strName.IndexOf('"');
                    strName = strName.Substring(0, iQuote);
                    strReturn = strName;
                }
            }

            return strReturn;
        } // GetName

        public static string GetUserName()
        {
            string strReturn = "Not found";
            int iSearch = 0;
            string strName = "";
            int iQuote = 0;

            if (g_strFileType == "mainlog")
            {
                iSearch = TextPos(g_allText, "\"userName\":", 0, false);
                if (iSearch != -1)
                {
                    strName = g_allText.Substring(iSearch + 1, 60);
                    strName = strName.TrimStart('"');
                    iQuote = strName.IndexOf('"');
                    strName = strName.Substring(0, iQuote);
                    strReturn = strName;
                }
            }
            else if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "\"userName\":", 0, false);
                if (iSearch != -1)
                {
                    strName = g_allText.Substring(iSearch + 2, 60);
                    strName = strName.TrimStart('"');
                    iQuote = strName.IndexOf('"');
                    strName = strName.Substring(0, iQuote);
                    strReturn = strName;
                }
            }

            return strReturn;
        } // GetUserName

        public static string GetOID()
        {
            string strReturn = "Not found";
            int iSearch = 0;

            if (g_strFileType == "mainlog")
            {
                iSearch = TextPos(g_allText, "\"oid\":", 0, false);
                if (iSearch != -1)
                {
                    string strOID = g_allText.Substring(iSearch + 1, 37);
                    strOID = strOID.TrimStart('"');
                    int iQuote = strOID.IndexOf('"');
                    if (iQuote != -1)
                    {
                        strOID = strOID.Substring(0, iQuote);
                    }
                    strReturn = strOID;
                }
            }
            else if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "\"oid\":", 0, false);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch + 2, 36);
                }
            }

            return strReturn;
        } // GetOID

        public static string GetTenantID()
        {
            string strReturn = "Not found";
            int iSearch = 0;

            if (g_strFileType == "mainlog")
            {
                iSearch = TextPos(g_allText, "Tenant Id set to", 0, true);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch + 1, 36);
                }
            }
            else if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "UserInfo.TenantId", 0, false);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch + 4, 36);
                }
            }

            return strReturn;
        } // GetTenantID

        public static string GetTeamsVer()
        {
            int iSearch = 0;
            string strReturn = "Not found";

            if (g_strFileType == "mainlog")
            {
                iSearch = TextPos(g_allText, "Starting app Teams, version", 0, true);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch + 1, 12);
                }
            }
            else if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "appversion", 0, false);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch + 4, 12);
                }
            }

            strReturn = strReturn.Trim(',', '"');

            return strReturn;
        } //GetTeamsVer

        public static string GetSlimcoreVer()
        {
            string strReturn = "Not found";
            int iSearch = 0;

            if (g_strFileType == "mainlog")
            {
                iSearch = TextPos(g_allText, "built-in slimcore version:", 0, true);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch + 1, 10);
                }
            }
            else if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "slimcoreVersion\":", 0, false);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch + 2, 10);
                }
            }

            strReturn = strReturn.Trim(',', '"');

            return strReturn;
        } // GetSlimcoreVer

        public static string GetRing()
        {
            string strReturn = "Not found";
            int iSearch = 0;

            if (g_strFileType == "mainlog")
            {
                iSearch = TextPos(g_allText, "ring is initialized", 0, true);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch + 4, 8);
                    strReturn = strReturn.Trim();
                }
            }
            else if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "UserInfo.Ring", 0, false);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch + 4, 8);
                    strReturn = strReturn.Trim();
                    strReturn = strReturn.TrimEnd(',', '"');
                }
            }

            return strReturn;
        } // GetRing

        public static string GetClientType()
        {
            string strReturn = "Not found";
            int iSearch = 0;

            if (g_strFileType == "mainlog")
            {
                iSearch = TextPos(g_allText, "AppInfo.ClientType", 0, true);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch + 2, 8);
                    strReturn = strReturn.Trim();
                    strReturn = strReturn.TrimEnd(',');
                }
            }
            else if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "AppInfo.ClientType", 0, false);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch + 4, 8);
                    strReturn = strReturn.Trim();
                    strReturn = strReturn.TrimEnd(',', '"');
                }
            }

            return strReturn;
        } // GetClientType

        public static string GetVdiMode()
        {
            string strVdiNum = "Not found";
            string strVdiModeText;
            string strReturn = "Not found";
            int iSearch = 0;

            if (g_strFileType == "mainlog")
            {
                iSearch = TextPos(g_allText, "vdiMode:", 0, true);
                if (iSearch != -1)
                {
                    strVdiNum = g_allText.Substring(iSearch + 1, 4);
                    strVdiNum = strVdiNum.TrimEnd('e', 'v', 'S', 'c', 'p');
                    strVdiNum = strVdiNum.Trim();
                    strVdiNum = strVdiNum.TrimEnd(',');
                }
            }
            else if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "vdiMode\":", 0, false);
                if (iSearch != -1)
                {
                    strVdiNum = g_allText.Substring(iSearch + 2, 4);
                    strVdiNum = strVdiNum.Trim();
                    strVdiNum = strVdiNum.TrimEnd(',', '"');
                }
            }
            
            if (!strVdiNum.Contains("Not found"))
            {
                strVdiModeText = GetVdiModeDecoded(strVdiNum);
                strReturn = strVdiNum + ": " + strVdiModeText;
            }

            return strReturn;
        } // GetVdiMode

        public static string GetOSVer()
        {
            string strReturn = "Not found";
            string strOSVer = "Not found";
            string strOSBuild = "";
            string strOSInfo;
            int iSearch = 0;
            int iRet = 0;

            if (g_strFileType == "mainlog")
            {
                iSearch = TextPos(g_allText, "osversion", 0, true);
                if (iSearch != -1)
                {
                    iSearch = iSearch + 1;
                    iRet = g_allText.IndexOf('\n', iSearch);
                    strOSVer = g_allText.Substring(iSearch, iRet - iSearch);
                    strOSVer = strOSVer.Trim();
                    strOSVer = strOSVer.TrimEnd('"', ',', '\n');
                    strOSVer = strOSVer.Trim(); 
                }
            }
            else if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "osversion", 0, false);
                if (iSearch != -1)
                {
                    iSearch = iSearch + 4;
                    iRet = g_allText.IndexOf('\n', iSearch);
                    strOSVer = g_allText.Substring(iSearch, iRet - iSearch);
                    strOSVer = strOSVer.Trim();
                    strOSVer = strOSVer.TrimEnd('"', ',', '\n');
                    strOSVer = strOSVer.Trim();
                }
            }
            if (strOSVer != "Not found")
            {
                strOSBuild = GetOSBuild(strOSVer);
                strOSInfo = GetOSVerInfo(strOSBuild);

                strReturn = strOSVer + " - " + strOSInfo;
            }
        
            return strReturn;
        } // GetOSVer

        private static string GetOSBuild(string strOSVer)
        {
            string strOSBuild = "";

            if (strOSVer.StartsWith("10.1") || strOSVer.StartsWith("11.") || strOSVer.StartsWith("12."))
            {
                strOSBuild = strOSVer;
            }
            else if (strOSVer.StartsWith("6.2.") || strOSVer.StartsWith("6.3."))
            {
                strOSBuild = strOSVer.Substring(4);
            }
            else if (strOSVer.Contains("22000"))
            {
                strOSBuild = "22000"; //Windows 11 - I think...
            }
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
        }

        public static string GetTimezone()
        {
            string strReturn = "Not found";
            int iSearch = 0;

            if (g_strFileType == "mainlog")
            {
                iSearch = TextPos(g_allText, "GMT", 0, false);
                if (iSearch != -1)
                {
                    strReturn = g_allText.Substring(iSearch - 3, 8);
                }
            }
            else if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "timezone\":", 0, false);
                if (iSearch != -1)
                {
                    string strTZ = "";
                    strTZ = g_allText.Substring(iSearch + 1, 3);
                    strTZ = strTZ.Trim('\n');
                    strTZ = strTZ.Trim(',');

                    if (strTZ.Contains("-"))
                    {
                        if (strTZ.Length == 2)
                        {
                            strTZ = strTZ.Remove(0, 1);
                            strTZ = "-0" + strTZ;
                        }
                        strTZ = "GMT" + strTZ + "00";
                    }
                    else
                    {
                        if (strTZ.Length == 1)
                        {
                            strTZ = "0" + strTZ;
                        }
                        strTZ = "GMT+" + strTZ + "00";
                    }
                    strReturn = strTZ;
                }
            }

            return strReturn;
        } // GetTimezone

        public static string GetHostname()
        {
            string strReturn = "Not found";
            int iSearch = 0;
            int iRet = 0;

            if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "hostname\":", 0, false);
                if (iSearch != -1)
                {
                    iSearch = iSearch + 2;
                    iRet = g_allText.IndexOf('\n', iSearch);
                    string strHost = g_allText.Substring(iSearch, iRet - iSearch);
                    strHost = strHost.Trim();
                    strHost = strHost.TrimEnd(',', '\"');
                    strReturn = strHost;
                }
            }

            return strReturn;
        } // GetHostname

        public static string GetIPAddr()
        {
            string strReturn = "Not found";
            string strIPAddr = "";
            int iSearch = 0;
            int iRet = 0;
            int iComma = 0;

            if (g_strFileType == "mainlog")
            {
                iSearch = TextPos(g_allText, "ipaddr\":", 0, false);
                if (iSearch != -1)
                {
                    iSearch = iSearch + 1;
                    iComma = g_allText.IndexOf(',', iSearch);
                    strIPAddr = g_allText.Substring(iSearch, iComma - iSearch);
                    strIPAddr = strIPAddr.Trim();
                    strIPAddr = strIPAddr.TrimEnd(',', '\"');
                    strReturn = strIPAddr;
                }
            }

            if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "ipaddr\":", 0, false);
                if (iSearch != -1)
                {
                    iSearch = iSearch + 2;
                    iRet = g_allText.IndexOf('\n', iSearch);
                    strIPAddr = g_allText.Substring(iSearch, iRet - iSearch);
                    strIPAddr = strIPAddr.Trim();
                    strIPAddr = strIPAddr.TrimEnd(',', '\"');
                    strReturn = strIPAddr;
                }
            }

            return strReturn;
        } // GetIPAddr

        public static string GetDeviceInfo()
        {
            string strReturn = "Not found";
            int iSearch = 0;
            int iRet = 0;

            if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "DeviceInfo.SystemManufacturer", 0, false);
                if (iSearch != -1)
                {
                    iSearch = iSearch + 4;
                    iRet = g_allText.IndexOf('\n', iSearch);
                    string strMaker = g_allText.Substring(iSearch, iRet - iSearch);
                    strMaker = strMaker.Trim();
                    strMaker = strMaker.TrimEnd(',', '\"');
                    strReturn = strMaker;
                }

                iSearch = TextPos(g_allText, "DeviceInfo.SystemProductName", 0, false);
                if (iSearch != -1)
                {
                    iSearch = iSearch + 4;
                    iRet = g_allText.IndexOf('\n', iSearch);
                    string strMachine = g_allText.Substring(iSearch, iRet - iSearch);
                    strMachine = strMachine.Trim();
                    strMachine = strMachine.TrimEnd(',', '\"');
                    strReturn += ", " + strMachine;
                }
            }
            return strReturn;
        }

        public static string GetCPU()
        {
            string strReturn = "Not found";
            int iSearch = 0;
            int iRet = 0;

            if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "cpumodel", 0, false);
                if (iSearch != -1)
                {
                    iSearch = iSearch + 4;
                    iRet = g_allText.IndexOf('\n', iSearch);
                    string strCPU = g_allText.Substring(iSearch, iRet - iSearch);
                    strCPU = strCPU.Trim();
                    strCPU = strCPU.TrimEnd(',', '\"');
                    strReturn = strCPU;

                    iSearch = TextPos(g_allText, "cores\":", 0, false);
                    if (iSearch != -1)
                    {
                        string strCores = g_allText.Substring(iSearch, 3);
                        strCores = strCores.Trim();
                        strCores = strCores.TrimEnd(',', '\n');
                        strReturn += ", " + strCores + " cores";
                    }
                }
            }

            return strReturn;
        } // GetCPU

        public static string GetMem()
        {
            string strReturn = "Not found";
            int iSearch = 0;
            int iRet = 0;

            if (g_strFileType == "maindiag")
            {
                iSearch = TextPos(g_allText, "totalMemory", 0, false);
                if (iSearch != -1)
                {
                    iSearch = iSearch + 3;
                    iRet = g_allText.IndexOf('\n', iSearch);
                    string strMem = g_allText.Substring(iSearch, iRet - iSearch);
                    strMem = strMem.Trim();
                    strMem = strMem.TrimEnd(',', '\"');
                    strReturn = strMem;
                }
            }

            return strReturn;
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
                iSearch = TextPos(g_allText, "CallId:", 0, false);
                while (iSearch != -1)
                {
                    iFirstPos = iSearch;
                    iSearch = iSearch + 1;
                    strCallID = g_allText.Substring(iSearch, 36);

                    if (strCallID.Contains(":"))
                    {
                        iSearch = TextPos(g_allText, "CallId:", iSearch + 1, false);
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

                    iSearch = TextPos(g_allText, "CallId:", iSearch + 1, false);
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
            int iSearch = 0;
            string strStart = "";
            string strEnd = "";


            if (g_strFileType == "mainlog")
            {
                foreach (string strLine in g_allLines)
                {
                    if (strLine.Contains("GMT"))
                    {
                        iSearch = TextPos(strLine, "<", 0, false);
                        strStart = strLine.Substring(0, iSearch-2);
                        break;
                    }
                }

                foreach (string strLine in g_allLines.Reverse<string>())
                {
                    if (strLine.Contains("GMT"))
                    {
                        iSearch = TextPos(strLine, "<", 0, false);
                        strEnd = strLine.Substring(0, iSearch-2);
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
                        iSearch = TextPos(strLine, "Z", 0, false);
                        strStart = strLine.Substring(0, iSearch-1);
                        strStart = strStart.Replace('T', ' ');
                        strStart = strStart + " GMT";
                        break;
                    }
                }

                foreach (string strLine in g_allLines)
                {
                    if (strLine.Contains("Z Inf") || strLine.Contains("Z War") || strLine.Contains("Z Err"))
                    {
                        iSearch = TextPos(strLine, "Z", 0, false);
                        strEnd = strLine.Substring(0, iSearch-1);
                        strEnd = strEnd.Replace('T', ' ');
                        strEnd = strEnd + " GMT";
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
                        iSearch = TextPos(strLine, "[", 0, false);
                        strStart = strLine.Substring(0, iSearch - 2);
                        strStart = strStart + " GMT";
                        break;
                    }
                    // For Dev TAT decoded file
                    else if (strLine.Contains("TL_INFO")|| strLine.Contains("TL_WARN") || strLine.Contains("TL_ERROR") || strLine.Contains("TL_FATAL"))
                    {
                        iSearch = TextPos(strLine, "  ", 10, false);
                        strStart = strLine.Substring(10, iSearch - 12);
                        strStart = strStart + " GMT";
                        break;
                    }
                }

                foreach (string strLine in g_allLines.Reverse<string>())
                {
                    // For BRB decoded file
                    if (strLine.Contains("[#") && strLine.Contains("-S]"))
                    {
                        iSearch = TextPos(strLine, "[", 0, false);
                        strEnd = strLine.Substring(0, iSearch - 2);
                        strEnd = strEnd + " GMT";
                        break;
                    }
                    // For Dev TAT decoded file
                    else if (strLine.Contains("TL_INFO") || strLine.Contains("TL_WARN") || strLine.Contains("TL_ERROR") || strLine.Contains("TL_FATAL"))
                    {
                        iSearch = TextPos(strLine, "  ", 10, false);
                        strEnd = strLine.Substring(10, iSearch - 12);
                        strEnd = strEnd + " GMT";
                        break;
                    }
                }
            }

            strReturn = strStart + " to " + strEnd;

            return strReturn;
        } // GetLogDateTimes

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
