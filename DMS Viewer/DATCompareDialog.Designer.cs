namespace DMS_Viewer
{
    partial class DATCompareDialog
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lstRight = new System.Windows.Forms.ListView();
            this.lstLeft = new System.Windows.Forms.ListView();
            this.btnCompareRight = new System.Windows.Forms.Button();
            this.btnCompareToLeft = new System.Windows.Forms.Button();
            this.btnViewDataLeft = new System.Windows.Forms.Button();
            this.btnViewDataRight = new System.Windows.Forms.Button();
            this.lblLeft = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.lblRight = new System.Windows.Forms.Label();
            this.lblLeftRows = new System.Windows.Forms.Label();
            this.lblRightRows = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lstRight
            // 
            this.lstRight.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lstRight.HideSelection = false;
            this.lstRight.Location = new System.Drawing.Point(276, 31);
            this.lstRight.Margin = new System.Windows.Forms.Padding(2);
            this.lstRight.Name = "lstRight";
            this.lstRight.Size = new System.Drawing.Size(221, 346);
            this.lstRight.TabIndex = 26;
            this.lstRight.UseCompatibleStateImageBehavior = false;
            this.lstRight.View = System.Windows.Forms.View.Details;
            this.lstRight.SelectedIndexChanged += new System.EventHandler(this.lstRight_SelectedIndexChanged);
            this.lstRight.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstRight_MouseClick);
            // 
            // lstLeft
            // 
            this.lstLeft.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstLeft.HideSelection = false;
            this.lstLeft.Location = new System.Drawing.Point(11, 31);
            this.lstLeft.Margin = new System.Windows.Forms.Padding(2);
            this.lstLeft.Name = "lstLeft";
            this.lstLeft.Size = new System.Drawing.Size(221, 346);
            this.lstLeft.TabIndex = 27;
            this.lstLeft.UseCompatibleStateImageBehavior = false;
            this.lstLeft.View = System.Windows.Forms.View.Details;
            this.lstLeft.SelectedIndexChanged += new System.EventHandler(this.lstLeft_SelectedIndexChanged);
            this.lstLeft.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstLeft_MouseClick);
            // 
            // btnCompareRight
            // 
            this.btnCompareRight.Enabled = false;
            this.btnCompareRight.Location = new System.Drawing.Point(63, 415);
            this.btnCompareRight.Name = "btnCompareRight";
            this.btnCompareRight.Size = new System.Drawing.Size(101, 23);
            this.btnCompareRight.TabIndex = 28;
            this.btnCompareRight.Text = "Compare to Right";
            this.btnCompareRight.UseVisualStyleBackColor = true;
            this.btnCompareRight.Click += new System.EventHandler(this.btnCompareRight_Click);
            // 
            // btnCompareToLeft
            // 
            this.btnCompareToLeft.Enabled = false;
            this.btnCompareToLeft.Location = new System.Drawing.Point(337, 415);
            this.btnCompareToLeft.Name = "btnCompareToLeft";
            this.btnCompareToLeft.Size = new System.Drawing.Size(101, 23);
            this.btnCompareToLeft.TabIndex = 29;
            this.btnCompareToLeft.Text = "Compare to Left";
            this.btnCompareToLeft.UseVisualStyleBackColor = true;
            this.btnCompareToLeft.Click += new System.EventHandler(this.Button2_Click);
            // 
            // btnViewDataLeft
            // 
            this.btnViewDataLeft.Enabled = false;
            this.btnViewDataLeft.Location = new System.Drawing.Point(63, 382);
            this.btnViewDataLeft.Name = "btnViewDataLeft";
            this.btnViewDataLeft.Size = new System.Drawing.Size(101, 23);
            this.btnViewDataLeft.TabIndex = 31;
            this.btnViewDataLeft.Text = "View Data";
            this.btnViewDataLeft.UseVisualStyleBackColor = true;
            this.btnViewDataLeft.Click += new System.EventHandler(this.btnViewDataLeft_Click);
            // 
            // btnViewDataRight
            // 
            this.btnViewDataRight.Enabled = false;
            this.btnViewDataRight.Location = new System.Drawing.Point(337, 382);
            this.btnViewDataRight.Name = "btnViewDataRight";
            this.btnViewDataRight.Size = new System.Drawing.Size(101, 23);
            this.btnViewDataRight.TabIndex = 32;
            this.btnViewDataRight.Text = "View Data";
            this.btnViewDataRight.UseVisualStyleBackColor = true;
            this.btnViewDataRight.Click += new System.EventHandler(this.btnViewDataRight_Click);
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(8, 9);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(71, 13);
            this.lblLeft.TabIndex = 33;
            this.lblLeft.Text = "Select a file...";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(206, 3);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(24, 23);
            this.button6.TabIndex = 34;
            this.button6.Text = "...";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(473, 4);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(24, 23);
            this.button7.TabIndex = 35;
            this.button7.Text = "...";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(274, 9);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(71, 13);
            this.lblRight.TabIndex = 36;
            this.lblRight.Text = "Select a file...";
            // 
            // lblLeftRows
            // 
            this.lblLeftRows.AutoSize = true;
            this.lblLeftRows.Location = new System.Drawing.Point(171, 382);
            this.lblLeftRows.Name = "lblLeftRows";
            this.lblLeftRows.Size = new System.Drawing.Size(40, 13);
            this.lblLeftRows.TabIndex = 37;
            this.lblLeftRows.Text = "Rows: ";
            // 
            // lblRightRows
            // 
            this.lblRightRows.AutoSize = true;
            this.lblRightRows.Location = new System.Drawing.Point(445, 382);
            this.lblRightRows.Name = "lblRightRows";
            this.lblRightRows.Size = new System.Drawing.Size(40, 13);
            this.lblRightRows.TabIndex = 38;
            this.lblRightRows.Text = "Rows: ";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(11, 444);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(485, 16);
            this.progressBar1.TabIndex = 39;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Table Name";
            this.columnHeader1.Width = 213;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Table Name";
            this.columnHeader2.Width = 217;
            // 
            // DATCompareDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 463);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblRightRows);
            this.Controls.Add(this.lblLeftRows);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.lblLeft);
            this.Controls.Add(this.btnViewDataRight);
            this.Controls.Add(this.btnViewDataLeft);
            this.Controls.Add(this.btnCompareToLeft);
            this.Controls.Add(this.btnCompareRight);
            this.Controls.Add(this.lstLeft);
            this.Controls.Add(this.lstRight);
            this.Name = "DATCompareDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DAT Compare";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ListView lstRight;
        private System.Windows.Forms.ListView lstLeft;
        private System.Windows.Forms.Button btnCompareRight;
        private System.Windows.Forms.Button btnCompareToLeft;
        private System.Windows.Forms.Button btnViewDataLeft;
        private System.Windows.Forms.Button btnViewDataRight;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label lblLeftRows;
        private System.Windows.Forms.Label lblRightRows;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}