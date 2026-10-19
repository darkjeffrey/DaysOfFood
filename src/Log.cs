using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace DaysOfFood
{
    internal static class Log
    {
        private static readonly HashSet<string> Reported = new HashSet<string>();

        /// <summary>Chat log line while "Debug messages" is on. Not translated.</summary>
        public static void Debug(string text)
        {
            if (DofSettings.Current.DebugMessages)
                Show("[DaysOfFood] " + text, Colors.Gray);
        }

        /// <summary>Shows only the first error from each place, since some of them run every frame.</summary>
        public static void Error(string where, Exception e)
        {
            if (!Reported.Add(where)) return;
            Show(new TextObject("{=dof_err_generic}[DaysOfFood] error ({WHERE}): {ERROR}")
                .SetTextVariable("WHERE", where)
                .SetTextVariable("ERROR", e.Message)
                .ToString(), Colors.Red);
        }

        private static void Show(string text, Color color)
        {
            InformationManager.DisplayMessage(new InformationMessage(text, color));
        }
    }
}
