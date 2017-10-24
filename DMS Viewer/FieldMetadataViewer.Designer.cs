namespace DMS_Viewer
{
    partial class FieldMetadataViewer
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
            this.txtUseEdit = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbGuiControl = new System.Windows.Forms.ComboBox();
            this.txtFieldLength = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbFieldFormat = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDecPos = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtVersionNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbFieldType = new System.Windows.Forms.ComboBox();
            this.txtFieldName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // txtUseEdit
            // 
            this.txtUseEdit.Location = new System.Drawing.Point(110, 193);
            this.txtUseEdit.Name = "txtUseEdit";
            this.txtUseEdit.Size = new System.Drawing.Size(89, 20);
            this.txtUseEdit.TabIndex = 38;
            this.txtUseEdit.Text = "0";
            this.txtUseEdit.TextChanged += new System.EventHandler(this.txtUseEdit_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 196);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 13);
            this.label9.TabIndex = 37;
            this.label9.Text = "Use Edit Number:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(84, 320);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 36;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 170);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "GUI Control:";
            // 
            // cmbGuiControl
            // 
            this.cmbGuiControl.FormattingEnabled = true;
            this.cmbGuiControl.Location = new System.Drawing.Point(84, 166);
            this.cmbGuiControl.Name = "cmbGuiControl";
            this.cmbGuiControl.Size = new System.Drawing.Size(150, 21);
            this.cmbGuiControl.TabIndex = 34;
            // 
            // txtFieldLength
            // 
            this.txtFieldLength.Location = new System.Drawing.Point(110, 60);
            this.txtFieldLength.Name = "txtFieldLength";
            this.txtFieldLength.Size = new System.Drawing.Size(124, 20);
            this.txtFieldLength.TabIndex = 33;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "Field Length:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Field Format:";
            // 
            // cmbFieldFormat
            // 
            this.cmbFieldFormat.FormattingEnabled = true;
            this.cmbFieldFormat.Location = new System.Drawing.Point(84, 139);
            this.cmbFieldFormat.Name = "cmbFieldFormat";
            this.cmbFieldFormat.Size = new System.Drawing.Size(150, 21);
            this.cmbFieldFormat.TabIndex = 30;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Field Type:";
            // 
            // txtDecPos
            // 
            this.txtDecPos.Location = new System.Drawing.Point(110, 86);
            this.txtDecPos.Name = "txtDecPos";
            this.txtDecPos.Size = new System.Drawing.Size(124, 20);
            this.txtDecPos.TabIndex = 28;
            this.txtDecPos.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Decimal Positions:";
            // 
            // txtVersionNumber
            // 
            this.txtVersionNumber.Location = new System.Drawing.Point(110, 34);
            this.txtVersionNumber.Name = "txtVersionNumber";
            this.txtVersionNumber.Size = new System.Drawing.Size(124, 20);
            this.txtVersionNumber.TabIndex = 26;
            this.txtVersionNumber.Text = "1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Version Number:";
            // 
            // cmbFieldType
            // 
            this.cmbFieldType.FormattingEnabled = true;
            this.cmbFieldType.Location = new System.Drawing.Point(84, 112);
            this.cmbFieldType.Name = "cmbFieldType";
            this.cmbFieldType.Size = new System.Drawing.Size(150, 21);
            this.cmbFieldType.TabIndex = 24;
            // 
            // txtFieldName
            // 
            this.txtFieldName.Location = new System.Drawing.Point(76, 8);
            this.txtFieldName.Name = "txtFieldName";
            this.txtFieldName.Size = new System.Drawing.Size(158, 20);
            this.txtFieldName.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Field Name:";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 219);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(222, 95);
            this.listBox1.TabIndex = 39;
            this.listBox1.SelectedValueChanged += new System.EventHandler(this.listBox1_SelectedValueChanged);
            // 
            // FieldMetadataViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 352);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.txtUseEdit);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cmbGuiControl);
            this.Controls.Add(this.txtFieldLength);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbFieldFormat);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtDecPos);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtVersionNumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbFieldType);
            this.Controls.Add(this.txtFieldName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FieldMetadataViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Field Metadata Explorer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtUseEdit;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbGuiControl;
        private System.Windows.Forms.TextBox txtFieldLength;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbFieldFormat;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDecPos;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtVersionNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbFieldType;
        private System.Windows.Forms.TextBox txtFieldName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
    }
}