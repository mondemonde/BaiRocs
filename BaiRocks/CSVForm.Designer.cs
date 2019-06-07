namespace BaiRocs
{
    partial class CSVForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSVForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageCSV = new System.Windows.Forms.TabPage();
            this.dgCSV = new System.Windows.Forms.DataGridView();
            this.bindingNavigatorCSV = new System.Windows.Forms.BindingNavigator(this.components);
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBox7 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.btnExcel = new System.Windows.Forms.Button();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.toolStripButton29 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton30 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton31 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton32 = new System.Windows.Forms.ToolStripButton();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtCurrentUser = new System.Windows.Forms.TextBox();
            this.bindingSourceCSV = new System.Windows.Forms.BindingSource(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPageCSV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCSV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorCSV)).BeginInit();
            this.bindingNavigatorCSV.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.groupBox16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceCSV)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageCSV);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 450);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageCSV
            // 
            this.tabPageCSV.Controls.Add(this.dgCSV);
            this.tabPageCSV.Controls.Add(this.bindingNavigatorCSV);
            this.tabPageCSV.Controls.Add(this.groupBox15);
            this.tabPageCSV.Controls.Add(this.groupBox16);
            this.tabPageCSV.Location = new System.Drawing.Point(4, 25);
            this.tabPageCSV.Name = "tabPageCSV";
            this.tabPageCSV.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCSV.Size = new System.Drawing.Size(792, 421);
            this.tabPageCSV.TabIndex = 9;
            this.tabPageCSV.Text = "CSV";
            this.tabPageCSV.UseVisualStyleBackColor = true;
            // 
            // dgCSV
            // 
            this.dgCSV.AllowUserToAddRows = false;
            this.dgCSV.AllowUserToDeleteRows = false;
            this.dgCSV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgCSV.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgCSV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgCSV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgCSV.Location = new System.Drawing.Point(3, 73);
            this.dgCSV.Name = "dgCSV";
            this.dgCSV.ReadOnly = true;
            this.dgCSV.RowHeadersVisible = false;
            this.dgCSV.RowTemplate.Height = 24;
            this.dgCSV.Size = new System.Drawing.Size(786, 188);
            this.dgCSV.TabIndex = 36;
            // 
            // bindingNavigatorCSV
            // 
            this.bindingNavigatorCSV.AddNewItem = null;
            this.bindingNavigatorCSV.CountItem = this.toolStripLabel7;
            this.bindingNavigatorCSV.DeleteItem = null;
            this.bindingNavigatorCSV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bindingNavigatorCSV.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.bindingNavigatorCSV.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton29,
            this.toolStripButton30,
            this.toolStripSeparator25,
            this.toolStripTextBox7,
            this.toolStripLabel7,
            this.toolStripSeparator26,
            this.toolStripButton31,
            this.toolStripButton32,
            this.toolStripSeparator27,
            this.toolStripSeparator28});
            this.bindingNavigatorCSV.Location = new System.Drawing.Point(3, 261);
            this.bindingNavigatorCSV.MoveFirstItem = this.toolStripButton29;
            this.bindingNavigatorCSV.MoveLastItem = this.toolStripButton32;
            this.bindingNavigatorCSV.MoveNextItem = this.toolStripButton31;
            this.bindingNavigatorCSV.MovePreviousItem = this.toolStripButton30;
            this.bindingNavigatorCSV.Name = "bindingNavigatorCSV";
            this.bindingNavigatorCSV.PositionItem = this.toolStripTextBox7;
            this.bindingNavigatorCSV.Size = new System.Drawing.Size(786, 27);
            this.bindingNavigatorCSV.TabIndex = 38;
            this.bindingNavigatorCSV.Text = "bindingNavigator1";
            // 
            // toolStripLabel7
            // 
            this.toolStripLabel7.Name = "toolStripLabel7";
            this.toolStripLabel7.Size = new System.Drawing.Size(45, 24);
            this.toolStripLabel7.Text = "of {0}";
            this.toolStripLabel7.ToolTipText = "Total number of items";
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            this.toolStripSeparator25.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripTextBox7
            // 
            this.toolStripTextBox7.AccessibleName = "Position";
            this.toolStripTextBox7.AutoSize = false;
            this.toolStripTextBox7.Name = "toolStripTextBox7";
            this.toolStripTextBox7.Size = new System.Drawing.Size(65, 27);
            this.toolStripTextBox7.Text = "0";
            this.toolStripTextBox7.ToolTipText = "Current position";
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            this.toolStripSeparator26.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            this.toolStripSeparator27.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripSeparator28
            // 
            this.toolStripSeparator28.Name = "toolStripSeparator28";
            this.toolStripSeparator28.Size = new System.Drawing.Size(6, 27);
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.txtCurrentUser);
            this.groupBox15.Controls.Add(this.btnLoad);
            this.groupBox15.Controls.Add(this.btnExcel);
            this.groupBox15.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox15.Location = new System.Drawing.Point(3, 288);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(786, 130);
            this.groupBox15.TabIndex = 37;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Search";
            // 
            // btnExcel
            // 
            this.btnExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExcel.Location = new System.Drawing.Point(576, 22);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(128, 29);
            this.btnExcel.TabIndex = 29;
            this.btnExcel.Text = "Create CSV";
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // groupBox16
            // 
            this.groupBox16.BackColor = System.Drawing.Color.White;
            this.groupBox16.Controls.Add(this.pictureBox8);
            this.groupBox16.Controls.Add(this.label11);
            this.groupBox16.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox16.Location = new System.Drawing.Point(3, 3);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(786, 70);
            this.groupBox16.TabIndex = 35;
            this.groupBox16.TabStop = false;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(24, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(381, 35);
            this.label11.TabIndex = 0;
            this.label11.Text = "CSV";
            // 
            // toolStripButton29
            // 
            this.toolStripButton29.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton29.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton29.Image")));
            this.toolStripButton29.Name = "toolStripButton29";
            this.toolStripButton29.RightToLeftAutoMirrorImage = true;
            this.toolStripButton29.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton29.Text = "Move first";
            // 
            // toolStripButton30
            // 
            this.toolStripButton30.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton30.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton30.Image")));
            this.toolStripButton30.Name = "toolStripButton30";
            this.toolStripButton30.RightToLeftAutoMirrorImage = true;
            this.toolStripButton30.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton30.Text = "Move previous";
            // 
            // toolStripButton31
            // 
            this.toolStripButton31.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton31.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton31.Image")));
            this.toolStripButton31.Name = "toolStripButton31";
            this.toolStripButton31.RightToLeftAutoMirrorImage = true;
            this.toolStripButton31.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton31.Text = "Move next";
            // 
            // toolStripButton32
            // 
            this.toolStripButton32.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton32.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton32.Image")));
            this.toolStripButton32.Name = "toolStripButton32";
            this.toolStripButton32.RightToLeftAutoMirrorImage = true;
            this.toolStripButton32.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton32.Text = "Move last";
            // 
            // pictureBox8
            // 
            this.pictureBox8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox8.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox8.Image")));
            this.pictureBox8.Location = new System.Drawing.Point(709, 21);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(67, 39);
            this.pictureBox8.TabIndex = 1;
            this.pictureBox8.TabStop = false;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(315, 63);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(128, 32);
            this.btnLoad.TabIndex = 30;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtCurrentUser
            // 
            this.txtCurrentUser.Location = new System.Drawing.Point(235, 22);
            this.txtCurrentUser.Name = "txtCurrentUser";
            this.txtCurrentUser.Size = new System.Drawing.Size(291, 22);
            this.txtCurrentUser.TabIndex = 31;
            this.txtCurrentUser.Text = "rgalvez";
            this.txtCurrentUser.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CSVForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "CSVForm";
            this.Text = "CSVForm";
            this.Load += new System.EventHandler(this.CSVForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPageCSV.ResumeLayout(false);
            this.tabPageCSV.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCSV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorCSV)).EndInit();
            this.bindingNavigatorCSV.ResumeLayout(false);
            this.bindingNavigatorCSV.PerformLayout();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceCSV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageCSV;
        private System.Windows.Forms.DataGridView dgCSV;
        private System.Windows.Forms.BindingNavigator bindingNavigatorCSV;
        private System.Windows.Forms.ToolStripLabel toolStripLabel7;
        private System.Windows.Forms.ToolStripButton toolStripButton29;
        private System.Windows.Forms.ToolStripButton toolStripButton30;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
        private System.Windows.Forms.ToolStripButton toolStripButton31;
        private System.Windows.Forms.ToolStripButton toolStripButton32;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator28;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.GroupBox groupBox16;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtCurrentUser;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.BindingSource bindingSourceCSV;
    }
}