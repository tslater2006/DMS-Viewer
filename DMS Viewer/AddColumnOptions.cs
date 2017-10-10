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
    public partial class AddColumnOptions : Form
    {
        public DMSNewColumn newColumn;
        public string defaultValue;

        public AddColumnOptions()
        {
            InitializeComponent();
        }

        private void AddColumnOptions_Load(object sender, EventArgs e)
        {
            /* new DMSNewColumn("MY_FIELD", 1, 20, 0, UseEditFlags.UNKNOWN4, FieldTypes.CHAR, FieldFormats.MIXEDCASE, GUIControls.DEFAULT);*/

            /* Populate Field Type Combo */
            cmbFieldType.Items.AddRange(Enum.GetNames(typeof(FieldTypes)));
            cmbFieldFormat.Items.AddRange(Enum.GetNames(typeof(FieldFormats)));
            cmbGuiControl.Items.AddRange(Enum.GetNames(typeof(GUIControls)));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "The UseEdit value can be found on the PSRECFIELD Table, use the RECNAME and FIELDNAME to look up the USEEDIT field.", "Whats the UseEdit?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            FieldTypes type = (FieldTypes)Enum.GetValues(typeof(FieldTypes)).GetValue(cmbFieldType.SelectedIndex);
            FieldFormats format = (FieldFormats)Enum.GetValues(typeof(FieldFormats)).GetValue(cmbFieldFormat.SelectedIndex);
            GUIControls gui = (GUIControls)Enum.GetValues(typeof(GUIControls)).GetValue(cmbGuiControl.SelectedIndex);

            newColumn = new DMSNewColumn(txtFieldName.Text, int.Parse(txtVersionNumber.Text), int.Parse(txtFieldLength.Text), int.Parse(txtDecPos.Text), (UseEditFlags)int.Parse(txtUseEdit.Text), type, format, gui);

            defaultValue = txtDefaultValue.Text;

            this.Hide();
        }
    }
}
