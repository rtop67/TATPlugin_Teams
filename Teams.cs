using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using static TATPlugin_Teams.Resource;
using static TATPlugin_Teams.GetInfo;
using static TATPlugin_Teams.CallingDiag;
using static TATPlugin_Teams.MainLogs;
using static TATPlugin_Teams.RendererDiag;
using static TATPlugin_Teams.MediaMsrtc;

namespace TATPlugin_Teams
{
    public class Teams
    {
        public static string g_strFileType = "";
        public static string g_strFileName = "";
        public static string[] g_allTextSplit;
        public static string g_allText;
        public static List<string> g_allLines = new List<string>();
        public static bool g_bSupported;
        public static string g_strParsedFile = "";

        public static List<string> g_CrashLines = new List<string>();
        public static List<string> g_CallIDs = new List<string>();
        public static List<string> g_VideoDevices = new List<string>();

        // Required to get the DLL loaded, to show file type on Open File Dialog
        public string GetFileTypeDescription()
        {
            return "Teams Log Files";
        }

        // Required to get the DLL loaded, to show file type on Open File Dialog
        public string GetFileTypePattern()
        {
            return "*.txt;*.log";
        }

        // Required to get the DLL loaded.  Does this DLL/Plugin support the file being loaded?
        public bool IsFileTypeSupported(string strFile)
        {
            g_bSupported = false;

            g_strFileType = GetFileType(strFile);

            if (g_strFileType == "mainlog" || g_strFileType == "maindiag" || g_strFileType == "rendererdiag" || g_strFileType == "callingdiag" || g_strFileType == "mediamsrtc")
                g_bSupported = true;

            return g_bSupported;
        }

        // Required to get the DLL loaded. Open file and return a TextReader back to TAT.
        public TextReader GetReaderForFile(string strFile)
        {
            //Do some stuff to get info for the user from the logs
            if (IsFileTypeSupported(strFile))
            {
                ResetGlobals();
                CreateParsedFile(strFile);
                OpenFile(strFile);
                ParseFile();
                WriteParsedFile();
            }

            // Open the file and get the TextReader setup to return back to the main app.
            try
            {
                FileInfo fileInfo = new FileInfo(g_strParsedFile);
                FileStream fsFile = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                TextReader trFile = new StreamReader(fsFile);

                return trFile;
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Error: {ex.Message}", g_strParsedFile);
            }
        }

        // If I get there and if it's needed, can do a settings dialog for the plugin
        /*public static Form GetSettingsDialog()
        {
            return null;
        }*/

        // Open the file - prep for parsing
        private void OpenFile(string strFile)
        {
            g_allText = File.ReadAllText(strFile);
            g_allTextSplit = g_allText.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (string strLine in g_allTextSplit)
            {
                g_allLines.Add(strLine);
            }
        }

        private void ResetGlobals()
        {
            g_allLines.Clear();
            g_CrashLines.Clear();
            g_CallIDs.Clear();
            g_allText = "";
            g_allCallingLines.Clear();
            g_MainInfoLines.Clear();
            g_RendererLines.Clear();
            g_MediaLines.Clear();
        }

        //Create a new file with -PARSED at the end of the file name - so we know it is parsed
        private void CreateParsedFile(string strFile)
        {
            FileInfo fi = new FileInfo(strFile);
            string strFileNoExt = Path.GetFileNameWithoutExtension(fi.Name);
            string strExt = fi.Extension;
            string strDir = fi.DirectoryName;

            g_strParsedFile = strDir + "\\" + strFileNoExt + "-PARSED" + strExt;

            //create the file - for our new parsed file
            File.Create(g_strParsedFile).Close();
        }

        // Parse our file
        private void ParseFile()
        {
            switch (g_strFileType)
            {
                case "mainlog":
                case "maindiag":
                    ParseMainLogs();
                    break;
                case "rendererdiag":
                    ParseRendererDiag();
                    break;
                case "callingdiag":
                    ParseCallingDiag();
                    break;
                case "mediamsrtc":
                    ParseMediaLog();
                    break;
            }
        } // ParseFile

        // Write relevant text to the parsed file
        private void WriteParsedFile()
        {
            switch (g_strFileType)
            {
                case "mainlog":
                case "maindiag":
                    WriteMainLog();
                    break;
                case "rendererdiag":
                    WriteRendererDiag();
                    break;
                case "callingdiag":
                    WriteCallingDiag();
                    break;
                case "mediamsrtc":
                    WriteMediaLog();
                    break;
            }
        } // WriteParsedFile
    }
}
