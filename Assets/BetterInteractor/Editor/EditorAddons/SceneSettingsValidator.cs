using Better.EditorTools.SettingsTools;
using UnityEditor;

namespace Better.Interactor.EditorAddons
{
    [InitializeOnLoad]
    public static class InteractionSettingsValidator
    {
        static InteractionSettingsValidator()
        {
            EditorApplication.delayCall += DelayCall;
        }

        private static void DelayCall()
        {
            Validate();
        }
        private static void Validate()
        {
            var settings = ProjectSettingsToolsContainer<InteractionSettingsTool>.Instance.LoadOrCreateScriptableObject();
        }
    }
}