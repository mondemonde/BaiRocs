using BaiRocs.Services;
using LogApplication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaiRocAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            //1. ini       

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            var frontDir = FileService.Config.GetValue("FrontFolder");
            string dumpDir = FileService.Config.GetValue("DumpFolder");
            Global.IdleCountSet = 5;
            Global.IdleCount = Global.IdleCountSet;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            Global.LogInfo("Running Version: " + version);



            //2.loop
            while (!Global.IsShuttingDown)
            {
                try
                {
                        FileService.CleanRootFolder();
                        FileService.DigForImageFiles(dumpDir,0);
                        Thread.Sleep(10000);

                }
                catch (Exception err)
                {

                    Global.LogError(err);
                }
            }


        }

        private static void FsWatcher_Created(object sender, FileSystemEventArgs e)
        {
            //throw new NotImplementedException();
            //if(e.ChangeType== WatcherChangeTypes.Created && e.
            Global.LogWarn("Folder Created: " + e.FullPath);
            var target = e.FullPath + "\\ReadMe.url";
            if (!File.Exists(target))
            {
                var curDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                File.Copy(curDir + "\\ReadMe.url",target);
            }
        }
    }
}
