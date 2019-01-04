namespace DMS_Viewer
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateSQLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebuildScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tableList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtStarted = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.columnList = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblRowCount = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label7 = new System.Windows.Forms.Label();
            this.whereClause = new System.Windows.Forms.TextBox();
            this.dataViewer = new System.Windows.Forms.Button();
            this.copyTables = new System.Windows.Forms.Button();
            this.copyWhereClause = new System.Windows.Forms.Button();
            this.btnRecordMeta = new System.Windows.Forms.Button();
            this.btnCompareToDB = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1472, 40);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(64, 36);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(211, 38);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(211, 38);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(211, 38);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateSQLToolStripMenuItem,
            this.rebuildScriptToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(82, 36);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // generateSQLToolStripMenuItem
            // 
            this.generateSQLToolStripMenuItem.Enabled = false;
            this.generateSQLToolStripMenuItem.Name = "generateSQLToolStripMenuItem";
            this.generateSQLToolStripMenuItem.Size = new System.Drawing.Size(261, 38);
            this.generateSQLToolStripMenuItem.Text = "Generate SQL";
            this.generateSQLToolStripMenuItem.Click += new System.EventHandler(this.generateSQLToolStripMenuItem_Click);
            // 
            // rebuildScriptToolStripMenuItem
            // 
            this.rebuildScriptToolStripMenuItem.Name = "rebuildScriptToolStripMenuItem";
            this.rebuildScriptToolStripMenuItem.Size = new System.Drawing.Size(261, 38);
            this.rebuildScriptToolStripMenuItem.Text = "Rebuild Script";
            this.rebuildScriptToolStripMenuItem.Click += new System.EventHandler(this.rebuildScriptToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tableList
            // 
            this.tableList.FormattingEnabled = true;
            this.tableList.ItemHeight = 25;
            this.tableList.Location = new System.Drawing.Point(16, 158);
            this.tableList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableList.Name = "tableList";
            this.tableList.Size = new System.Drawing.Size(306, 654);
            this.tableList.TabIndex = 6;
            this.tableList.SelectedIndexChanged += new System.EventHandler(this.tableList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 129);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 25);
            this.label1.TabIndex = 7;
            this.label1.Text = "Records";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 25);
            this.label2.TabIndex = 8;
            this.label2.Text = "Version:";
            // 
            // txtVersion
            // 
            this.txtVersion.Location = new System.Drawing.Point(104, 62);
            this.txtVersion.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.ReadOnly = true;
            this.txtVersion.Size = new System.Drawing.Size(204, 31);
            this.txtVersion.TabIndex = 9;
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(444, 62);
            this.txtDatabase.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.ReadOnly = true;
            this.txtDatabase.Size = new System.Drawing.Size(294, 31);
            this.txtDatabase.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(328, 65);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 25);
            this.label3.TabIndex = 10;
            this.label3.Text = "Database:";
            // 
            // txtStarted
            // 
            this.txtStarted.Location = new System.Drawing.Point(876, 60);
            this.txtStarted.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtStarted.Name = "txtStarted";
            this.txtStarted.ReadOnly = true;
            this.txtStarted.Size = new System.Drawing.Size(294, 31);
            this.txtStarted.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(784, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 25);
            this.label4.TabIndex = 12;
            this.label4.Text = "Started:";
            // 
            // columnList
            // 
            this.columnList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colType,
            this.colSize});
            this.columnList.Location = new System.Drawing.Point(364, 238);
            this.columnList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.columnList.Name = "columnList";
            this.columnList.Size = new System.Drawing.Size(890, 631);
            this.columnList.TabIndex = 14;
            this.columnList.UseCompatibleStateImageBehavior = false;
            this.columnList.View = System.Windows.Forms.View.Details;
            this.columnList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.columnList_MouseClick);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 130;
            // 
            // colType
            // 
            this.colType.Text = "Type";
            this.colType.Width = 116;
            // 
            // colSize
            // 
            this.colSize.Text = "Size";
            this.colSize.Width = 100;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(358, 210);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 25);
            this.label5.TabIndex = 15;
            this.label5.Text = "Columns";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1298, 208);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 25);
            this.label6.TabIndex = 16;
            this.label6.Text = "Rows:";
            // 
            // lblRowCount
            // 
            this.lblRowCount.AutoSize = true;
            this.lblRowCount.Location = new System.Drawing.Point(1380, 208);
            this.lblRowCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRowCount.Name = "lblRowCount";
            this.lblRowCount.Size = new System.Drawing.Size(24, 25);
            this.lblRowCount.TabIndex = 17;
            this.lblRowCount.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(358, 158);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 25);
            this.label7.TabIndex = 18;
            this.label7.Text = "Where:";
            // 
            // whereClause
            // 
            this.whereClause.Location = new System.Drawing.Point(442, 152);
            this.whereClause.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.whereClause.Name = "whereClause";
            this.whereClause.Size = new System.Drawing.Size(694, 31);
            this.whereClause.TabIndex = 19;
            // 
            // dataViewer
            // 
            this.dataViewer.Enabled = false;
            this.dataViewer.Location = new System.Drawing.Point(1268, 238);
            this.dataViewer.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dataViewer.Name = "dataViewer";
            this.dataViewer.Size = new System.Drawing.Size(180, 44);
            this.dataViewer.TabIndex = 20;
            this.dataViewer.Text = "View Data";
            this.dataViewer.UseVisualStyleBackColor = true;
            this.dataViewer.Click += new System.EventHandler(this.dataViewer_Click);
            // 
            // copyTables
            // 
            this.copyTables.Location = new System.Drawing.Point(16, 829);
            this.copyTables.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.copyTables.Name = "copyTables";
            this.copyTables.Size = new System.Drawing.Size(310, 44);
            this.copyTables.TabIndex = 21;
            this.copyTables.Text = "Copy Record List";
            this.copyTables.UseVisualStyleBackColor = true;
            this.copyTables.Click += new System.EventHandler(this.copyTables_Click);
            // 
            // copyWhereClause
            // 
            this.copyWhereClause.Location = new System.Drawing.Point(1152, 148);
            this.copyWhereClause.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.copyWhereClause.Name = "copyWhereClause";
            this.copyWhereClause.Size = new System.Drawing.Size(106, 44);
            this.copyWhereClause.TabIndex = 22;
            this.copyWhereClause.Text = "Copy";
            this.copyWhereClause.UseVisualStyleBackColor = true;
            this.copyWhereClause.Click += new System.EventHandler(this.copyWhereClause_Click);
            // 
            // btnRecordMeta
            // 
            this.btnRecordMeta.Enabled = false;
            this.btnRecordMeta.Location = new System.Drawing.Point(1268, 294);
            this.btnRecordMeta.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnRecordMeta.Name = "btnRecordMeta";
            this.btnRecordMeta.Size = new System.Drawing.Size(180, 44);
            this.btnRecordMeta.TabIndex = 23;
            this.btnRecordMeta.Text = "View Metadata";
            this.btnRecordMeta.UseVisualStyleBackColor = true;
            this.btnRecordMeta.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCompareToDB
            // 
            this.btnCompareToDB.Enabled = false;
            this.btnCompareToDB.Location = new System.Drawing.Point(1268, 391);
            this.btnCompareToDB.Margin = new System.Windows.Forms.Padding(6);
            this.btnCompareToDB.Name = "btnCompareToDB";
            this.btnCompareToDB.Size = new System.Drawing.Size(180, 44);
            this.btnCompareToDB.TabIndex = 24;
            this.btnCompareToDB.Text = "Compare to DB";
            this.btnCompareToDB.UseVisualStyleBackColor = true;
            this.btnCompareToDB.Click += new System.EventHandler(this.btnCompareToDB_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1472, 937);
            this.Controls.Add(this.btnCompareToDB);
            this.Controls.Add(this.btnRecordMeta);
            this.Controls.Add(this.copyWhereClause);
            this.Controls.Add(this.copyTables);
            this.Controls.Add(this.dataViewer);
            this.Controls.Add(this.whereClause);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblRowCount);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.columnList);
            this.Controls.Add(this.txtStarted);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtDatabase);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tableList);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "DMS Explorer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ListBox tableList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtStarted;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView columnList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblRowCount;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateSQLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rebuildScriptToolStripMenuItem;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox whereClause;
        private System.Windows.Forms.Button dataViewer;
        private System.Windows.Forms.Button copyTables;
        private System.Windows.Forms.Button copyWhereClause;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.Button btnRecordMeta;
        private System.Windows.Forms.Button btnCompareToDB;
    }
}

