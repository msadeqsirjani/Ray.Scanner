using NAPS2.Config;
using System;

namespace NAPS2.Dependencies
{
    public class ComponentManager
    {
        private readonly AppConfigManager appConfigManager;

        private string basePath;

        public ComponentManager(AppConfigManager appConfigManager)
        {
            this.appConfigManager = appConfigManager;
        }

        public string BasePath
        {
            get
            {
                if (basePath != null) return basePath;
                var customPath = appConfigManager.Config.ComponentsPath;
                basePath = string.IsNullOrWhiteSpace(customPath)
                    ? Paths.Components
                    : Environment.ExpandEnvironmentVariables(customPath);
                return basePath;
            }
        }
    }
}
