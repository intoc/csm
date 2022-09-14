using System;
using System.Linq;

namespace csm.Models; 
public class SettingsChangedEventArgs : EventArgs {
    public string SettingsFile { get; private set; }
    public string Message { get; private set; }
    public bool Passed { get; private set; }

    public SettingsChangedEventArgs(string settingsPath, string action, bool passed) {
        Passed = passed;
        SettingsFile = settingsPath;
        Message = string.Format("{0}: {1}", action, SettingsFile);
    }
}
