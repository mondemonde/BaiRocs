using LogApplication.Common.Commands;
using LogApplication.INFRA;
using MyCommonLib.DAL;
using MyCommonLib.Model;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using LogApplication.Common.Config;
using BaiRocs.DAL;
using BaiRocs.Common;
using System.IO;
using BaiRocs.Models;
//using BaiRocs.DAL;
//using BaiRocs.MODEL;

namespace BaiRocs.WF
{
    public static class Global
    {
        #region EVENTS


        public static bool IsShuttingDown { get; set; }

        //this is itialize by background worker
        public static WorkflowApplication ThisWF;
        public static MainFrm MainWindow;

        public static void ReadEvent(object state)
        {
            if (state == null) return;

            // string text = state.ToString(); //"Hello bookmark!"; //Console.ReadLine();
            //var cmdParam = state as ChessCommandParam;//CmdParam;
            // Resume the Activity that set this bookmark (ReadString).
            // Global.ThisWF.ResumeBookmark(cmdParam.BookMarkName, cmdParam);
        }

        public static void ReadEventString(string state)
        {
            if (state == null) return;
            string text = state;
            //var cmdParam = state as ChessCommandParam;//CmdParam;
            //Resume the Activity that set this bookmark(ReadString).
            Global.ThisWF.ResumeBookmark("EventString", text);
        }

        public static void ReadEventCmdParam(CmdParam state)
        {
            if (state == null) return;
            //var cmdParam = state as ChessCommandParam;//CmdParam;
            //Resume the Activity that set this bookmark(ReadString).
            Global.ThisWF.ResumeBookmark("CmdParamBookMark", state);
        }

        public static void StartMainWindow()
        {
            MainWindow = new MainFrm();
            MainWindow.Show();
        }


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

        public static void Ini()
        {
            ConfigManager config = new ConfigManager();
            Global.MainWindow.MyConfig = config;
            Config = config;

            LogInfo(config.GetValue("TestKey"));

            #region MYDB
            using (MyDBContext db = new MyDBContext())
            {
                var rawQuery = db.TableConfigs.ToList();
                var thisConfig = rawQuery.First();
                Console.WriteLine("DB initialize: test local db of " + thisConfig.Id.ToString());
            }

            #endregion  

            #region WORKING DIR
            var MyWorkingDirList = new List<KeyValuePair<string, string>>();
            using (MyDBContext db = new MyDBContext())
            {
                //var rawQuery = db.AifsProjects.OrderBy(p=>p.Project).ThenBy(p=>p.Branch).ToList();
                //Console.WriteLine("DB initialize: WORKING DIR list");

                //foreach(Proj p in rawQuery )
                //{
                //    MyWorkingDirList.Add(new KeyValuePair<string, string>(
                //                   p.ProjectName,
                //                    p.WorkingDir));
                //    Console.WriteLine(p.ProjectName + " - " + p.WorkingDir);
                //}


            }

            //MyReceiptMetaData.NatureOfTnx = new Dictionary<string, string> {
            //    {"Parking" ,"Parking fee" },
            //    { "Gasoline","Gasoline"},
            //    { "Transportation","Transportation fare/Grab/Uber"},
            //    {"Hotel","Hotel accomodation/lodging"},
            //    { "Meal","Meal - Others"},
            //    {"Grocery","Grocery(food items)"},
            //    {"Repairs", "Repairs and Maintenance"},
            //    {"Telephone","Telephone /Mobile billing"},
            //    {"Wellness", "Beauty and wellness"},
            //    {"Appliances", "Home appliances"},
            //    {"Internet", "Internet billing"}

            //};


            #endregion


        }

        public static bool IsScanBusy { get; set; }
        public static bool MakeScanIdle { get; set; }

        public static string ProcessStatus { get => s_processStatus; set => s_processStatus = value; }
        public static List<BaiOcrLine> OcrLines { get; set; }
        public static Sigma CurrentSigma { get; set; }

        //public static DirectoryInfo CurrentUserFolder { get; set; }
        public static string CurrentUserFolder { get; set; }

        public static string CurrentUser { get; set; }

        public static string CurrentImageFolder { get; set; }
        public static string CurrentImagePath { get; set; }


        public static Receipt CurrentReciept { get; set; }
        public static bool HasNothingToConvert { get; set; }


        #endregion end STatic


    }

}
