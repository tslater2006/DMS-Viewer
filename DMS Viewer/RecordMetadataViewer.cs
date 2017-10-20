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
    public partial class RecordMetadataViewer : Form
    {
        public RecordMetadataViewer(DMSRecordMetadata metadata)
        {
            InitializeComponent();

            txtBuildSeq.Text = metadata.BuildSequence.ToString();
            txtDBName.Text = metadata.DBName;
            txtDeleteRecord.Text = metadata.AnalyticDeleteRecord;
            txtFieldCount.Text = metadata.FieldCount.ToString();
            txtIndexCount.Text = metadata.IndexCount.ToString();
            txtOptTriggers.Text = metadata.OptimizationTriggers;
            txtOwner.Text = metadata.OwnerID;
            txtParentRecord.Text = metadata.ParentRecord;
            txtRecordDBName.Text = metadata.RecordDBName;
            txtRecordLang.Text = metadata.RecordLanguage;
            txtRecordName.Text = metadata.RecordName;
            txtRelLang.Text = metadata.RelatedLanguageRecord;
            txtTablespace.Text = metadata.TableSpaceName;
            txtVersion2.Text = metadata.VersionNumber2.ToString();
            txtVersionNum.Text = metadata.VersionNumber.ToString();

        }
    }
}
