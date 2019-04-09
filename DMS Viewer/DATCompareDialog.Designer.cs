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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.btnViewDataLeft = new System.Windows.Forms.Button();
            this.btnViewDataRight = new System.Windows.Forms.Button();
            this.lblLeft = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.lblRight = new System.Windows.Forms.Label();
            this.lblLeftRows = new System.Windows.Forms.Label();
            this.lblRightRows = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lstRight
            // 
            this.lstRight.HideSelection = false;
            this.lstRight.Location = new System.Drawing.Point(322, 36);
            this.lstRight.Margin = new System.Windows.Forms.Padding(2);
            this.lstRight.Name = "lstRight";
            this.lstRight.Size = new System.Drawing.Size(257, 399);
            this.lstRight.TabIndex = 26;
            this.lstRight.UseCompatibleStateImageBehavior = false;
            this.lstRight.View = System.Windows.Forms.View.List;
            this.lstRight.SelectedIndexChanged += new System.EventHandler(this.lstRight_SelectedIndexChanged);
            // 
            // lstLeft
            // 
            this.lstLeft.HideSelection = false;
            this.lstLeft.Location = new System.Drawing.Point(13, 36);
            this.lstLeft.Margin = new System.Windows.Forms.Padding(2);
            this.lstLeft.Name = "lstLeft";
            this.lstLeft.Size = new System.Drawing.Size(257, 399);
            this.lstLeft.TabIndex = 27;
            this.lstLeft.UseCompatibleStateImageBehavior = false;
            this.lstLeft.View = System.Windows.Forms.View.List;
            this.lstLeft.SelectedIndexChanged += new System.EventHandler(this.lstLeft_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(74, 479);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 27);
            this.button1.TabIndex = 28;
            this.button1.Text = "Compare to Right";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(393, 479);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(118, 27);
            this.button2.TabIndex = 29;
            this.button2.Text = "Compare to Left";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(234, 479);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(118, 27);
            this.button3.TabIndex = 30;
            this.button3.Text = "Bidirectional";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // btnViewDataLeft
            // 
            this.btnViewDataLeft.Enabled = false;
            this.btnViewDataLeft.Location = new System.Drawing.Point(74, 441);
            this.btnViewDataLeft.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnViewDataLeft.Name = "btnViewDataLeft";
            this.btnViewDataLeft.Size = new System.Drawing.Size(118, 27);
            this.btnViewDataLeft.TabIndex = 31;
            this.btnViewDataLeft.Text = "View Data";
            this.btnViewDataLeft.UseVisualStyleBackColor = true;
            this.btnViewDataLeft.Click += new System.EventHandler(this.btnViewDataLeft_Click);
            // 
            // btnViewDataRight
            // 
            this.btnViewDataRight.Enabled = false;
            this.btnViewDataRight.Location = new System.Drawing.Point(393, 441);
            this.btnViewDataRight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnViewDataRight.Name = "btnViewDataRight";
            this.btnViewDataRight.Size = new System.Drawing.Size(118, 27);
            this.btnViewDataRight.TabIndex = 32;
            this.btnViewDataRight.Text = "View Data";
            this.btnViewDataRight.UseVisualStyleBackColor = true;
            this.btnViewDataRight.Click += new System.EventHandler(this.btnViewDataRight_Click);
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(9, 10);
            this.lblLeft.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(75, 15);
            this.lblLeft.TabIndex = 33;
            this.lblLeft.Text = "Select a file...";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(240, 3);
            this.button6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(28, 27);
            this.button6.TabIndex = 34;
            this.button6.Text = "...";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(552, 5);
            this.button7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(28, 27);
            this.button7.TabIndex = 35;
            this.button7.Text = "...";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(320, 10);
            this.lblRight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(75, 15);
            this.lblRight.TabIndex = 36;
            this.lblRight.Text = "Select a file...";
            // 
            // lblLeftRows
            // 
            this.lblLeftRows.AutoSize = true;
            this.lblLeftRows.Location = new System.Drawing.Point(200, 441);
            this.lblLeftRows.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLeftRows.Name = "lblLeftRows";
            this.lblLeftRows.Size = new System.Drawing.Size(41, 15);
            this.lblLeftRows.TabIndex = 37;
            this.lblLeftRows.Text = "Rows: ";
            // 
            // lblRightRows
            // 
            this.lblRightRows.AutoSize = true;
            this.lblRightRows.Location = new System.Drawing.Point(519, 441);
            this.lblRightRows.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRightRows.Name = "lblRightRows";
            this.lblRightRows.Size = new System.Drawing.Size(41, 15);
            this.lblRightRows.TabIndex = 38;
            this.lblRightRows.Text = "Rows: ";
            // 
            // DATCompareDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 519);
            this.Controls.Add(this.lblRightRows);
            this.Controls.Add(this.lblLeftRows);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.lblLeft);
            this.Controls.Add(this.btnViewDataRight);
            this.Controls.Add(this.btnViewDataLeft);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lstLeft);
            this.Controls.Add(this.lstRight);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnViewDataLeft;
        private System.Windows.Forms.Button btnViewDataRight;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label lblLeftRows;
        private System.Windows.Forms.Label lblRightRows;
        private System.Windows.Forms.Label lblRight;
    }
}