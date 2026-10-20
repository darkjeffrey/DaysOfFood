using System;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace DaysOfFood
{
    /// <summary>The "Days of Food" page in Mod Options (MCM).</summary>
    public sealed class DofSettings : AttributeGlobalSettings<DofSettings>
    {
        private const string General = "{=dof_mcm_group_general}General";
        private const string Colors = "{=dof_mcm_group_colors}Colors";
        private const string Alerts = "{=dof_mcm_group_alerts}Alerts";
        private const string Testing = "{=dof_mcm_group_testing}Testing";

        private static readonly DofSettings Defaults = new DofSettings();

        /// <summary>The saved settings, or the defaults while MCM isn't loaded yet.</summary>
        internal static DofSettings Current
        {
            get
            {
                try { return Instance ?? Defaults; }
                catch (Exception) { return Defaults; }
            }
        }

        public override string Id => "DaysOfFood";
        public override string FolderName => "DaysOfFood";
        public override string FormatType => "json2";

        // MCM doesn't translate DisplayName, so resolve it here. Before the text system is ready TextObject returns
        // the raw "{=id}text"; strip the id so the English name still shows.
        public override string DisplayName
        {
            get
            {
                string name = new TextObject("{=dof_mcm_title}Days of Food").ToString();
                return name.StartsWith("{=", StringComparison.Ordinal) ? name.Substring(name.IndexOf('}') + 1) : name;
            }
        }

        [SettingPropertyBool("{=dof_mcm_Enabled}Enable", Order = 0, RequireRestart = false,
            HintText = "{=dof_mcm_Enabled_hint}Show how many days your food will last on the campaign map bar instead of how many food items you carry. Off: the vanilla food count comes back at the next map bar refresh.")]
        [SettingPropertyGroup(General, GroupOrder = 0)]
        public bool Enabled { get; set; } = true;

        [SettingPropertyBool("{=dof_mcm_ShowDecimal}Show tenths of a day", Order = 1, RequireRestart = false,
            HintText = "{=dof_mcm_ShowDecimal_hint}On: show 4.5d. Off: show whole days only (4d), rounded down, so 4d means at least 4 full days.")]
        [SettingPropertyGroup(General, GroupOrder = 0)]
        public bool ShowDecimal { get; set; } = false;

        [SettingPropertyBool("{=dof_mcm_ShowFoodAmount}Also show the food amount", Order = 2, RequireRestart = false,
            HintText = "{=dof_mcm_ShowFoodAmount_hint}Show the vanilla food amount above the days left, on two lines with a line between them.")]
        [SettingPropertyGroup(General, GroupOrder = 0)]
        public bool ShowFoodAmount { get; set; } = false;

        [SettingPropertyInteger("{=dof_mcm_RedDays}Red at (days or less)", 0, 30, "0", Order = 0, RequireRestart = false,
            HintText = "{=dof_mcm_RedDays_hint}The number turns red when this many whole days of food or fewer are left. 0 means only when less than one day is left.")]
        [SettingPropertyGroup(Colors, GroupOrder = 1)]
        public int RedDays { get; set; } = 3;

        [SettingPropertyInteger("{=dof_mcm_BlinkDays}Blinking red at (days or less)", 0, 30, "0", Order = 1, RequireRestart = false,
            HintText = "{=dof_mcm_BlinkDays_hint}The number blinks red when this many whole days of food or fewer are left. This wins over plain red. 0 means only when less than one day is left.")]
        [SettingPropertyGroup(Colors, GroupOrder = 1)]
        public int BlinkDays { get; set; } = 1;

        [SettingPropertyFloatingInteger("{=dof_mcm_BlinkSeconds}Blink speed (seconds)", 0.2f, 2f, "0.0", Order = 2, RequireRestart = false,
            HintText = "{=dof_mcm_BlinkSeconds_hint}How long the number stays red, and then normal, in each blink.")]
        [SettingPropertyGroup(Colors, GroupOrder = 1)]
        public float BlinkSeconds { get; set; } = 0.5f;

        [SettingPropertyBool("{=dof_mcm_BlinkDarkRed}Blink between dark and bright red", Order = 3, RequireRestart = false,
            HintText = "{=dof_mcm_BlinkDarkRed_hint}On: the blinking number goes from dark red to bright red. Off: it goes from the normal color to red.")]
        [SettingPropertyGroup(Colors, GroupOrder = 1)]
        public bool BlinkDarkRed { get; set; } = true;

        [SettingPropertyBool("{=dof_mcm_AlertTop}Announcement at the top of the screen", Order = 0, RequireRestart = false,
            HintText = "{=dof_mcm_AlertTop_hint}Show a message at the top of the screen when food drops to the red days, and again when it drops to the blinking days.")]
        [SettingPropertyGroup(Alerts, GroupOrder = 2)]
        public bool AlertTop { get; set; } = true;

        [SettingPropertyBool("{=dof_mcm_AlertMapNotification}Map notification", Order = 1, RequireRestart = false,
            HintText = "{=dof_mcm_AlertMapNotification_hint}Show a dismissable notification on the right of the map, like the ones for wars and army dispersals, when food drops to the red days and again at the blinking days. Right click or click it to dismiss. It isn't kept in your save.")]
        [SettingPropertyGroup(Alerts, GroupOrder = 2)]
        public bool AlertMapNotification { get; set; } = true;

        [SettingPropertyBool("{=dof_mcm_DebugMessages}Debug messages", Order = 0, RequireRestart = false,
            HintText = "{=dof_mcm_DebugMessages_hint}Each time the days or the color change, show the food, daily consumption, days left and the chosen color in the chat log. Useful for checking the mod; turn it off for normal play.")]
        [SettingPropertyGroup(Testing, GroupOrder = 3)]
        public bool DebugMessages { get; set; } = false;
    }
}
