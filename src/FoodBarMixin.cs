using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Library;

namespace DaysOfFood
{
    /// <summary>
    /// Adds DofBlinkReds and DofSeparator to the map bar items, for DofMapBarValueTextWidget to bind to. Only the
    /// food item's values ever change.
    /// </summary>
    [ViewModelMixin]
    public sealed class FoodBarMixin : BaseViewModelMixin<MapInfoItemVM>
    {
        private static FoodBarMixin _foodItem;
        private bool _blinkReds;
        private bool _separator;

        public FoodBarMixin(MapInfoItemVM vm) : base(vm)
        {
            if (vm?.ItemId == "food") _foodItem = this;
        }

        public static void SetFood(bool blinkReds, bool separator)
        {
            if (_foodItem == null) return;
            _foodItem.DofBlinkReds = blinkReds;
            _foodItem.DofSeparator = separator;
        }

        [DataSourceProperty]
        public bool DofBlinkReds
        {
            get => _blinkReds;
            set
            {
                if (value == _blinkReds) return;
                _blinkReds = value;
                OnPropertyChangedWithValue(value, nameof(DofBlinkReds));
            }
        }

        [DataSourceProperty]
        public bool DofSeparator
        {
            get => _separator;
            set
            {
                if (value == _separator) return;
                _separator = value;
                OnPropertyChangedWithValue(value, nameof(DofSeparator));
            }
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            if (_foodItem == this) _foodItem = null;
        }
    }
}
