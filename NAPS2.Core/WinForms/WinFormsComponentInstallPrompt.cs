using NAPS2.Dependencies;
using NAPS2.Lang.Resources;
using System.Windows.Forms;

namespace NAPS2.WinForms
{
    public class WinFormsComponentInstallPrompt : IComponentInstallPrompt
    {
        private readonly IFormFactory formFactory;

        public WinFormsComponentInstallPrompt(IFormFactory formFactory)
        {
            this.formFactory = formFactory;
        }

        public bool PromptToInstall(ExternalComponent component, string promptText)
        {
            if (MessageBox.Show(promptText, MiscResources.DownloadNeeded, MessageBoxButtons.YesNo) != DialogResult.Yes)
                return component.IsInstalled;
            var progressForm = formFactory.Create<FDownloadProgress>();
            progressForm.QueueFile(component);
            progressForm.ShowDialog();
            return component.IsInstalled;
        }
    }
}
