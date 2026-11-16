using System;
using System.Globalization;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Localization;

namespace DaysOfFood
{
    internal enum FoodLevel
    {
        Normal,
        Low,      // red
        Critical, // blinking red
    }

    /// <summary>
    /// Shows days of food instead of the food count on the campaign map bar. Vanilla MapInfoVM.UpdatePlayerInfo
    /// rewrites the food item at every map bar refresh, so the postfix rewrites its text and warning right after.
    /// IntValue (the food amount) is left alone: it drives the short flash when food goes up or down, and the
    /// optional second line.
    /// </summary>
    [HarmonyPatch(typeof(MapInfoVM))]
    internal static class FoodDaysDisplay
    {
        private static readonly AccessTools.FieldRef<MapInfoVM, MapInfoItemVM> FoodItem =
            AccessTools.FieldRefAccess<MapInfoVM, MapInfoItemVM>("_foodInfo");

        private static FoodLevel _level;
        private static string _lastDebug;

        [HarmonyPostfix, HarmonyPatch("UpdatePlayerInfo")]
        private static void AfterUpdatePlayerInfo(MapInfoVM __instance)
        {
            try { Update(FoodItem(__instance)); }
            catch (Exception e) { Log.Error("UpdatePlayerInfo", e); }
        }

        /// <summary>Runs every frame, also while the game is paused, which keeps the number blinking.</summary>
        [HarmonyPostfix, HarmonyPatch(nameof(MapInfoVM.Tick))]
        private static void AfterTick(MapInfoVM __instance)
        {
            if (_level != FoodLevel.Critical || !DofSettings.Current.Enabled) return;
            try { FoodItem(__instance).HasWarning = BlinkIsRed(); }
            catch (Exception e) { Log.Error("Tick", e); }
        }

        private static void Update(MapInfoItemVM item)
        {
            MobileParty party = MobileParty.MainParty;
            if (item == null || party == null) return;

            DofSettings settings = DofSettings.Current;
            if (!settings.Enabled)
            {
                // Vanilla has already written its own text and warning; only our color needs undoing.
                _level = FoodLevel.Normal;
                FoodBarMixin.SetFood(false, false);
                return;
            }

            float eatenPerDay = -party.FoodChange;
            float? days = eatenPerDay > 0.0001f ? Math.Max(0f, party.Food / eatenPerDay) : (float?)null; // null: not eating
            int wholeDays = days.HasValue ? (int)Math.Min(days.Value, 100000f) : int.MaxValue;

            _level = wholeDays <= settings.BlinkDays ? FoodLevel.Critical
                : wholeDays <= settings.RedDays ? FoodLevel.Low
                : FoodLevel.Normal;

            // Vanilla has just set IntValue to the food amount.
            string daysText = Format(days, settings.ShowDecimal);
            item.Value = settings.ShowFoodAmount ? item.IntValue + "\n" + daysText : daysText;
            item.HasWarning = _level == FoodLevel.Low || (_level == FoodLevel.Critical && BlinkIsRed());
            FoodBarMixin.SetFood(_level == FoodLevel.Critical && settings.BlinkDarkRed, settings.ShowFoodAmount);
            FoodAlerts.OnLevel(_level, wholeDays);

            // Log only when what the player sees changes; the food amount itself drops a little at every refresh.
            string shown = item.Value.Replace("\n", " / ") + " " + _level;
            if (settings.DebugMessages && shown != _lastDebug)
            {
                _lastDebug = shown;
                Log.Debug($"food {party.Food:0.0}, eats {eatenPerDay:0.00}/day, shows {shown} (game's own whole days: {party.GetNumDaysForFoodToLast()})");
            }
        }

        private static string Format(float? days, bool tenths)
        {
            if (days == null) return "-";
            string number = days >= 1000f ? "999+"
                : tenths ? (Math.Floor(days.Value * 10f) / 10f).ToString("0.0", CultureInfo.CurrentCulture)
                : ((int)days.Value).ToString(CultureInfo.CurrentCulture);
            return new TextObject("{=dof_days_value}{DAYS}d").SetTextVariable("DAYS", number).ToString();
        }

        /// <summary>Real time, not campaign time, so it also blinks while paused.</summary>
        private static bool BlinkIsRed()
        {
            int halfPeriodMs = Math.Max(50, (int)(DofSettings.Current.BlinkSeconds * 1000f));
            return ((Environment.TickCount & int.MaxValue) / halfPeriodMs) % 2 == 0;
        }
    }
}
