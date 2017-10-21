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
        public FieldMetadataViewer(DMSColumn column, DMSRecordFieldMetadata metadata)
        {
            InitializeComponent();

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
        }
    }
}
