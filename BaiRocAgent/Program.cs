using BaiRocs.Services;
using LogApplication;
using System;
using System.Collections.Generic;
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
            var frontDir = FileService.Config.GetValue("FrontFolder");
            string dumpDir = FileService.Config.GetValue("DumpFolder");

           var  _dirWatcher = new FileSystemWatcher(frontDir);
            _dirWatcher.IncludeSubdirectories = false;
            _dirWatcher.NotifyFilter = NotifyFilters.DirectoryName;
            _dirWatcher.EnableRaisingEvents = true;
            _dirWatcher.Created += FsWatcher_Created;


            //2.loop
            while (!Global.IsShuttingDown)
            {
                FileService.CleanRootFolder();
                FileService.DigForImageFiles(dumpDir,0);
                Thread.Sleep(10000);
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
