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

        //Get Camera / Screen Sharing
        public static void GetVidDev()
        {
            int iSearch = 0;
            int iFirstPos = 0;
            string strVidDev = "";


        }
    }
}