using BaiRocAgent;
using BaiRocs.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BaiRocWindowsService
{
    public partial class BaiService : ServiceBase
    {
        public BaiService()
        {
            InitializeComponent();

        }
        Timer timer1 = new Timer(); // name space(using System.Timers;) 
        Timer timer2 = new Timer(); // name space(using System.Timers;) 

        protected override void OnStart(string[] args)
        {
            timer1.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer1.Interval = 10000; //number in milisecinds  
            timer1.Enabled = true;

            timer2.Elapsed += new ElapsedEventHandler(OnElapsedTime2);
            timer2.Interval = 5000; //number in milisecinds  
            timer2.Enabled = false;
            Global.IdleCountSet = 5;

            Global.IdleCount = Global.IdleCountSet;



        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            // WriteToFile("Service is recall at " + DateTime.Now);
            Global.ProcessStatus = "ready";
            timer2.Enabled = true;
            timer1.Enabled = false;
            Global.LogWarn("Timer1 ended.");

        }

        private void OnElapsedTime2(object source, ElapsedEventArgs e)
        {
            // WriteToFile("Service is recall at " + DateTime.Now);
            if (Global.ProcessStatus != "ready")
                return;

            try
            {
                Global.ProcessStatus = "busy";
                //var frontDir = FileService.Config.GetValue("FrontFolder");
                string dumpDir = FileService.Config.GetValue("DumpFolder");

                FileService.CleanRootFolder();
                FileService.DigForImageFiles(dumpDir, 0);
                //Thread.Sleep(10000);
               if( FileService.CheckIsScannerRunning())
                {
                    Global.RunBaiRocs();
                }

            }
            catch (Exception err)
            {
                Global.LogError(err);
            }
            finally
            {
                Global.ProcessStatus = "ready";

            }
        }
        protected override void OnStop()
        {
            Global.LogError("BaiRoc Service Stopped.");
        }

       
    }
}
