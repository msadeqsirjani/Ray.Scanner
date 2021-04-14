using NAPS2.Config;

namespace NAPS2.Scan
{
    /// <summary>
    /// A class used to help keep profile names consistent across forms.
    ///
    /// TODO: This should probably be replaced by an event handler system.
    /// </summary>
    public class ProfileNameTracker
    {
        private readonly IUserConfigManager userConfigManager;

        public ProfileNameTracker(IUserConfigManager userConfigManager)
        {
            this.userConfigManager = userConfigManager;
        }

        public void RenamingProfile(string oldName, string newName)
        {
            if (string.IsNullOrEmpty(oldName))
            {
                return;
            }

            if (userConfigManager.Config.LastBatchSettings == null) return;
            if (userConfigManager.Config.LastBatchSettings.ProfileDisplayName != oldName) return;
            userConfigManager.Config.LastBatchSettings.ProfileDisplayName = newName;
            userConfigManager.Save();
        }

        public void DeletingProfile(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (userConfigManager.Config.LastBatchSettings == null) return;
            if (userConfigManager.Config.LastBatchSettings.ProfileDisplayName != name) return;
            userConfigManager.Config.LastBatchSettings.ProfileDisplayName = null;
            userConfigManager.Save();
        }
    }
}
