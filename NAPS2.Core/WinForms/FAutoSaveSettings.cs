using NAPS2.ImportExport;
using NAPS2.Scan;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace NAPS2.WinForms
{
    public partial class FAutoSaveSettings : FormBase
    {
        private readonly DialogHelper dialogHelper;

        public FAutoSaveSettings(DialogHelper dialogHelper)
        {
            this.dialogHelper = dialogHelper;
            InitializeComponent();
        }

        protected override void OnLoad(object sender, EventArgs e)
        {
            if (ScanProfile.AutoSaveSettings != null)
            {
                txtFilePath.Text = ScanProfile.AutoSaveSettings.FilePath;
                cbPromptForFilePath.Checked = ScanProfile.AutoSaveSettings.PromptForFilePath;
                cbClearAfterSave.Checked = ScanProfile.AutoSaveSettings.ClearImagesAfterSaving;
                switch (ScanProfile.AutoSaveSettings.Separator)
                {
                    case SaveSeparator.FilePerScan:
                        rdFilePerScan.Checked = true;
                        break;
                    case SaveSeparator.PatchT:
                        rdSeparateByPatchT.Checked = true;
                        break;
                    default:
                        rdFilePerPage.Checked = true;
                        break;
                }
            }

            new LayoutManager(this)
                .Bind(txtFilePath)
                    .WidthToForm()
                .Bind(btnChooseFolder, btnOK, btnCancel)
                    .RightToForm()
                .Activate();
        }

        public bool Result { get; private set; }

        public ScanProfile ScanProfile { get; set; }

        private void SaveSettings()
        {
            ScanProfile.AutoSaveSettings = new AutoSaveSettings
            {
                FilePath = txtFilePath.Text,
                PromptForFilePath = cbPromptForFilePath.Checked,
                ClearImagesAfterSaving = cbClearAfterSave.Checked,
                Separator = rdFilePerScan.Checked ? SaveSeparator.FilePerScan
                          : rdSeparateByPatchT.Checked ? SaveSeparator.PatchT
                          : SaveSeparator.FilePerPage
            };
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilePath.Text) && !cbPromptForFilePath.Checked)
            {
                txtFilePath.Focus();
                return;
            }
            Result = true;
            SaveSettings();
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnChooseFolder_Click(object sender, EventArgs e)
        {
            if (dialogHelper.PromptToSavePdfOrImage(null, out string savePath))
            {
                txtFilePath.Text = savePath;
            }
        }

        private void LinkPlaceholders_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var form = FormFactory.Create<FPlaceholders>();
            form.FileName = txtFilePath.Text;
            if (form.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = form.FileName;
            }
        }

        private void LinkPatchCodeInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(FBatchScan.PATCH_CODE_INFO_URL);
        }
    }
}
