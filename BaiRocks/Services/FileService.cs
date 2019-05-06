using BaiRocs.WF;
using LogApplication.Common.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Services
{
  public static class FileService
    {

        public static List<string> RootFileRestrictions
        {
            get
            {
                List<string> restrictions = new List<string>
                {
                    "receipt.csv",
                    "readme.lnk"
                };

                return restrictions;
            }
        }
        public static List<string> ImageFileRestrictions
        {
            get
            {
                List<string> restrictions = new List<string>
                {
                    ".bmp",
                    ".png",
                    ".jpg"
                };

                return restrictions;
            }
        }
        private static ConfigManager s_config;
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


        public static void CleanRootFolder()
        {

            //0. Delete empty folders...
            var rootDir = Config.GetValue("RawImageFolder");
            var deleteFiles = Directory.GetFiles(rootDir, "done.txt", SearchOption.AllDirectories);
            foreach (string delFile in deleteFiles)
            {
                try
                {
                    var del = Path.GetDirectoryName(delFile);
                    Directory.Delete(del, true);
                }
                catch (Exception err)
                {
                    Global.LogError(err.Message);
                }
            }

            //delete empty folders
            try
            {
                var users = Directory.GetDirectories(rootDir, "*", SearchOption.TopDirectoryOnly);
                foreach (string user in users)
                {
                    var images = Directory.GetDirectories(user, "*", SearchOption.TopDirectoryOnly);
                    foreach (string imagedir in images)
                    {
                        var fileImages = Directory.GetFiles(imagedir, "*.*", SearchOption.AllDirectories);
                        if (fileImages.Count() == 0)
                            Directory.Delete(imagedir, true);

                    }

                }
            }
            catch (Exception err)
            {

                Global.LogError(err.Message);
            }


            //remove log.txt etc  in user folder
            try
            {
                var users = Directory.GetDirectories(rootDir, "*", SearchOption.TopDirectoryOnly);
                foreach (string user in users)
                {
                    var xtrafiles = Directory.GetFiles(user, "*.*", SearchOption.TopDirectoryOnly);
                    foreach (string f in xtrafiles)
                    {
                        if (Path.GetFileName(f) != "receipt.csv")
                        {
                            File.Delete(f);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Global.LogError(err.Message);
                // throw;
            }




          



        }
        public static List<FileInfo> FindImages()
        {

            //1. //rgalvez folder Set CURRENT USER folder 
            List<FileInfo> imageFiles = new List<FileInfo>();

            if (SetCurrentUserFolder())
            {
                //2 rgalvez/imagefolder
                var rootImageFolder = Global.CurrentUserFolder;


                //Global.CurrentListImageFolder =new DirectoryInfo(rootImageFolder).GetDirectories();

                //3. get log.txt list
                var logList = new DirectoryInfo(rootImageFolder).GetFiles("log.txt", SearchOption.AllDirectories).ToList();
                logList = logList.OrderBy(f => f.CreationTime).ToList();

                var currentLog = logList.FirstOrDefault();

                if (logList.Count > 0)
                {
                    //Global.CurrentImageFolder = currentLog.Directory;
                    var CurrentImageFolder = currentLog.Directory;
                    Global.CurrentImageFolder = CurrentImageFolder.FullName;

                    //4. final list of images in current folder
                    //var filesAll = Global.CurrentImageFolder.GetFiles();
                    var filesAll = CurrentImageFolder.GetFiles();

                    List<string> restrictions = new List<string>
                {
                    ".bmp",
                    ".png",
                    ".jpg"
                };
                    foreach (var f in filesAll)
                    {
                        var fname = f.FullName;
                        var ext = Path.GetExtension(fname);
                        if (!restrictions.Contains(ext.ToLower()))
                        {
                            var filename = Path.GetFileName(fname).ToLower();
                            if (filename != "log.txt" && filename != "receipt.csv")
                            {
                                File.Delete(fname);
                                Global.LogError("File Deleted..." + fname);
                                //bindingSource1.RemoveCurrent();
                            }

                        }
                        else
                        {
                            imageFiles.Add(f);
                        }

                    }
                }
                else
                {
                    //Global.CurrentImageFolder = null;
                    Global.CurrentImageFolder = string.Empty;
                }

                //5 so what is the current image folder


                // bindingSource1.DataSource = imageFiles.ToList();
                // dgFiles.DataSource = bindingSource1;
            }
            else
            {
                // bindingSource1.DataSource = imageFiles.ToList();
                // dgFiles.DataSource = bindingSource1;
                Global.LogWarn("Nothing to convert.");
                Global.HasNothingToConvert = true;

            }

            return imageFiles;

        }


        public static bool SetCurrentUserFolder()
        {
            //SetStatus(ProcessStatus.Searching);
            var rootDir = Config.GetValue("RawImageFolder");
            var dir = new DirectoryInfo(rootDir);
            //FileInfo[] files = dir.GetFiles().Take(10);
            var motherDir = new DirectoryInfo(rootDir);

            //sort
            var logList = motherDir.GetFiles("log.txt", SearchOption.AllDirectories).ToList();
            logList = logList.OrderBy(f => f.CreationTime).ToList();
            var currentLog = logList.FirstOrDefault();

            if (currentLog != null)
            {
                DirectoryInfo[] Folders = motherDir.GetDirectories("*", SearchOption.TopDirectoryOnly); //( Directory.getd(rootDir, "*.*", SearchOption.AllDirectories).Take(5);
                var userStart = currentLog.DirectoryName.ToLower();
                //var dirList = Folders.ToList().Where(f => userStart.StartsWith(f.FullName.ToLower()));
                Global.CurrentUserFolder = Path.GetDirectoryName(userStart); //dirList.FirstOrDefault().FullName;//Folders.OrderBy(f => f.LastAccessTime).FirstOrDefault();

                Global.LogWarn("Global.CurrentFolder -->" + Path.GetFileName(Global.CurrentUserFolder));
                Global.CurrentUser = Path.GetFileName(Global.CurrentUserFolder);

                return true;
            }
            else
            {
                //TODO...
                //nothing to convert- no log files
                return false;
            }



        }

        public static void ClearCSVFiles(string dir)
        {
           var csvs= Directory.GetFiles(dir, "*.csv", SearchOption.AllDirectories);
            foreach(string f in csvs)
            {
                try
                {
                    File.Delete(f);
                }
                catch (Exception err)
                {

                    Global.LogError(err.Message);
                }
            }
        }

        public static void CreateLogText(string dir)
        {
            var log = Path.Combine(dir, "log.txt");
            if (!File.Exists(log))
            {
                var sw = File.CreateText(log);
                sw.Close();
            }


        }


    }
}
