using BaiRocs.WF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiRocs
{
    static class Program
    {

        //debug mode


        //_HACK MAIN safe to delete    
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "-autorun")
                {
                    // call http client args[i+1] for URL
                    Global.AutoRun = true;


                }
            }
            Application.Run(new MainFrm());



        }
        //static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
        static Mutex mutex = new Mutex(true, "{5efbf2f3-4904-4799-9753-6d20320e26ad}");

        //release mode
        static void xMain(string[] args)
        {

            //for (int i = 0; i < args.Length; i++)
            //{
            //    if (args[i].ToLower() == "-autorun")
            //    {
            //        // call http client args[i+1] for URL
            //        Global.AutoRun = true;
            //    }
            //}


            Global.AutoRun = true;
            Global.IsShuttingDown = false;

            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                MainFrm frm = new MainFrm();
                frm.Show();
                frm.AutoRun();
                mutex.ReleaseMutex();
            }
            else
            {
                // MessageBox.Show("only one instance at a time");
                Global.LogError("Only one instance of BAIROCS can run.");
            }      
                     

        }
    }
}
