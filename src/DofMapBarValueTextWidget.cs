using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar;

namespace DaysOfFood
{
    /// <summary>
    /// The vanilla map bar value text, plus DofBlinkReds: while it's on, the normal and warning colors become
    /// dark and bright red, so toggling the warning blinks between them. Values without the binding keep it off.
    /// </summary>
    public class DofMapBarValueTextWidget : MapBarCustomValueTextWidget
    {
        private static readonly Color DarkRed = Color.ConvertStringToColor("#7A0A0AFF");
        private static readonly Color BrightRed = Color.ConvertStringToColor("#FF2626FF");

        private bool _blinkReds;
        private Color _normalColor;
        private Color _warningColor;

        public DofMapBarValueTextWidget(UIContext context) : base(context)
        {
        }

        // No OnPropertyChanged call: the value only flows from the view model, and the call's overloads would
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
    }
}
