using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;

namespace Moondancer
{
    public class VisualProfile : ObservableObject
    {
        private string visualMode = "Cover";
        public string VisualMode { get => visualMode; set => SetValue(ref visualMode, value); }
        private string logoAspectRatio = "16:9";
        public string LogoAspectRatio { get => logoAspectRatio; set => SetValue(ref logoAspectRatio, value); }
    }

    public class MoondancerSettings : ObservableObject
    {
        // Legacy keys retained to migrate the 0.4.1 Fullscreen preferences.
        private string visualMode = "Cover";
        public string VisualMode { get => visualMode; set => SetValue(ref visualMode, value); }
        private string logoAspectRatio = "16:9";
        public string LogoAspectRatio { get => logoAspectRatio; set => SetValue(ref logoAspectRatio, value); }

        private VisualProfile desktop = new VisualProfile { VisualMode = "Cover", LogoAspectRatio = "1:1" };
        public VisualProfile Desktop { get => desktop; set => SetValue(ref desktop, value); }
        private VisualProfile fullscreen = new VisualProfile { VisualMode = "Cover", LogoAspectRatio = "16:9" };
        public VisualProfile Fullscreen { get => fullscreen; set => SetValue(ref fullscreen, value); }
        // Null in pre-0.5 settings, explicit version thereafter.
        public int ProfilesVersion { get; set; }
    }

    public class MoondancerSettingsViewModel : ObservableObject, ISettings
    {
        private readonly Moondancer plugin;
        private MoondancerSettings editingClone;
        private MoondancerSettings settings;
        public MoondancerSettings Settings { get => settings; set { settings = value; OnPropertyChanged(); } }

        public MoondancerSettingsViewModel(Moondancer plugin)
        {
            this.plugin = plugin;
            Settings = plugin.LoadPluginSettings<MoondancerSettings>() ?? new MoondancerSettings();
            if (Settings.ProfilesVersion < 1)
            {
                // Preserve the previously selected global mode and frame for Fullscreen.
                Settings.Fullscreen = new VisualProfile {
                    VisualMode = Settings.VisualMode ?? "Cover",
                    LogoAspectRatio = Settings.LogoAspectRatio ?? "16:9"
                };
                Settings.Desktop = new VisualProfile { VisualMode = "Cover", LogoAspectRatio = "1:1" };
                Settings.ProfilesVersion = 1;
                plugin.SavePluginSettings(Settings);
            }
            if (Settings.Desktop == null) Settings.Desktop = new VisualProfile { VisualMode = "Cover", LogoAspectRatio = "1:1" };
            if (Settings.Fullscreen == null) Settings.Fullscreen = new VisualProfile();
        }
        public void BeginEdit() { editingClone = Serialization.GetClone(Settings); }
        public void CancelEdit() { if (editingClone != null) Settings = editingClone; }
        public void EndEdit() { plugin.SavePluginSettings(Settings); }
        public bool VerifySettings(out List<string> errors)
        {
            errors = new List<string>();
            Validate(Settings.Desktop, "Desktop", errors);
            Validate(Settings.Fullscreen, "Fullscreen", errors);
            return errors.Count == 0;
        }
        private static void Validate(VisualProfile profile, string name, List<string> errors)
        {
            if (profile == null) { errors.Add(name + ": falta el perfil."); return; }
            if (profile.VisualMode != "Cover" && profile.VisualMode != "Icon" && profile.VisualMode != "Background" && profile.VisualMode != "Logo" && profile.VisualMode != "BackgroundLogo")
                errors.Add(name + ": modalidad no válida.");
            if (profile.LogoAspectRatio != "16:9" && profile.LogoAspectRatio != "1:1")
                errors.Add(name + ": marco debe ser 16:9 o 1:1.");
        }
    }
}
