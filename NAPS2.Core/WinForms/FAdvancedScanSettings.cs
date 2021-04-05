using NAPS2.Config;
using NAPS2.Scan;
using NAPS2.Scan.Twain;
using NAPS2.Scan.Wia;
using NAPS2.Scan.Wia.Native;
using System;

namespace NAPS2.WinForms
{
    public partial class FAdvancedScanSettings : FormBase
    {
        private readonly AppConfigManager appConfigManager;

        public FAdvancedScanSettings(AppConfigManager appConfigManager)
        {
            this.appConfigManager = appConfigManager;
            InitializeComponent();

            AddEnumItems<WiaVersion>(cmbWiaVersion);
            AddEnumItems<TwainImpl>(cmbTwainImpl);
            if (!Environment.Is64BitProcess)
            {
                cmbTwainImpl.Items.Remove(TwainImpl.X64);
            }
        }

        protected override void OnLoad(object sender, EventArgs e)
        {
            UpdateValues(ScanProfile);
            UpdateEnabled();

            new LayoutManager(this)
                .Bind(groupBox1, groupBox2, groupBox3, tbImageQuality, tbWhiteThreshold, tbCoverageThreshold)
                    .WidthToForm()
                .Bind(txtImageQuality, txtWhiteThreshold, txtCoverageThreshold, btnOK, btnCancel)
                    .RightToForm()
                .Activate();
        }

        private void UpdateValues(ScanProfile scanProfile)
        {
            cbHighQuality.Checked = scanProfile.MaxQuality;
            tbImageQuality.Value = scanProfile.Quality;
            txtImageQuality.Text = scanProfile.Quality.ToString("G");
            cbBrightnessContrastAfterScan.Checked = scanProfile.BrightnessContrastAfterScan;
            cbAutoDeskew.Checked = scanProfile.AutoDeskew;
            cbWiaOffsetWidth.Checked = scanProfile.WiaOffsetWidth;
            cmbWiaVersion.SelectedIndex = (int)scanProfile.WiaVersion;
            cbForcePageSize.Checked = scanProfile.ForcePageSize;
            cbForcePageSizeCrop.Checked = scanProfile.ForcePageSizeCrop;
            cbFlipDuplex.Checked = scanProfile.FlipDuplexedPages;
            cmbTwainImpl.SelectedIndex = (int)scanProfile.TwainImpl;
            cbExcludeBlankPages.Checked = scanProfile.ExcludeBlankPages;
            tbWhiteThreshold.Value = scanProfile.BlankPageWhiteThreshold;
            txtWhiteThreshold.Text = scanProfile.BlankPageWhiteThreshold.ToString("G");
            tbCoverageThreshold.Value = scanProfile.BlankPageCoverageThreshold;
            txtCoverageThreshold.Text = scanProfile.BlankPageCoverageThreshold.ToString("G");
        }

        private void UpdateEnabled()
        {
            cmbTwainImpl.Enabled = ScanProfile.DriverName == TwainScanDriver.DRIVER_NAME;
            cbWiaOffsetWidth.Enabled = ScanProfile.DriverName == WiaScanDriver.DRIVER_NAME;
            cmbWiaVersion.Enabled = ScanProfile.DriverName == WiaScanDriver.DRIVER_NAME;
            tbImageQuality.Enabled = !cbHighQuality.Checked;
            txtImageQuality.Enabled = !cbHighQuality.Checked;
            tbWhiteThreshold.Enabled = cbExcludeBlankPages.Checked && ScanProfile.BitDepth != ScanBitDepth.BlackWhite;
            txtWhiteThreshold.Enabled = cbExcludeBlankPages.Checked && ScanProfile.BitDepth != ScanBitDepth.BlackWhite;
            tbCoverageThreshold.Enabled = cbExcludeBlankPages.Checked;
            txtCoverageThreshold.Enabled = cbExcludeBlankPages.Checked;
        }

        public ScanProfile ScanProfile { get; set; }

        private void SaveSettings()
        {
            ScanProfile.Quality = tbImageQuality.Value;
            ScanProfile.MaxQuality = cbHighQuality.Checked;
            ScanProfile.BrightnessContrastAfterScan = cbBrightnessContrastAfterScan.Checked;
            ScanProfile.AutoDeskew = cbAutoDeskew.Checked;
            ScanProfile.WiaOffsetWidth = cbWiaOffsetWidth.Checked;
            if (cmbWiaVersion.SelectedIndex != -1)
            {
                ScanProfile.WiaVersion = (WiaVersion)cmbWiaVersion.SelectedIndex;
            }
            ScanProfile.ForcePageSize = cbForcePageSize.Checked;
            ScanProfile.ForcePageSizeCrop = cbForcePageSizeCrop.Checked;
            ScanProfile.FlipDuplexedPages = cbFlipDuplex.Checked;
            if (cmbTwainImpl.SelectedIndex != -1)
            {
                ScanProfile.TwainImpl = (TwainImpl)cmbTwainImpl.SelectedIndex;
            }
            ScanProfile.ExcludeBlankPages = cbExcludeBlankPages.Checked;
            ScanProfile.BlankPageWhiteThreshold = tbWhiteThreshold.Value;
            ScanProfile.BlankPageCoverageThreshold = tbCoverageThreshold.Value;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TbImageQuality_Scroll(object sender, EventArgs e)
        {
            txtImageQuality.Text = tbImageQuality.Value.ToString("G");
        }

        private void TxtImageQuality_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtImageQuality.Text, out int value)) return;
            if (value >= tbImageQuality.Minimum && value <= tbImageQuality.Maximum)
            {
                tbImageQuality.Value = value;
            }
        }

        private void CbHighQuality_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        private void CbExcludeBlankPages_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        private void TbWhiteThreshold_Scroll(object sender, EventArgs e)
        {
            txtWhiteThreshold.Text = tbWhiteThreshold.Value.ToString("G");
        }

        private void TxtWhiteThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtWhiteThreshold.Text, out int value)) return;
            if (value >= tbWhiteThreshold.Minimum && value <= tbWhiteThreshold.Maximum)
            {
                tbWhiteThreshold.Value = value;
            }
        }

        private void TbCoverageThreshold_Scroll(object sender, EventArgs e)
        {
            txtCoverageThreshold.Text = tbCoverageThreshold.Value.ToString("G");
        }

        private void TxtCoverageThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCoverageThreshold.Text, out int value)) return;
            if (value >= tbCoverageThreshold.Minimum && value <= tbCoverageThreshold.Maximum)
            {
                tbCoverageThreshold.Value = value;
            }
        }

        private void BtnRestoreDefaults_Click(object sender, EventArgs e)
        {
            UpdateValues(appConfigManager.Config.DefaultProfileSettings ?? new ScanProfile { Version = ScanProfile.CURRENT_VERSION });
        }
    }
}
