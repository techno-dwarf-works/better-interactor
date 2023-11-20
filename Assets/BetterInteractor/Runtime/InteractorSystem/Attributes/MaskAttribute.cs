using System;
using System.Diagnostics;
using Better.Attributes.Runtime.Select;
using Better.Tools.Runtime;

namespace Better.Interactor.Runtime.Attributes
{
    [Conditional(BetterEditorDefines.Editor)]
    [AttributeUsage(AttributeTargets.Field)]
    public class MaskAttribute : DropdownAttribute
    {
        private static string selectorName = $"r:{nameof(InteractionSettings)}.{nameof(InteractionSettings.GetNamedMasks)}()";

        public MaskAttribute() : base(selectorName)
        {
        }

        public MaskAttribute(DisplayName displayName) : base(selectorName, displayName)
        {
        }

        public MaskAttribute(DisplayGrouping displayGrouping) : base(selectorName, displayGrouping)
        {
        }
    }
}