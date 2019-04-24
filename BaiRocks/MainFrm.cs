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
            //STEP 1
            Global.MainWindow = this;
            backgroundWorker1.RunWorkerAsync();
            Thread.Sleep(1000);

            chkDelImage.Checked =Convert.ToBoolean(Convert.ToInt32(Global.Config.GetValue("AutoDeleteImage")));

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


            SetStatus(ProcessStatus.Ready);
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

        private void chkIsScanBusy_CheckedChanged(object sender, EventArgs e)
        {
            Global.MakeScanIdle = chkIsScanBusy.Checked;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SetStatus(ProcessStatus.Searching);
            var rootDir = Global.Config.GetValue("RawImageFolder");
            //var dir = new DirectoryInfo(rootDir);
            //FileInfo[] files = dir.GetFiles().Take(10);
            var motherDir = new DirectoryInfo(rootDir);
            DirectoryInfo[] Folders = motherDir.GetDirectories("*", SearchOption.TopDirectoryOnly); //( Directory.getd(rootDir, "*.*", SearchOption.AllDirectories).Take(5);
            Global.CurrentFolder = Folders.OrderBy(f => f.LastAccessTime).FirstOrDefault();
            Global.LogWarn("Global.CurrentFolder -->" + Global.CurrentFolder.Name);
            Global.CurrentUser = Global.CurrentFolder.Name;


            //var files = Directory.GetFiles(Global.CurrentFolder.FullName);
            var files = Global.CurrentFolder.GetFiles();
            bindingSource1.DataSource = files.ToList();
            dgFiles.DataSource = bindingSource1;

        }

        private void btnChekFile_Click(object sender, EventArgs e)
        {
            SetStatus(ProcessStatus.Scanning);
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
            SetStatus(ProcessStatus.Scanning);
            FileInfo current = (FileInfo)bindingSource1.Current;
            //BaiRocService azureSvc = new BaiRocService();
            //azureSvc.OnReadDone += AzureSvc_OnReadDone;
            //var fname = current.FullName;
            //azureSvc.ReadImage(fname);


            ScanImageCmdParam p = new ScanImageCmdParam();
            p.FileFullPath = current.FullName;
            p.SavePayload();
            Global.ReadEventCmdParam(p);

            while (Global.ProcessStatus == ProcessStatus.Scanning.ToString())
            {

                ConsoleSpinner.Instance.Update();
                //AsciiArt.Draw();
                Task.Delay(100);
            }

            bindingSourceOCR.DataSource = Global.OcrLines;
            dgOCR.DataSource = bindingSourceOCR;
            SetStatus(ProcessStatus.Ready);


        }

        private void AzureSvc_OnReadDone(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            BaiRocService azureSvc = (BaiRocService)sender;
            Global.OcrLines = azureSvc.RawList;
            bindingSourceOCR.DataSource = Global.OcrLines;
            dgOCR.DataSource = bindingSourceOCR;

            SetStatus(ProcessStatus.Ready);
        }

        public void SetStatus(ProcessStatus status)
        {
            ssMsg.Text = status.ToString();
            Global.ProcessStatus = status.ToString();
            Application.DoEvents();
        }


        private void btnWeight_Click(object sender, EventArgs e)
        {
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


        }

        private void btnElect1_Click(object sender, EventArgs e)
        {
            var sigmas = Global.CurrentSigma.GetSigmasDesc();

            foreach (var s in sigmas)
            {
                if (s.Value >= 2.5)//3 sigma accuracy
                {
                    if (Enum.TryParse(s.Key, out ReceiptParts part))
                        RocsTextService.ElectOcrLineBySigma(part);

                }
            }
            bindingSourceElect1.DataSource = Global.OcrLines;
            bindingNavigatorElect1.BindingSource = bindingSourceElect1;
            dgElection1.DataSource = bindingSourceElect1;
            dgElection1.Refresh();
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

            btnSigma_Click(sender, e);
            btnElect1_Click(sender, e);

            //get amounttitle now
            var ocrTender = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.AmountTenderTiTle.ToString()).FirstOrDefault();
            if (ocrTender.ElectedAs == ReceiptParts.AmountTenderTiTle.ToString())
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
                if (ocrChange.ElectedAs == ReceiptParts.ChangeTitle.ToString())
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


            }



        }

        private void btn3rdElection_Click(object sender, EventArgs e)
        {
            btnSigma_Click(sender, e);
            btnElect1_Click(sender, e);




            //final resort if no result
            var qParts = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.DateTitle.ToString()).ToList();
            if (qParts.Count == 0)
            {
                qParts = Global.OcrLines.OrderBy(o => o.WeightedAsDateTitle).ToList();
                var ocr = qParts.Last();
                ocr.ElectedAs = ReceiptParts.DateTitle.ToString();
            }

            qParts = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.VendorName.ToString()).ToList();
            if (qParts.Count == 0)
            {
                qParts = Global.OcrLines.OrderBy(o => o.WeightedAsVendorName).ToList();
                var ocr = qParts.Last();
                ocr.ElectedAs = ReceiptParts.VendorName.ToString();
            }

            qParts = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.VendorTINTitle.ToString()).ToList();
            if (qParts.Count == 0)
            {
                qParts = Global.OcrLines.OrderBy(o => o.WeightedAsVendorTINTitle).ToList();
                var ocr = qParts.Last();
                ocr.ElectedAs = ReceiptParts.VendorTINTitle.ToString();
            }

            qParts = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.PriceTitle.ToString()).ToList();
            if (qParts.Count == 0)
            {
                qParts = Global.OcrLines.OrderBy(o => o.WeightedAsTotalTitle).ToList();
                var ocr = qParts.Last();
                ocr.ElectedAs = ReceiptParts.PriceTitle.ToString();
            }


            qParts = Global.OcrLines.Where(o => o.ElectedAs == ReceiptParts.Address.ToString()).ToList();
            if (qParts.Count == 0)
            {
                qParts = Global.OcrLines.OrderBy(o => o.WeightedAsAddress).ToList();
                var ocr = qParts.Last();
                ocr.ElectedAs = ReceiptParts.Address.ToString();
            }

            //foreach(BaiOcrLine ocr in Global.OcrLines)
            //{

            //}

        }

        private void btnCreateReceipt_Click(object sender, EventArgs e)
        {
            List<Receipt> table = new List<Receipt>();
            string user = Global.CurrentFolder.Name;
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
                    RocsTextService.GetReceiptDateValue(ocr);
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

            table.Add(Global.CurrentReciept);
            receiptBindingSource.DataSource = table;
            dgReceipts.DataSource = receiptBindingSource;
            this.bindingNavigatorReceipts.BindingSource = receiptBindingSource;


        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
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

            var receipt = (Receipt)receiptBindingSource.Current;
            using (MyDBContext db = new MyDBContext())
            {
                db.TableReceipts.Add(receipt);
                db.SaveChanges();

            }

            if (chkDelImage.Checked)
            {
                FileInfo current = (FileInfo)bindingSource1.Current;
                var fname = current.FullName;// context.GetValue(FilePath);
                File.Delete(fname);
                Global.LogWarn("File Deleted..." + fname);
            }


        }


        void SaveDataGridViewToCSV(string filename)
        {
            // Choose whether to write header. Use EnableWithoutHeaderText instead to omit header.
            dgCSV.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            // Select all the cells
            dgCSV.SelectAll();
            // Copy selected cells to DataObject
            DataObject dataObject = dgCSV.GetClipboardContent();
            // Get the text of the DataObject, and serialize it to a file
            File.WriteAllText(filename, dataObject.GetText(TextDataFormat.CommaSeparatedValue));
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            using (MyDBContext db = new MyDBContext())
            {
                var q = db.TableReceipts.Where(r => r.UserFolder == Global.CurrentFolder.Name);
                var list = q.OrderByDescending(r => r.Date).ToList();

                this.bindingSourceCSV.DataSource = list;
                dgCSV.DataSource = bindingSourceCSV;
                this.bindingNavigatorCSV.BindingSource = bindingSourceCSV;

            }


        }
    }
}