using System;
using System.Linq;
using static TATPlugin_Teams.Teams;
using static TATPlugin_Teams.Resource;
using static TATPlugin_Teams.GetInfo;

namespace TATPlugin_Teams
{
    class GetMediaInfo
    {
        // For getting the BandwidthCaps from Media MSRTC logs
        public static string GetBWCap()
        {
            string strReturn = "Not found";
            string strUp = "";
            string strDown = "";
            int iSearch = 0;


            if (g_strFileType == "mediamsrtc")
            {
                iSearch = TextPos(g_allText, "\"BandwidthCaps\":", 0, false);
                if (iSearch != -1)
                {
                    int iLB = TextPos(g_allText, "[", iSearch, false);
                    int iRB = TextPos(g_allText, "]", iSearch, false);
                    int iLen = iRB - iLB + 1;
                    strReturn = g_allText.Substring(iLB - 1, iLen);

                    int iUL = TextPos(strReturn, "\"uplink\":", 0, false);
                    int iComma = TextPos(strReturn, ",", iUL, false);
                    iLen = iComma - iUL;
                    strUp = strReturn.Substring(iUL, iLen - 1);

                    int iDL = TextPos(strReturn, "\"downlink\":", 0, false);
                    int iCurl = TextPos(strReturn, "}", iDL, false);
                    iLen = iCurl - iDL;
                    strDown = strReturn.Substring(iDL, iLen - 1);

                    int iUp = Int32.Parse(strUp);
                    int iDown = Int32.Parse(strDown);

                    string strMbpsUp = Byteps2Mbps(iUp);
                    string strMbpsDown = Byteps2Mbps(iDown);

                    strUp = strUp + " bytes/sec UP == " + strMbpsUp;
                    strDown = strDown + " bytes/sec DOWN == " + strMbpsDown;

                    strReturn = strReturn + " >> " + strUp + " | " + strDown;

                }
            }

            return strReturn;
        }

        // For getting the ECS Bandwidth Cap - if present in the log
        public static string GetECSBWCap()
        {
            string strReturn = "Not found";
            int iSearch = 0;

            if (g_strFileType == "mediamsrtc")
            {
                iSearch = TextPos(g_allText, "Bandwidth cap is set from ECS:", 0, false);
                if (iSearch != -1)
                {
                    iSearch++;
                    int iPeriod = TextPos(g_allText, ".", iSearch, false);
                    int iLen = iPeriod - iSearch;
                    string strCap = g_allText.Substring(iSearch, iLen - 1);

                    int iCap = Int32.Parse(strCap);
                    string strMbpsCap = Bitsps2Mbps(iCap);

                    strReturn = strCap + " >> " + strMbpsCap;
                }
            }

            return strReturn;
        }

        //Get Cameras / Screen Sharing Used
        public static void GetVidDev()
        {
            int iSearch = 0;
            int iFirstPos = 0;
            int iEnd = 0;
            int iLen = 0;
            string strVidDev = "";

            if (g_strFileType == "mediamsrtc")
            {
                iSearch = TextPos(g_allText, "source friendly-name:", 0, false);
                while (iSearch != -1)
                {
                    iFirstPos = iSearch;
                    iSearch = iSearch + 1;

                    iEnd = TextPos(g_allText, "\n202", iSearch, false); // can't seem to just find "\n" so have to hack it a bit using characters from the next line (BRB output)

                    if (iEnd == -1)  // For TAT decoded files need to do something different.
                    {
                        iEnd = TextPos(g_allText, "(", iSearch, false);
                        iEnd = iEnd - 2;
                    }
                    else
                    {
                        iEnd = iEnd - 4;  // hack to eliminate the extra characters. (for BRB decoded logs)
                    }
                    
                    iLen = iEnd - iSearch;
                    strVidDev = g_allText.Substring(iSearch, iLen);
                    
                    bool bFound = false;
                    foreach (string strItem in g_VideoDevices)
                    {
                        if (strItem.Contains(strVidDev))
                        {
                            bFound = true;
                        }
                    }
                    if (bFound == false)
                    {
                        g_VideoDevices.Add(strVidDev);
                    }
                    iSearch = TextPos(g_allText, "source friendly-name:", iSearch+1, false);
                    if (iSearch <= iFirstPos)
                        break;
                }
            }
        }
    }
}