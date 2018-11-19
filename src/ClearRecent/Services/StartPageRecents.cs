using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ClearRecent.Models;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.CodeContainerManagement;

namespace ClearRecent.Services
{
    internal class StartPageRecents
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Files files;

        internal StartPageRecents(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            files = new Files();
        }

        internal bool ProjectsFound() =>
            GetRecents(GetManager()).Count > 0;

        internal bool NonFavoriteProjectsFound =>
            GetRecents(GetManager()).Any(kv => !kv.Value.IsFavorite);

        internal void ClearAllProjects() => Clear(_ => true);
        internal void ClearMissingProjects() => Clear((kv => files.Missing(kv.Key)));
        internal void ClearNonFavoriteProjects() => Clear((kv => !kv.Value.IsFavorite));

        private void Clear(Func<KeyValuePair<string , ProjectSettingValue>, bool> shouldDelete)
        {
            var manager = GetManager();
            var recents = GetRecents(manager);

            if (recents.Count == 0) { return; }

            var registry = GetRegistry(manager);

            foreach (var kv in recents)
            {
                if (shouldDelete(kv)) { registry.RemoveAsync(kv.Key); }
            }
        }

        private ISettingsManager GetManager() =>
            serviceProvider.GetService(typeof(SVsSettingsPersistenceManager)) as ISettingsManager;

        private IDictionary<string, ProjectSettingValue> GetRecents(ISettingsManager manager) {
            var settingsList = manager.GetOrCreateList("CodeContainers.Offline", isMachineLocal: true);
            return 
                settingsList
                    .Keys
                    .ToDictionary(keySelector: key => key, elementSelector: key => settingsList.GetValueOrDefault<ProjectSettingValue>(key));
        }
        private static CodeContainerRegistry GetRegistry(ISettingsManager manager) =>
            new CodeContainerRegistry(manager);

        [Guid("9b164e40-c3a2-4363-9bc5-eb4039def653")]
        private class SVsSettingsPersistenceManager
        {
        }
    }
}
