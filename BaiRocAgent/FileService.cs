using BaiRocAgent;
using LogApplication;
using LogApplication.Common;
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
                    "ReadMe.lnk",
                    "ReadMe.url"
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
            var rootDir = Config.GetValue("FrontFolder");
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
                       DirectoryInfo dir = new DirectoryInfo(imagedir);
                       TimeSpan span =  DateTime.Now - dir.CreationTime;
                        if (span.Minutes > 30)
                        {
                            var fileImages = Directory.GetFiles(imagedir, "*.*", SearchOption.AllDirectories);

                            if (fileImages.Count() == 0)
                                Directory.Delete(imagedir, true);

                            else if (fileImages.Count() == 1)
                            {
                                if (Path.GetFileName(fileImages[0]).ToLower() == "log.txt")
                                {
                                    Directory.Delete(imagedir, true);
                                }
                            }
                        }
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
                        if (Path.GetExtension(f) != ".csv")
                        {
                            var fName = Path.GetFileName(f);

                            if (!(fName == "ReadMe.lnk" || fName == "ReadMe.url"))
                            {
                                if (!RootFileRestrictions.Contains(fName))
                                {
                                    File.Delete(f);
                                    Global.LogWarn("File DELETED: " + f);
                                }

                            }

                           
                        }
                    }

                    //add readme link
                    var link = Path.Combine(user, "ReadMe.url");
                    if (!File.Exists(link))
                    {
                        var curDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        File.Copy(curDir + "\\ReadMe.url", link);

                    }

                }
            }
            catch (Exception err)
            {
                Global.LogError(err.Message);
                // throw;
            }


            //Clear unknown files in rootfolder

            deleteFiles = Directory.GetFiles(rootDir, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string delFile in deleteFiles)
            {
                try
                {
                    var fName = Path.GetFileName(delFile);
                    if(!(fName=="ReadMe.lnk"|| fName=="ReadMe.url"))
                    {
                        File.Delete(delFile);
                        Global.LogWarn("File DELETED: " + delFile);

                    }
                }
                catch (Exception err)
                {
                    Global.LogError(err.Message);
                }
            }


            //add readme link
            var readMe = Path.Combine(rootDir, "ReadMe.url");
            if (!File.Exists(readMe))
            {
                var curDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                File.Copy(curDir + "\\ReadMe.url", readMe);

            }



        }
        public static List<FileInfo> FindImages(int i)
        {

            //1. //rgalvez folder Set CURRENT USER folder 
            List<FileInfo> imageFiles = new List<FileInfo>();

            if (SetCurrentUserFolder(i))
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

                  
                    foreach (var f in filesAll)
                    {
                        var fname = f.FullName;
                        var ext = Path.GetExtension(fname);
                        if (!ImageFileRestrictions.Contains(ext.ToLower()))
                        {
                            var filename = Path.GetFileName(fname).ToLower();
                            if (filename != "log.txt" && Path.GetExtension(filename) != ".csv")
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


        public static bool SetCurrentUserFolder(int i=0)
        {
            //SetStatus(ProcessStatus.Searching);
            var rootDir = Config.GetValue("FrontFolder");
            var dir = new DirectoryInfo(rootDir);
            //FileInfo[] files = dir.GetFiles().Take(10);
            var motherDir = new DirectoryInfo(rootDir);

            //sort
            var logList = motherDir.GetFiles("log.txt", SearchOption.AllDirectories).ToList();
            logList = logList.OrderBy(f => f.CreationTime).ToList();
            var currentLog = logList.FirstOrDefault();

            if (i >= logList.Count)
                currentLog = null;
            else if (logList.Count() > 0)
                currentLog = logList[i];

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
                Global.CurrentUser = string.Empty;
                Global.CurrentUserFolder = string.Empty;
                return false;
            }



        }

        public static bool MoveFile(FileInfo file, string toFileFullPath)
        {
            try
            {
                file.MoveTo(toFileFullPath);
                Agent.LogInfo("Copied to... " + toFileFullPath);

                return true;
            }
            catch (Exception)
            {
               
                Agent.LogInfo("Error move... " + toFileFullPath);
               return false;
            }
        }

        //public static bool UpdateLogFile( string logFileFullPath)
        //{
        //    try
        //    {
        //        file.MoveTo(logFileFullPath);
        //        Agent.LogInfo("Copied to... " + logFileFullPath);

        //        return true;
        //    }
        //    catch (Exception)
        //    {

        //        Agent.LogInfo("Error move... " + logFileFullPath);
        //        return false;
        //    }
        //}


        public static void DigForImageFiles(string dumpDir,int index)
        {
            var images = FileService.FindImages(index);
            string dumpFolder=string.Empty;
            if (!string.IsNullOrEmpty(Global.CurrentUser))
            {
                foreach (var image in images)
                {
                    var dir = image.DirectoryName;
                    string userImageFolderName = Path.GetFileName(dir);
                    dumpFolder = dumpDir + "\\" + Global.CurrentUser + "\\" + userImageFolderName;

                    if (!Directory.Exists(dumpFolder))
                        Directory.CreateDirectory(dumpFolder);

                    var dumpFullName = dumpFolder + "\\" + image.Name;
                    if (FileService.MoveFile(image, dumpFullName) == false)
                    {
                        //update log for error
                        //var log = image.DirectoryName + "log.txt";
                        //FileService.UpdateLogFile(log);
                        CreateLogText(dumpFolder);
                        //skip current userfolder
                        DigForImageFiles(dumpDir,index + 1);
                        break;
                    }
                }

                if(!string.IsNullOrEmpty(dumpFolder))
                        CreateLogText(dumpFolder);


                if(images.Count>0)
                {
                  if(!CheckIsScannerRunning())
                    {
                        Global.RunBaiRocs();
                    }
                }

            }
            else
            {
                Global.IdleCount -= 1;
                Agent.LogWarn("Idle status---" + Global.IdleCount.ToString());
                if(Global.IdleCount <= 0)
                {
                   if(! CheckIsScannerRunning())
                    {
                        Global.IdleCount = Global.IdleCountSet;
                        //scann dump file
                        Global.RunBaiRocs();
                    }
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

        public static bool CheckIsScannerRunning()
        {
            try
            {
                #region --------------------TRY CONTENT----------------------
                //string dumpDir = FileService.Config.GetValue("RawImageFolder");//dump folder in bairocs scannner
                string dumpDir = FileService.Config.GetValue("DumpFolder");//dump folder bairocs agent

                string fMesage = dumpDir + "\\running.txt";
                if (File.Exists(fMesage))
                {
                    //File.Delete(fMesage);
                    FileInfo f = new FileInfo(fMesage);
                    f.Delete();
                }

                //check for 3 sec
                var limit = DateTime.Now.AddSeconds(3);
                while (DateTime.Now < limit)
                {
                    ConsoleSpinner.Instance.Update();
                    //AsciiArt.Draw();
                    //Task.Delay(100);
                }

                if (File.Exists(fMesage))
                {
                    return true;
                }
                else
                   return false;

                #endregion
            }
            catch (Exception err)
            {

                Global.LogError("FileMesssageActivity---> " + err.Message);
                return true;
            }


        }

    }

}
