using BaiRocs.DAL;
using BaiRocs.Services;
using BaiRocs.WF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiRocs
{
    public partial class CSVForm : Form
    {
        public CSVForm()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (MyReceiptOnlyContext db = new MyReceiptOnlyContext())
            {
                var q = db.TableReceipts.Where(r => r.UserFolder == txtCurrentUser.Text);
                var list = q.OrderBy(r => r.Created).ToList();



                this.bindingSourceCSV.DataSource = list;
                dgCSV.DataSource = bindingSourceCSV;
                this.bindingNavigatorCSV.BindingSource = bindingSourceCSV;

            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            string csvName = "ReceiptDownLoad";

            //export to csv

            var folder = Global.Config.GetValue("ServerDownload") + "\\";// + txtCurrentUser.Text; //Global.CurrentUserFolder;
            var file = csvName + ".csv";
            var path = Path.Combine(folder, file);
            SaveDataGridViewToCSV(path);

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


        private void CSVForm_Load(object sender, EventArgs e)
        {

        }
    }
}
