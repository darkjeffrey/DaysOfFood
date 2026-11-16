using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar;

namespace DaysOfFood
{
    /// <summary>
    /// The vanilla map bar value text, plus two switches bound to FoodBarMixin. Values without the bindings keep
    /// both off.
    /// DofBlinkReds: the normal and warning colors become dark and bright red, so toggling the warning blinks
    /// between them.
    /// DofSeparator: shows the line between the two text lines (the hidden "DofSeparator" child that
    /// MapBarPrefabPatch adds).
    /// </summary>
    public class DofMapBarValueTextWidget : MapBarCustomValueTextWidget
    {
        public const string SeparatorId = "DofSeparator";

        private static readonly Color DarkRed = Color.ConvertStringToColor("#7A0A0AFF");
        private static readonly Color BrightRed = Color.ConvertStringToColor("#FF2626FF");

        private bool _blinkReds;
        private bool _separator;
        private Color _normalColor;
        private Color _warningColor;

        public DofMapBarValueTextWidget(UIContext context) : base(context)
        {
        }

        // No OnPropertyChanged calls: the values only flow from the view model, and the call's overloads would
        // need a System.Numerics.Vectors reference.
        public bool DofBlinkReds
        {
            get => _blinkReds;
            set
            {
                if (value == _blinkReds) return;
                _blinkReds = value;
                if (value)
                {
                    _normalColor = NormalColor;
                    _warningColor = WarningColor;
                    NormalColor = DarkRed;
                    WarningColor = BrightRed;
                }
                else
                {
                    NormalColor = _normalColor;
                    WarningColor = _warningColor;
                }
            }
        }

        public bool DofSeparator
        {
            get => _separator;
            set
            {
                _separator = value;
                ShowSeparator();
            }
        }

        // The child may be attached after the binding has set DofSeparator.
        protected override void OnChildAdded(Widget child)
        {
            base.OnChildAdded(child);
            ShowSeparator();
        }

        private void ShowSeparator()
        {
            for (int i = 0; i < ChildCount; i++)
            {
                Widget child = GetChild(i);
                if (child.Id == SeparatorId) child.IsVisible = _separator;
            }
        }
    }
}
