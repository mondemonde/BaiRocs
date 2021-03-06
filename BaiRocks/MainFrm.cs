﻿using BaiRocs.Commands;
using BaiRocs.Common;
using BaiRocs.DAL;
using BaiRocs.Models;
using BaiRocs.Policy;
using BaiRocs.Services;
using BaiRocs.WF;
using DotBits.Configuration;
using LogApplication.Common;
using LogApplication.Common.Config;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiRocs
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();

            var config = new DotBits.Configuration.ConfigUserControl();
            config.Dock = DockStyle.Fill;
            tabConfig.Controls.Add(config);

        }
        public ConfigManager MyConfig { get; set; }

        private void MainFrm_Load(object sender, EventArgs e)
        {

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            Global.LogInfo("Running Version: " + version);

            //STEP 1
            Global.MainWindow = this;
            backgroundWorker1.RunWorkerAsync();
            Thread.Sleep(1000);

            chkDelImage.Checked = Convert.ToBoolean(Convert.ToInt32(Global.Config.GetValue("AutoDeleteImage")));

            comboBoxDim.DataSource = MyReceiptMetaData.ListOfDimensions;
            MyReceiptMetaData.NatureOfTnx = new Dictionary<string, int> {
                {"Parking fee",0 },
                { "Gasoline",0},
                { "Transportation fare/Grab/Uber",0},
                {"Hotel accomodation/lodging",0},
                { "Meal - Others",0},
              //  {"Grocery(food items)",0},
                {"Repairs and Maintenance",0},
                {"Telephone /Mobile billing",0},
                {"Beauty and wellness",0},
                {"Home appliances",0},
                {"Internet billing",0}

            };
            var myNature = MyReceiptMetaData.NatureOfTnx.OrderBy(i => i.Value);

            cbDetailFactor.DataSource = new BindingSource(myNature, null);
            cbDetailFactor.DisplayMember = "Key";
            cbDetailFactor.ValueMember = "Key";

            Global.MakeScanIdle = chkIsScanBusy.Checked;
            SetStatus(ProcessStatus.Ready);

            if (Global.AutoRun)
                AutoRun();

        }

        #region --------------------------GLOBAL METHODS
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //STEP 2

            Global.ThisWF = new WorkflowApplication(new BackGroundActivity());
            //WFHostingApplication.HostWF.ThisWF = Global.ThisWF;


            // Notify when the workflow completes.
            ManualResetEvent completedEvent = new ManualResetEvent(false);
            Global.ThisWF.Completed += delegate (WorkflowApplicationCompletedEventArgs eWf)
            {
                completedEvent.Set();
            };

            // Run the workflow.
            Global.ThisWF.Run();

            // Get user input from the console and send it to the workflow instance.
            // This is done in a separate thread in order to not block current execution.
            ThreadPool.QueueUserWorkItem(Global.ReadEvent);

            // Wait until the workflow completes.
            completedEvent.WaitOne();

            Console.WriteLine("Workflow completed.  Waiting 3 seconds before exiting...");
            Thread.Sleep(3000);

        }


        public void DoTestSample(string message)
        {
            SetStatus(ProcessStatus.Ready);
        }










        #endregion -------------------------GLOBAL Methods

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //STEP 3
            Global.IsShuttingDown = false;

        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //TestCmd();
            List<BaiOcrLine> testLines = new List<BaiOcrLine>();
            BaiOcrLine ocr = new BaiOcrLine
            {

                Content="Date:10/11/19 TIN:123456789 address: 154 bisalao st. Maligay Nov Q.C. Vendor: SEven eleven"

            };
            testLines.Add(ocr);

            BaiOcrLine ocr1 = new BaiOcrLine
            {

                Content = "TOTAL:100 CHANGE:123 Cash: 500"

            };

            testLines.Add(ocr1);

            List<BaiOcrLine> result = new List<BaiOcrLine>();
            foreach(var thisOcr in testLines)
            {
                RocsTextService.RefineOcr(thisOcr,ref result);

            }
            int i = 0;
            foreach(BaiOcrLine item in result)
            {
                i += 1;
                item.LineNo = i;
                Console.WriteLine(string.Format("{0}. {1}", item.LineNo.ToString(), item.Content));
            }

        }

        void TestCmd()
        {
            var p = new TestSampleCmdParam(btnTest);
            p.CommandName = "TestSample";
            p.SampleMessage = "Hello MOnd!!";
            p.SavePayload();

            Global.ReadEventCmdParam(p);

            //config
            ConfigEditor config = new ConfigEditor();
            config.ConfigFilename = ConfigManager.MyConfigPath; //AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            config.ShowDialog();
            //this is necessary to update cache values
            ConfigurationManager.RefreshSection("appSettings");
        }


        public void InvokeOnUiThread(Action action)
        {
            if (InvokeRequired)
            {
                BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        public bool SetCurrentUserFolder()
        {
            //SetStatus(ProcessStatus.Searching);
            var rootDir = Global.Config.GetValue("RawImageFolder");
            var dir = new DirectoryInfo(rootDir);
            //FileInfo[] files = dir.GetFiles().Take(10);
            var motherDir = new DirectoryInfo(rootDir);

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

        private void chkIsScanBusy_CheckedChanged(object sender, EventArgs e)
        {
            Global.MakeScanIdle = chkIsScanBusy.Checked;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            Global.HasNothingToConvert = false;
            SetStatus(ProcessStatus.Searching);

            FileService.CleanRootFolder();
            var imageFiles = FileService.FindImages();

            //get last count
            int iniRecord =  bindingSource1.Count;

            bindingSource1.DataSource = imageFiles.ToList();
            dgFiles.DataSource = bindingSource1;

         
            if (iniRecord<=0)
            {
                var stat = BaiRocService.GetEngineStat();
                if (imageFiles.Count > 0)
                {
                    stat.Status = "Scanning";

                    if(stat.MaxValue>0)
                     stat.LastBatchCount = stat.MaxValue;

                    stat.MaxValue = imageFiles.Count;
                    stat.Value = 1;
                    stat.LastStart = DateTime.Now;

                    var imageFolder = Path.GetFileNameWithoutExtension(Global.CurrentImageFolder);
                    stat.CurrentFolder = Global.CurrentUser + "/" + imageFolder;                     
                    BaiRocService.UpdateEngine(stat);
                }
                if (stat.Value >= stat.MaxValue)
                {
                    stat.Status = "Idle";
                    //stat.LastBatchCount = stat.MaxValue;
                    //stat.TotalConvert += stat.MaxValue;
                    //stat.MaxValue = 0;
                    //stat.Value = 0;
                    BaiRocService.UpdateEngine(stat);
                }
            }
            else
            {
                var stat = BaiRocService.GetEngineStat();
                stat.Value += 1;
                BaiRocService.UpdateEngine(stat);
            }           
            SetStatus(ProcessStatus.Ready);
        }

        private void btnChekFile_Click(object sender, EventArgs e)
        {
            WaitForReady();
            SetStatus(ProcessStatus.Scanning);

            //if no more image file in the current folder...
            if (bindingSource1.Current == null || bindingSource1.Count == 0)
            {
                //bool isbusy = true;
                //while (isbusy == true)
                //{

                //}
                try
                {
                    // Global.CurrentFolder.LastAccessTime = DateTime.Now;
                    //var filename = Path.Combine(Global.CurrentUserFolder.FullName, "log.txt");
                    //File.AppendAllText(filename, "checked: " + DateTime.Now.ToShortDateString()
                    //    + " " + DateTime.Now.ToLongTimeString() + Environment.NewLine);

                    if (!string.IsNullOrEmpty(Global.CurrentImageFolder))
                    {
                        var filename2 = Path.Combine(Global.CurrentImageFolder, "done.txt");
                        var stream = File.Create(filename2);
                        stream.Close();
                        Global.CurrentImageFolder = string.Empty;
                        Thread.Sleep(100);
                    }


                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }

                SetStatus(ProcessStatus.Ready);
                return;
            }


            FileInfo current = (FileInfo)bindingSource1.Current;
            List<string> restrictions = new List<string>
                {
                    ".bmp",
                    ".png",
                    ".jpg"
                };


            var fname = current.FullName;// context.GetValue(FilePath);

            var ext = Path.GetExtension(fname);
            if (!restrictions.Contains(ext.ToLower()))
            {
                if (Path.GetFileName(fname) != "log.txt")
                {
                    File.Delete(fname);
                    Global.LogError("File Deleted..." + fname);
                    bindingSource1.RemoveCurrent();
                }

            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("------------Ready for scan..." + fname);
                //Do OCR here..

            }

            SetStatus(ProcessStatus.Ready);
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            WaitForReady();
            SetStatus(ProcessStatus.Scanning);
            FileInfo current = (FileInfo)bindingSource1.Current;
            Global.CurrentImagePath = current.FullName;
            if (current == null)
            {
                Global.HasNothingToConvert = true;
                return;
            }

            Global.OcrLines = new List<BaiOcrLine>();
            ScanImageCmdParam p = new ScanImageCmdParam();
            p.FileFullPath = current.FullName;
            p.SavePayload();
            Global.ReadEventCmdParam(p);

            while (Global.ProcessStatus == ProcessStatus.Scanning.ToString())
            {

                ConsoleSpinner.Instance.Update();
                //AsciiArt.Draw();
                Task.Delay(100);
                Application.DoEvents();
            }

            AzureSvc_OnReadDone(sender, e);

            //if (Global.ProcessStatus == ProcessStatus.Ready.ToString())
            //{
            //    bindingSourceOCR.DataSource = Global.OcrLines;
            //    dgOCR.DataSource = bindingSourceOCR;
            //    SetStatus(ProcessStatus.Ready);
            //}


        }

        private void AzureSvc_OnReadDone(object sender, EventArgs e)
        {
           

            if (Global.ProcessStatus == ProcessStatus.Error.ToString())
            {
                //var err = (Exception)sender;
                Global.LogError("Error on AZURE.");

            }
            else if (Global.OcrLines.Count < 15)
            {
                //delete imagerow if not receipt
                File.Delete(Global.CurrentImagePath);
                bindingSource1.Remove(bindingSource1.Current);

                Global.LogError("Error on Scan... NOT A RECEIPT");
                Global.ProcessStatus = ProcessStatus.Error.ToString();
            }
            else
            {
                //delete imagerow
                File.Delete(Global.CurrentImagePath);
                bindingSource1.Remove(bindingSource1.Current);

                //refine OCR
                List<BaiOcrLine> result = new List<BaiOcrLine>();
                foreach (var thisOcr in Global.OcrLines)
                {
                    RocsTextService.RefineOcr(thisOcr, ref result);

                }
                int i = 0;
                foreach (BaiOcrLine item in result)
                {
                    i += 1;
                    item.LineNo = i;
                    Console.WriteLine(string.Format("{0}. {1}", item.LineNo.ToString(), item.Content));
                }

                Global.OcrLines = result;


                /////////////////////
                //update ocr list
                bindingSourceOCR.DataSource = Global.OcrLines;
                dgOCR.DataSource = bindingSourceOCR;
                Application.DoEvents();
                SetStatus(ProcessStatus.Ready);
            }

        }

        public void SetStatus(ProcessStatus status)
        {
            ssMsg.Text = status.ToString();
            Global.ProcessStatus = status.ToString();
            Application.DoEvents();
        }


        private void btnWeight_Click(object sender, EventArgs e)
        {
            WaitForReady();
            SetStatus(ProcessStatus.Weighing);
            for (int i = 0; i < Global.OcrLines.Count; i++)
            {
                var ocr = Global.OcrLines[i];
                OcrPolicy.AssertAsVendorName(ref ocr);
                OcrPolicy.AssertAsAddress(ref ocr);
                OcrPolicy.AssertAsDateTitle(ref ocr);
                //OcrPolicy.AssertAsDetail(ref ocr);

                OcrPolicy.AssertAsChangeTitle(ref ocr);
                OcrPolicy.AssertAsTenderTitle(ref ocr);
                OcrPolicy.AssertAsTotalTitle(ref ocr);
                OcrPolicy.AssertAsVendorTINtitle(ref ocr);
            }

            //bindingSourceOCR.Clear();
            // Application.DoEvents();

            bindingSourceOCR.DataSource = Global.OcrLines;
            BindingNavigatorOcr.BindingSource = bindingSourceOCR;
            dgOCR.DataSource = bindingSourceOCR;
            dgOCR.Refresh();

            SetStatus(ProcessStatus.Ready);
        }

        private void btnSigma_Click(object sender, EventArgs e)
        {
            WaitForReady();
            SetStatus(ProcessStatus.Sigma);

            Global.CurrentSigma = new Models.Sigma();
            foreach (var ocr in Global.OcrLines)
            {
                if (string.IsNullOrEmpty(ocr.ElectedAs))
                {
                    Global.CurrentSigma.SigmaAddress.Add(Convert.ToDouble(ocr.WeightedAsAddress));
                    Global.CurrentSigma.SigmaDateTitle.Add(Convert.ToDouble(ocr.WeightedAsDateTitle));
                    Global.CurrentSigma.SigmaTotalTitle.Add(Convert.ToDouble(ocr.WeightedAsTotalTitle));
                    Global.CurrentSigma.SigmaVendorName.Add(Convert.ToDouble(ocr.WeightedAsVendorName));
                    Global.CurrentSigma.SigmaVendorTINTitle.Add(Convert.ToDouble(ocr.WeightedAsVendorTINTitle));
                    Global.CurrentSigma.SigmaChangeTitle.Add(Convert.ToDouble(ocr.WeightedAsChangeTitle));
                    Global.CurrentSigma.SigmaTenderTitle.Add(Convert.ToDouble(ocr.WeightedAsTenderTitle));

                }
            }

            var list = Global.CurrentSigma.GetSigmasDesc();
            bindingSourceSigma.DataSource = list;
            bindingNavigatorSigma.BindingSource = bindingSourceSigma;
            dgSigma.DataSource = bindingSourceSigma;
            dgSigma.Refresh();

            SetStatus(ProcessStatus.Ready);
        }

        private void btnElect1_Click(object sender, EventArgs e)
        {
            WaitForReady();
            SetStatus(ProcessStatus.Election);


            var sigmas = Global.CurrentSigma.GetSigmasDesc();

            foreach (var s in sigmas)
            {
                if (s.Value >= 2.5)//3 sigma accuracy
                {
                    if (Enum.TryParse(s.Key, out ReceiptParts part))
                        RocsTextService.ElectOcrLineBySigmaDeferTotalTitle(part);

                }
            }
            bindingSourceElect1.DataSource = Global.OcrLines;
            bindingNavigatorElect1.BindingSource = bindingSourceElect1;
            dgElection1.DataSource = bindingSourceElect1;
            dgElection1.Refresh();

            SetStatus(ProcessStatus.Ready);

        }

        private void comboBoxDim_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dim = comboBoxDim.Text;
            using (MyDBContext db = new MyDBContext())
            {
                var wfactors = db.WeightFactors.ToList();
                var factors = wfactors.Where(f => f.Dimension == dim);
                bindingSourceWeight.DataSource = factors;
                bindingNavigatorWeight.BindingSource = bindingSourceWeight;
                dgWeight.DataSource = bindingSourceWeight;
            }



        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            //delete
            if (MessageBox.Show("Confirm delete?", "Warning:"
                , MessageBoxButtons.OKCancel
                , MessageBoxIcon.Warning) == DialogResult.OK)
            {
                using (MyDBContext db = new MyDBContext())
                {
                    var f = (WeightFactor)bindingSourceWeight.Current;
                    db.Entry(f).State = System.Data.Entity.EntityState.Deleted;
                    db.WeightFactors.Remove(f);
                    db.SaveChanges();
                }

                bindingSourceWeight.RemoveCurrent();

            }
        }

        private void btnAddWeight_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtToFind.Text))
                return;

            using (MyDBContext db = new MyDBContext())
            {

                db.WeightFactors.Add(new WeightFactor
                {
                    Weight = Convert.ToInt32(txtWeight.Value),
                    keyWord = txtToFind.Text,
                    Dimension = comboBoxDim.Text

                });
                db.SaveChanges();

                comboBoxDim_SelectedIndexChanged(sender, e);
                bindingSourceWeight.MoveLast();
            }

        }


        private void bindingSourceWeight_CurrentChanged(object sender, EventArgs e)
        {
            try
            {
                var current = (WeightFactor)bindingSourceWeight.Current;
                txtToFind.Text = current.keyWord;
                txtWeight.Value = current.Weight;
            }
            catch (Exception)
            {

                //throw;
            }

        }

        private void buttonUpdateWeight_Click(object sender, EventArgs e)
        {
            WeightFactor cur = (WeightFactor)bindingSourceWeight.Current;
            using (MyDBContext db = new MyDBContext())
            {
                var thisF = db.WeightFactors.Single(f => f.Id == cur.Id);
                thisF.Weight = Convert.ToInt32(txtWeight.Value);
                thisF.keyWord = txtToFind.Text;
                thisF.Dimension = comboBoxDim.Text;
                //db.WeightFactors.Attach(cur);               
                db.SaveChanges();

                comboBoxDim_SelectedIndexChanged(sender, e);

            }
        }

        private void btn2ndElection_Click(object sender, EventArgs e)
        {


            //get amounttitle now
            var ocrTender = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.AmountTenderTiTle.ToString()).FirstOrDefault();
            if (ocrTender != null && ocrTender.ElectedAs == ReceiptParts.AmountTenderTiTle.ToString())
            {

                for (int i = ocrTender.LineNo; i > 5; i--)
                {
                    if ((Global.OcrLines[i].Content.ToLower().Contains("total")
                        || Global.OcrLines[i].Content.ToLower().Contains("price")) &&
                         !Global.OcrLines[i].Content.ToLower().Contains("sub"))
                    {
                        Global.OcrLines[i].WeightedAsTotalTitle += 7;
                        break;
                    }
                }

            }
            else
            {
                var ocrChange = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.ChangeTitle.ToString()).FirstOrDefault();
                if (ocrChange != null && ocrChange.ElectedAs == ReceiptParts.ChangeTitle.ToString())
                {

                    for (int i = ocrChange.LineNo; i > 5; i--)
                    {
                        if ((Global.OcrLines[i].Content.ToLower().Contains("total")
                        || Global.OcrLines[i].Content.ToLower().Contains("price")) &&
                         !Global.OcrLines[i].Content.ToLower().Contains("sub"))
                        {
                            Global.OcrLines[i].WeightedAsTotalTitle += 7;
                            break;
                        }
                    }

                }


            }

            btnSigma_Click(sender, e);

            var sigmas = Global.CurrentSigma.GetSigmasDesc();

            foreach (var s in sigmas)
            {
                if (s.Value >= 2.5)//3 sigma accuracy
                {
                    if (Enum.TryParse(s.Key, out ReceiptParts part))
                        RocsTextService.ElectOcrLineBySigmaWithTotalTitle(part);

                }
            }
            bindingSourceElect1.DataSource = Global.OcrLines;
            bindingNavigatorElect1.BindingSource = bindingSourceElect1;
            dgElection1.DataSource = bindingSourceElect1;
            dgElection1.Refresh();

        }

        private void btn3rdElection_Click(object sender, EventArgs e)
        {

            //final resort if no result
            var qParts = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.DateTitle.ToString()).ToList();
            if (qParts.Count == 0)
            {
                qParts = Global.OcrLines.Where(x => string.IsNullOrEmpty(x.ElectedAs))
                    .OrderBy(o => o.WeightedAsDateTitle).ToList();
                var ocr = qParts.Last();

                ocr.ElectedAs = ReceiptParts.DateTitle.ToString();
            }

            qParts = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.VendorName.ToString()).ToList();
            if (qParts.Count == 0)
            {
                qParts = Global.OcrLines.Where(x => string.IsNullOrEmpty(x.ElectedAs))
                    .OrderBy(o => o.WeightedAsVendorName).ToList();
                var ocr = qParts.Last();
                ocr.ElectedAs = ReceiptParts.VendorName.ToString();
            }

            qParts = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.VendorTINTitle.ToString()).ToList();
            if (qParts.Count == 0)
            {
                qParts = Global.OcrLines.Where(x => string.IsNullOrEmpty(x.ElectedAs))
                    .OrderBy(o => o.WeightedAsVendorTINTitle).ToList();
                var ocr = qParts.Last();
                ocr.ElectedAs = ReceiptParts.VendorTINTitle.ToString();
            }

            qParts = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.PriceTitle.ToString()).ToList();
            if (qParts.Count == 0)
            {
                qParts = Global.OcrLines.Where(x => string.IsNullOrEmpty(x.ElectedAs))
                    .OrderBy(o => o.WeightedAsTotalTitle).ToList();
                var ocr = qParts.Last();
                ocr.ElectedAs = ReceiptParts.PriceTitle.ToString();
            }


            qParts = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.Address.ToString()).ToList();
            if (qParts.Count == 0)
            {
                qParts = Global.OcrLines.Where(x => string.IsNullOrEmpty(x.ElectedAs))
                    .OrderBy(o => o.WeightedAsAddress).ToList();
                var ocr = qParts.Last();
                ocr.ElectedAs = ReceiptParts.Address.ToString();
            }

            //foreach(BaiOcrLine ocr in Global.OcrLines)
            //{

            //}

        }

        private void btnCreateReceipt_Click(object sender, EventArgs e)
        {

            WaitForReady();
            SetStatus(ProcessStatus.Election);


            List<Receipt> table = new List<Receipt>();
            string user = Path.GetFileName(Global.CurrentUserFolder);
            Global.CurrentReciept = new Receipt
            {
                UserFolder = user
            };

            //vendorname
            RocsTextService.GetReceiptVendorName();
            RocsTextService.GetReceiptVendorAddress();

            //TIN,Date, total etc..
            foreach (BaiOcrLine ocr in Global.OcrLines)
            {
                if (ocr.ElectedAs == ReceiptParts.DateTitle.ToString())
                {
                    RocsTextService.GetReceiptDateValue(ocr.LineNo);
                }
                else if (ocr.ElectedAs == ReceiptParts.VendorTINTitle.ToString())
                {
                    RocsTextService.GetReceiptTINValue(ocr);
                }
                else if (ocr.ElectedAs == ReceiptParts.PriceTitle.ToString())
                {
                    RocsTextService.GetReceiptTotalValue(ocr);
                }

            }

            //last call
            var year = DateTime.Now.Date.Year;
            string strYear = year.ToString();
            //var y2 ="/" + strYear.Substring(2, 2);
            var y19 =strYear.Substring(2, 2);

            if (string.IsNullOrEmpty(Global.CurrentReciept.Date) || !Global.CurrentReciept.Date.Contains(y19))
            //|| !Global.CurrentReciept.Date.Contains(strYear) || !Global.CurrentReciept.Date.Contains(y2))
            {
                RocsTextService.GetDateByYear();
            }


            //TOTAL amount
            if (string.IsNullOrEmpty(Global.CurrentReciept.Amount)
                || Convert.ToDouble(RocsTextService.RefineToMoney(Global.CurrentReciept.Amount))>3000)
            //|| !Global.CurrentReciept.Date.Contains(strYear) || !Global.CurrentReciept.Date.Contains(y2))
            {
               RocsTextService.GetAmountByDifference();
            }

            table.Add(Global.CurrentReciept);
            receiptBindingSource.DataSource = table;
            dgReceipts.DataSource = receiptBindingSource;
            this.bindingNavigatorReceipts.BindingSource = receiptBindingSource;

            SetStatus(ProcessStatus.Ready);

        }

        private void btnDetails_Click(object sender, EventArgs e)
        {

            MyReceiptMetaData.NatureOfTnx = new Dictionary<string, int> {
                {"Parking fee",0 },
                { "Gasoline",0},
                { "Transportation fare/Grab/Uber",0},
                {"Hotel accomodation/lodging",0},
                { "Meal - Others",0},
              //  {"Grocery(food items)",0},
                {"Repairs and Maintenance",0},
                {"Telephone /Mobile billing",0},
                {"Beauty and wellness",0},
                {"Home appliances",0},
                {"Internet billing",0}

            };


            //details
            using (MyDBContext db = new MyDBContext())
            {

                var clone = new Dictionary<string, int>(MyReceiptMetaData.NatureOfTnx);

                foreach (var detail in clone)
                {
                    var factors = db.WeightFactorForDetails.Where(f => f.NatureOfTnx == detail.Key);
                    foreach (BaiOcrLine ocr in Global.OcrLines)
                    {
                        foreach (var factor in factors)
                        {
                            if (ocr.Content.ToLower().Contains(factor.keyWord.ToLower()))
                            {
                                MyReceiptMetaData.NatureOfTnx[detail.Key] += factor.Weight;

                            }
                        }//for factor
                    }//for ocrline
                }  //for detail             

            }  //db      

            //get max value
            var q = MyReceiptMetaData.NatureOfTnx.OrderBy(f => f.Value).Last();


            Global.CurrentReciept.Description = q.Key;

            List<Receipt> table = new List<Receipt>();
            table.Add(Global.CurrentReciept);
            receiptBindingSource.DataSource = table;
            dgReceipts.DataSource = receiptBindingSource;
            this.bindingNavigatorReceipts.BindingSource = receiptBindingSource;


        }

