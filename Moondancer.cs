using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Moondancer
{
    public class Moondancer : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        public MoondancerSettingsViewModel settings { get; private set; }

        public override Guid Id { get; } = Guid.Parse("022d2537-7fb5-49d1-a16d-654ebbef11eb");

        public Moondancer(IPlayniteAPI api) : base(api)
        {
            settings = new MoondancerSettingsViewModel(this);
            AddSettingsSupport(new AddSettingsSupportArgs
            {
                SourceName = "Moondancer",
                SettingsRoot = "settings.Settings"
            });
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }









        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new MoondancerSettingsView { DataContext = settings };
        }
    }
}