using System.Collections.Generic;
using Better.EditorTools.SettingsTools;
using Better.Interactor.Runtime;
using UnityEditor;

namespace Better.Interactor.EditorAddons
{
    internal class InteractionSettingProvider : ProjectSettingsProvider<InteractionSettings>
    {
        private readonly Editor _editor;

        public InteractionSettingProvider() : base(ProjectSettingsToolsContainer<InteractionSettingsTool>.Instance, SettingsScope.Project)
        {
            keywords = new HashSet<string>(new[] { "Better", "Interaction", "Layers" });
            _editor = Editor.CreateEditor(_settings);
        }

        [MenuItem(InteractionSettingsTool.MenuItemPrefix + "/" + ProjectSettingsRegisterer.HighlightPrefix, false, 999)]
        private static void Highlight()
        {
            SettingsService.OpenProjectSettings(ProjectSettingsToolsContainer<InteractionSettingsTool>.Instance.ProjectSettingKey);
        }

        protected override void DrawGUI()
        {
            _editor.OnInspectorGUI();
        }
    }
}