#region EXTENDED ELECTION




#endregion

        private void cbDetailFactor_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dim = cbDetailFactor.Text;
            using (MyDBContext db = new MyDBContext())
            {
                var wfactors = db.WeightFactorForDetails.ToList();
                var factors = wfactors.Where(f => f.NatureOfTnx == dim).OrderBy(f => f.keyWord);
                bindingSourceDetail.DataSource = factors;
                bindingNavigatorDetail.BindingSource = bindingSourceDetail;
                dgDetail.DataSource = bindingSourceDetail;
            }


        }

        private void btnWeightDetail_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtToFind.Text))
                return;

            using (MyDBContext db = new MyDBContext())
            {

                db.WeightFactorForDetails.Add(new WeightFactorForDetail
                {
                    Weight = Convert.ToInt32(numericUpDownDetail.Value),
                    keyWord = txtKeyWordDetail.Text,
                    NatureOfTnx = cbDetailFactor.Text

                });
                db.SaveChanges();

                cbDetailFactor_SelectedIndexChanged(sender, e);
                bindingSourceDetail.MoveLast();
            }

        }

        private void btnUpdateDetail_Click(object sender, EventArgs e)
        {
            WeightFactorForDetail cur = (WeightFactorForDetail)bindingSourceDetail.Current;
            using (MyDBContext db = new MyDBContext())
            {
                var thisF = db.WeightFactorForDetails.Single(f => f.Id == cur.Id);
                thisF.Weight = Convert.ToInt32(numericUpDownDetail.Value);
                thisF.keyWord = txtToFind.Text;
                thisF.NatureOfTnx = cbDetailFactor.Text;
                //db.WeightFactors.Attach(cur);               
                db.SaveChanges();

                cbDetailFactor_SelectedIndexChanged(sender, e);
            }
        }

        private void bindingSourceDetail_CurrentChanged(object sender, EventArgs e)
        {
            try
            {
                var current = (WeightFactorForDetail)bindingSourceDetail.Current;
                txtKeyWordDetail.Text = current.keyWord;
                numericUpDownDetail.Value = current.Weight;
            }
            catch (Exception)
            {

                //throw;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //save to db
            if (string.IsNullOrEmpty(txtToFind.Text))
                return;

            var allOcr = string.Empty;
            foreach (var ocr in Global.OcrLines)
            {
                allOcr += ocr.Content + "^`^";
            }

            var receipt = (Receipt)receiptBindingSource.Current;
            receipt.OcrLines = allOcr;

            using (MyReceiptOnlyContext db = new MyReceiptOnlyContext())
            {
                db.TableReceipts.Add(receipt);
                db.SaveChanges();

            }

            //if (chkDelImage.Checked)
            //{
            //    FileInfo current = (FileInfo)bindingSource1.Current;
            //    var fname = current.FullName;// context.GetValue(FilePath);
            //    File.Delete(fname);
            //    Global.LogWarn("File Deleted..." + fname);
            //}


        }


        void SaveDataGridViewToCSV(string filename)
        {
            FileService.ClearCSVFiles(Path.GetDirectoryName(filename));

            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);

                // Choose whether to write header. Use EnableWithoutHeaderText instead to omit header.
                dgCSV.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                // Select all the cells
                dgCSV.SelectAll();
                // Copy selected cells to DataObject
                DataObject dataObject = dgCSV.GetClipboardContent();
                // Get the text of the DataObject, and serialize it to a file
                File.WriteAllText(filename, dataObject.GetText(TextDataFormat.CommaSeparatedValue));

                //try to copy to Front Folder


            }
            catch (Exception ex)
            {

                Global.LogError(ex.Message);
            }

        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            string csvName = "receipt";
            using (MyReceiptOnlyContext db = new MyReceiptOnlyContext())
            {
                var q = db.TableReceipts.Where(r => r.UserFolder == Global.CurrentUser);
                var list = q.OrderBy(r => r.Created).ToList();

                if (list.Count>0)
                {
                    var cnt = list.Count;
                    csvName +=  cnt.ToString();
                }

                this.bindingSourceCSV.DataSource = list;
                dgCSV.DataSource = bindingSourceCSV;
                this.bindingNavigatorCSV.BindingSource = bindingSourceCSV;

            }

            //export to csv

            var folder = Global.Config.GetValue("FrontFolder") + "\\" + Global.CurrentUser; //Global.CurrentUserFolder;
            var file =csvName + ".csv";
            var path = Path.Combine(folder, file);
            SaveDataGridViewToCSV(path);

            var stat = BaiRocService.GetEngineStat();

            if(stat.Value >=stat.MaxValue)
            {
                stat.Status = "Idle";
                stat.LastBatchCount = stat.MaxValue;
                stat.TotalConvert += stat.MaxValue;
                stat.MaxValue = 0;
                stat.Value = 0;
                BaiRocService.UpdateEngine(stat);

            }



        }

        public void AutoRun()
        {
            Global.LogWarn("Autorun started.");
            btnAutoRun_Click(this, EventArgs.Empty);

        }
        int shutdown = 4;

        private void btnAutoRun_Click(object sender, EventArgs e)
        {
            chkIsScanBusy.Checked = false;
            Global.MakeScanIdle = chkIsScanBusy.Checked;

            while (Global.MakeScanIdle == false)
            {
                //try
                //{
                btnSearch_Click(sender, e);

               


                btnChekFile_Click(sender, e);
                if (Global.HasNothingToConvert == true)
                {
                    Application.DoEvents();
                    var limit = DateTime.Now.AddSeconds(10);
                    shutdown -= 1;
                    Console.WriteLine("Shutsdown in ..." + shutdown.ToString());

                    if (shutdown < 1)
                    {
                        Global.IsShuttingDown = true;
                        //this.Close();
                       // System.Windows.Forms.Application.Exit();
                       System.Environment.Exit(0);

                    }

                    while (DateTime.Now < limit)
                    {
                        ConsoleSpinner.Instance.Update();
                        //AsciiArt.Draw();
                        Task.Delay(100);
                        Application.DoEvents();
                    }
                    //Task.Delay(5000);
                    break;
                }
                if (bindingSource1.Current == null)
                {
                    break;
                }

                btnScan_Click(sender, e);
                if (Global.ProcessStatus == ProcessStatus.Error.ToString())
                {
                    Global.ProcessStatus = ProcessStatus.Ready.ToString();
                    break;
                }

                if (Global.HasNothingToConvert)
                {
                   var stat =  BaiRocService.GetEngineStat();
                    stat.Status = "Idle";
                    stat.MaxValue = 0;
                    stat.Value = 0;
                    BaiRocService.UpdateEngine(stat);
                    break;
                }

                if (Global.OcrLines == null)
                    break;

                //keep running..
                shutdown = 4;            
                try
                {
                    btnWeight_Click(sender, e);
                    btnSigma_Click(sender, e);
                    btnElect1_Click(sender, e);
                    btn2ndElection_Click(sender, e);
                    btn3rdElection_Click(sender, e);
                    btnCreateReceipt_Click(sender, e);
                    btnDetails_Click(sender, e);
                }
                catch (Exception)
                {

                    RecoverFromError1(sender, e);
                }

                try
                {
                   
                    btnAuthenticatedSet_Click(sender, e);
                    btnUserSet_Click(sender, e);
                    btnSave_Click(sender, e);
                    btnExcel_Click(sender, e);
                }
                catch (Exception)
                {
                    RecoverFromError3(sender, e);
                }

                Application.DoEvents();
            }
            if (Global.MakeScanIdle == false)
                btnAutoRun_Click(sender, e);

        }

        public void RecoverFromError1(object sender, EventArgs e)
        {
            //wait a minute 
            var limit = DateTime.Now.AddSeconds(5);
            while (DateTime.Now < limit)
            {
                ConsoleSpinner.Instance.Update();
                //AsciiArt.Draw();
                Task.Delay(100);
                Application.DoEvents();
            }

            try
            {
                btnWeight_Click(sender, e);
                btnSigma_Click(sender, e);
                btnElect1_Click(sender, e);
                btn2ndElection_Click(sender, e);
                btn3rdElection_Click(sender, e);
                btnCreateReceipt_Click(sender, e);
                btnDetails_Click(sender, e);
                btnAuthenticatedSet_Click(sender, e);
                btnUserSet_Click(sender, e);
                btnSave_Click(sender, e);
                btnExcel_Click(sender, e);
            }
            catch (Exception)
            {
                RecoverFromError2(sender, e);
            }
           

        }
        public void RecoverFromError2(object sender, EventArgs e)
        {
            //wait a minute 
            var limit = DateTime.Now.AddSeconds(10);
            while (DateTime.Now < limit)
            {
                ConsoleSpinner.Instance.Update();
                //AsciiArt.Draw();
                Task.Delay(100);
                Application.DoEvents();
            }
            try
            {
                btnWeight_Click(sender, e);
                btnSigma_Click(sender, e);
                btnElect1_Click(sender, e);
                btn2ndElection_Click(sender, e);
                btn3rdElection_Click(sender, e);
                btnCreateReceipt_Click(sender, e);
                btnDetails_Click(sender, e);
                btnAuthenticatedSet_Click(sender, e);
                btnUserSet_Click(sender, e);
                btnSave_Click(sender, e);
                btnExcel_Click(sender, e);
            }
            catch (Exception err)
            {
                Global.LogError(err);
            }


        }

        public void RecoverFromError3(object sender, EventArgs e)
        {
            //wait a minute 
            var limit = DateTime.Now.AddSeconds(5);
            while (DateTime.Now < limit)
            {
                ConsoleSpinner.Instance.Update();
                //AsciiArt.Draw();
                Task.Delay(100);
                Application.DoEvents();
            }

            try
            {
               
                btnAuthenticatedSet_Click(sender, e);
                btnUserSet_Click(sender, e);
                btnSave_Click(sender, e);
                btnExcel_Click(sender, e);
            }
            catch (Exception)
            {
                RecoverFromError4(sender, e);
            }
        }
        public void RecoverFromError4(object sender, EventArgs e)
        {
            //wait a minute 
            var limit = DateTime.Now.AddSeconds(10);
            while (DateTime.Now < limit)
            {
                ConsoleSpinner.Instance.Update();
                //AsciiArt.Draw();
                Task.Delay(100);
                Application.DoEvents();
            }
            try
            {
               
                btnAuthenticatedSet_Click(sender, e);
                btnUserSet_Click(sender, e);
                btnSave_Click(sender, e);
                btnExcel_Click(sender, e);
            }
            catch (Exception err)
            {
                Global.LogError(err);
            }


        }


        void WaitForReady()
        {
            while (Global.ProcessStatus != ProcessStatus.Ready.ToString())
            {
                Task.Delay(100);
                Application.DoEvents();
            }
        }

        private void btnClearDb_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to clear all records in the database?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (MyReceiptOnlyContext db = new MyReceiptOnlyContext())
                {
                    db.Database.ExecuteSqlCommand("DELETE FROM [Receipts]");

                    var q = db.TableReceipts.Where(r => r.UserFolder == Global.CurrentUser);
                    var list = q.OrderByDescending(r => r.Date).ToList();

                    this.bindingSourceCSV.DataSource = list;
                    dgCSV.DataSource = bindingSourceCSV;
                    this.bindingNavigatorCSV.BindingSource = bindingSourceCSV;

                }
            }
        }

        private void btnFeedback_Click(object sender, EventArgs e)
        {
            //TODO
            //using (MyDBContext db = new MyDBContext())
            //{
            //    var q = db.TableReceipts.Where(r => r.UserFolder == Global.CurrentUser);
            //    var list = q.OrderByDescending(r => r.Date).ToList();

            //    this.bindingSourceCSV.DataSource = list;
            //    dgCSV.DataSource = bindingSourceCSV;
            //    this.bindingNavigatorCSV.BindingSource = bindingSourceCSV;

            //}

            ////export to csv
            //var folder = Global.CurrentUserFolder;
            //var file = "receipt.csv";
            //var path = Path.Combine(folder, file);
            //SaveDataGridViewToCSV(path);
        }

        private void btnAuthenticatedSet_Click(object sender, EventArgs e)
        {
            WaitForReady();
            SetStatus(ProcessStatus.Election);
            List<Receipt> table = new List<Receipt>();

            var current = Global.CurrentReciept;
            var tinId = current.Tax_Identification;
            tinId = RocsTextService.RefineToPureNumber(tinId);


            using (MyReceiptOnlyContext db = new MyReceiptOnlyContext())
            {

                var q = db.TableReceiptFixes.Where(r => r.IsAuthenticated == true).ToList();

                List<ReceiptFix> listR = new List<ReceiptFix>();
                foreach(var r in q)
                {
                    // && RocsTextService.RefineToPureNumber(r.Tax_Identification) == tinId
                    if (RocsTextService.RefineToPureNumber(r.Tax_Identification) == tinId)
                    {
                        listR.Add(r);
                    }
                }

                var setting = listR.OrderByDescending(r => r.Modified).ToList().FirstOrDefault();

                if (listR.Count > 0)
                {
                    if ( setting.Id > 0)
                    {
                        current.Address = setting.Address;
                        current.Comapany_Name = setting.Comapany_Name;
                        current.Description = setting.Description;

                    }

                    Global.CurrentReciept = current;
                }
            }


            table.Add(Global.CurrentReciept);
            receiptBindingSource.DataSource = table;
            dgReceipts.DataSource = receiptBindingSource;
            this.bindingNavigatorReceipts.BindingSource = receiptBindingSource;

            SetStatus(ProcessStatus.Ready);
        }

        private void btnUserSet_Click(object sender, EventArgs e)
        {
            WaitForReady();
            SetStatus(ProcessStatus.Election);
            List<Receipt> table = new List<Receipt>();
            //string user = Path.GetFileName(Global.CurrentUserFolder);

            var current = Global.CurrentReciept;
            var tinId = current.Tax_Identification;
            tinId =RocsTextService.RefineToPureNumber(tinId);

            using (MyReceiptOnlyContext db = new MyReceiptOnlyContext())
            {

                var q = db.TableReceiptFixes.Where(r => r.UserFolder == Global.CurrentUser
                && r.Modified.HasValue );



                List<ReceiptFix> listR = new List<ReceiptFix>();
                foreach (var r in q)
                {
                    // && RocsTextService.RefineToPureNumber(r.Tax_Identification) == tinId
                    if (RocsTextService.RefineToPureNumber(r.Tax_Identification) == tinId)
                    {
                        listR.Add(r);
                    }
                }

                if (listR.Count > 0)
                {
                    var setting = listR.OrderByDescending(r => r.Modified).ToList().FirstOrDefault();
                    if ( setting.Id > 0)
                    {
                        current.Address = setting.Address;
                        current.Comapany_Name = setting.Comapany_Name;
                        current.Description = setting.Description;

                    }
                    Global.CurrentReciept = current;
                }
            }


            table.Add(Global.CurrentReciept);
            receiptBindingSource.DataSource = table;
            dgReceipts.DataSource = receiptBindingSource;
            this.bindingNavigatorReceipts.BindingSource = receiptBindingSource;

            SetStatus(ProcessStatus.Ready);
        }
    }
}