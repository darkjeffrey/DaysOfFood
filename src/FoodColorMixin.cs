using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Library;

namespace DaysOfFood
{
    /// <summary>
    /// Adds DofBlinkReds to the map bar items, for DofMapBarValueTextWidget to bind to. Only the food item's
    /// value ever changes.
    /// </summary>
    [ViewModelMixin]
    public sealed class FoodColorMixin : BaseViewModelMixin<MapInfoItemVM>
    {
        private static FoodColorMixin _food;
        private bool _blinkReds;

        public FoodColorMixin(MapInfoItemVM vm) : base(vm)
        {
            if (vm?.ItemId == "food") _food = this;
        }

        public static void SetFoodBlinkReds(bool on)
        {
            if (_food != null) _food.DofBlinkReds = on;
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

        public override void OnFinalize()
        {
            base.OnFinalize();
            if (_food == this) _food = null;
        }
    }
}
