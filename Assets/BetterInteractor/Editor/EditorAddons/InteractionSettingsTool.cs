using Better.EditorTools.SettingsTools;
using Better.Interactor.Runtime;

namespace Better.Interactor.EditorAddons
{
    public class InteractionSettingsTool : ProjectSettingsTools<InteractionSettings>
    {
        private const string SettingMenuItem = "Interaction";
        public const string MenuItemPrefix = ProjectSettingsRegisterer.BetterPrefix + "/" + SettingMenuItem;

        public InteractionSettingsTool() : base(SettingMenuItem, SettingMenuItem, new string[]
            { ProjectSettingsRegisterer.BetterPrefix, SettingMenuItem, ProjectSettingsRegisterer.ResourcesPrefix })
        {
        }
    }
}