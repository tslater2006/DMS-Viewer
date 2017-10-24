using DMSLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMS_Viewer
{
    public partial class FieldMetadataViewer : Form
    {
        private DMSRecordFieldMetadata meta;
        private DMSColumn col;
        public FieldMetadataViewer(DMSColumn column, DMSRecordFieldMetadata metadata)
        {
            InitializeComponent();

            meta = metadata;
            col = column;

            /* Populate Field Type Combo */
            cmbFieldType.Items.AddRange(Enum.GetNames(typeof(FieldTypes)));
            cmbFieldFormat.Items.AddRange(Enum.GetNames(typeof(FieldFormats)));
            cmbGuiControl.Items.AddRange(Enum.GetNames(typeof(GUIControls)));

            txtFieldName.Text = metadata.FieldName;
            txtFieldLength.Text = metadata.FieldLength.ToString();
            txtDecPos.Text = metadata.DecimalPositions.ToString();
            txtUseEdit.Text = ((int)metadata.UseEditMask).ToString();
            txtVersionNumber.Text = metadata.VersionNumber.ToString();
            cmbFieldType.Text = metadata.FieldType.ToString();
            cmbFieldFormat.Text = metadata.FieldFormat.ToString();
            cmbGuiControl.Text = metadata.DefaultGUIControl.ToString();

            foreach (var value in Enum.GetValues(typeof(UseEditFlags)))
            {
                listBox1.Items.Add(value);
                if (metadata.UseEditMask.HasFlag((Enum)value))
                {
                    listBox1.SetSelected(listBox1.Items.Count-1,true);
                }
            }
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            /* Update UseEditFlags value */

            UseEditFlags newValue = 0;
            
            for (var x = 0; x < listBox1.Items.Count; x++)
            {
                if (listBox1.GetSelected(x))
                {
                    newValue |= (UseEditFlags)listBox1.Items[x];
                }
            }

            txtUseEdit.Text = ((int)newValue).ToString();

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                meta.FieldName = txtFieldName.Text;
                meta.FieldLength = int.Parse(txtFieldLength.Text);
                meta.DecimalPositions = int.Parse(txtDecPos.Text);
                meta.UseEditMask = (UseEditFlags)(int.Parse(txtUseEdit.Text));
                meta.VersionNumber = int.Parse(txtVersionNumber.Text);
                meta.FieldType = (FieldTypes)(Enum.Parse(typeof (FieldTypes), cmbFieldType.Text));
                meta.FieldFormat = (FieldFormats)Enum.Parse(typeof(FieldFormats),cmbFieldFormat.Text);
                meta.DefaultGUIControl = (GUIControls)Enum.Parse(typeof(GUIControls),cmbGuiControl.Text);

                col.Name = meta.FieldName;

                /* TODO: Map the FieldTypes to the string version that Columns use */
                /* col.Type = ""; */

                switch (meta.FieldType)
                {
                    case FieldTypes.CHAR:
                    case FieldTypes.IMAGE_REF:
                        col.Type = "CHAR";
                        break;
                    case FieldTypes.LONG_CHAR:
                        col.Type = "LONG";
                        break;
                    case FieldTypes.NUMBER:
                    case FieldTypes.SIGNED_NUMBER:
                        col.Type = "NUMBER";
                        break;
                    case FieldTypes.DATE:
                        col.Type = "DATE";
                        break;
                    case FieldTypes.DATETIME:
                        col.Type = "DATETIME";
                        break;
                    case FieldTypes.TIME:
                        col.Type = "TIME";
                        break;
                    case FieldTypes.IMG_OR_ATTACH:
                        col.Type = "IMAGE";
                        break;
                }

                if (meta.DecimalPositions > 0)
                {
                    col.Size = meta.FieldLength + "," + meta.DecimalPositions;
                }
                else
                {
                    col.Size = meta.FieldLength.ToString();
                }
            }
            catch (Exception ex) { }
            this.Hide();
        }
    }
}
