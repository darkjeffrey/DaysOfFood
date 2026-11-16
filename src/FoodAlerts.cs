using System;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace DaysOfFood
{
    /// <summary>
    /// Low-food alerts: an announcement at the top of the screen and a dismissable map notification (like the ones
    /// for wars). Each fires once when food falls to Low and once more at Critical; recovering re-arms them.
    /// </summary>
    [HarmonyPatch(typeof(MapNotificationVM))]
    internal static class FoodAlerts
    {
        private static FoodLevel _alertedLevel;
        private static Campaign _campaign;
        private static MapNotificationVM _notifications;
        private static MapNotificationItemBaseVM _shownNotification;

        public static void OnLevel(FoodLevel level, int wholeDays)
        {
            if (!ReferenceEquals(_campaign, Campaign.Current)) // new game or loaded save: alert again
            {
                _campaign = Campaign.Current;
                _alertedLevel = FoodLevel.Normal;
            }

            FoodLevel previous = _alertedLevel;
            _alertedLevel = level;
            if (level <= previous) return;

            TextObject message = Message(level == FoodLevel.Critical, wholeDays);
            if (DofSettings.Current.AlertTop)
                MBInformationManager.AddQuickInformation(message);
            if (DofSettings.Current.AlertMapNotification)
                ShowNotification(level == FoodLevel.Critical, message);
            Log.Debug($"alert: {level}, {wholeDays} whole days");
        }

        private static TextObject Message(bool critical, int wholeDays)
        {
            if (wholeDays == 0)
                return new TextObject("{=dof_alert_none}Your party will run out of food within a day!");
            if (wholeDays == 1)
                return critical
                    ? new TextObject("{=dof_alert_critical_one}Food is almost gone: 1 day left!")
                    : new TextObject("{=dof_alert_low_one}Food is running low: 1 day left.");
            return (critical
                    ? new TextObject("{=dof_alert_critical}Food is almost gone: {DAYS} days left!")
                    : new TextObject("{=dof_alert_low}Food is running low: {DAYS} days left."))
                .SetTextVariable("DAYS", wholeDays);
        }

        private static void ShowNotification(bool critical, TextObject message)
        {
            if (_notifications == null) return;

            // Keep a single food notification: replace the previous one.
            if (_shownNotification != null && _notifications.NotificationItems.Contains(_shownNotification))
                _shownNotification.ExecuteRemove();

            var data = new LowFoodNotification(message, critical);
            _notifications.AddMapNotification(data);
            _shownNotification = _notifications.NotificationItems.FirstOrDefault(item => item.Data == data);
        }

        /// <summary>The map's notification list fills its data-to-item type table here; add ours.</summary>
        [HarmonyPostfix, HarmonyPatch("PopulateTypeDictionary")]
        private static void AfterPopulateTypeDictionary(MapNotificationVM __instance)
        {
            try
            {
                __instance.RegisterMapNotificationType(typeof(LowFoodNotification), typeof(LowFoodNotificationItemVM));
                _notifications = __instance;
            }
            catch (Exception e)
            {
                Log.Error("PopulateTypeDictionary", e);
            }
        }

        [HarmonyPrefix, HarmonyPatch(nameof(MapNotificationVM.OnFinalize))]
        private static void BeforeOnFinalize(MapNotificationVM __instance)
        {
            if (_notifications != __instance) return;
            _notifications = null;
            _shownNotification = null;
        }
    }

    /// <summary>
    /// Only ever given to the map's notification list, never to CampaignInformationManager, so it isn't saved.
    /// </summary>
    public sealed class LowFoodNotification : InformationData
    {
        private readonly bool _critical;

        public LowFoodNotification(TextObject message, bool critical) : base(message)
        {
            _critical = critical;
        }

        public override TextObject TitleText => _critical
            ? new TextObject("{=dof_notif_title_critical}Out of Food Soon")
            : new TextObject("{=dof_notif_title_low}Low Food");

        public override string SoundEventPath => "event:/ui/notification/army_dispersion";
    }

    public sealed class LowFoodNotificationItemVM : MapNotificationItemBaseVM
    {
        public LowFoodNotificationItemVM(LowFoodNotification data) : base(data)
        {
            NotificationIdentifier = "armycreation"; // the picture; the game has no food one
            _onInspect = ExecuteRemove;              // clicking it just dismisses it
        }
    }
}
