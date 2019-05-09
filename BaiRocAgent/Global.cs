using LogApplication.Common.Commands;
using LogApplication.INFRA;
using MyCommonLib.DAL;
using MyCommonLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using LogApplication.Common.Config;
using System.IO;
using System.Diagnostics;

namespace BaiRocAgent
{
    public static class Global
    {
        #region EVENTS


        public static bool IsShuttingDown { get; set; }

        #endregion


        #region ------------------------------------LOGGING---------------------


        static DiagnosticLogger _logger;
        private static string s_processStatus;
        private static ConfigManager s_config;

        public static DiagnosticLogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = new DiagnosticLogger();

                return _logger;
            }
            set
            {
                _logger = value;
            }

        }
        public static void ClearLog()
        {
            _logger = new DiagnosticLogger();
        }
        public static void LogError(Exception err)
        {
            Logger.Error(err);
        }
        public static void LogError(string err)
        {
            Logger.AddMessage(err);
            Logger.Error();
        }
        public static void LogInfo(string info)
        {
            Logger.Log(info);
        }

        public static void LogWarn(string warn)
        {
            Logger.AddMessage(warn);
            Logger.Warn();

        }


        #endregion


        #region STATIC METHODS


        // public static   Proj CurrentProject { get; set; }

        /// <summary>
        /// this is the base folder web app running
        /// </summary>
        public static string RootDirectory { get; set; }

        public static ConfigManager Config
        {
            get
            {
                if (s_config == null)
                    s_config = new ConfigManager();
                return s_config;
            }
            set { s_config = value; }
        }


        public static bool IsScanBusy { get; set; }
        public static bool MakeScanIdle { get; set; }

        public static string ProcessStatus { get => s_processStatus; set => s_processStatus = value; }

        //public static DirectoryInfo CurrentUserFolder { get; set; }
        public static string CurrentUserFolder { get; set; }

        public static string CurrentUser { get; set; }

        public static string CurrentImageFolder { get; set; }
        public static string CurrentImagePath { get; set; }
        public static bool HasNothingToConvert { get; set; }

        public static int IdleCount { get; set; }
        public static int IdleCountSet { get; set; }

        public static void RunBaiRocs()
        {
            try
            {
                string exe = Global.Config.GetValue("BaiRocsExe");
                Process p = Process.Start(exe,"-autorun");
            }
            catch(Exception err)
            {
                // Debugger.Break();
                Global.LogError(err.Message);
            }
        }

        #endregion end STatic


    }

}
