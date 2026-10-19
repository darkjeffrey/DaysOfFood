# Days of Food

A Mount & Blade II: Bannerlord mod (v1.4.8) that shows how many **days your food will last** on the campaign map bar, for example **12d**, instead of how many food items you carry.

- **Red** when food runs low (3 days or less by default).
- **Blinking dark and bright red** when it's nearly gone (1 day or less by default). It keeps blinking while the game is paused.
- **Alerts** when food first drops into each range: an announcement at the top of the screen and a dismissable map notification, like the ones for wars. Buying food back above a range re-arms its alert.

Days left is the party's food divided by what it eats per day, rounded down, so **4d** means at least 4 full days. It shows **-** when the party eats nothing. The food tooltip and the short flash when your food goes up or down are vanilla.

Nothing is stored in your save, so you can add or remove the mod mid-campaign.

## Options (Mod Options)

| Option | Default | |
|---|---|---|
| Enable | On | Off brings back the vanilla food count. |
| Show tenths of a day | Off | 4.5d instead of 4d. |
| Red at (days or less) | 3 | 0 means less than one day. |
| Blinking red at (days or less) | 1 | Wins over plain red. |
| Blink speed (seconds) | 0.5 | Length of each half of a blink. |
| Blink between dark and bright red | On | Off blinks between the normal color and red. |
| Announcement at the top of the screen | On | |
| Map notification | On | Click or right-click it to dismiss. |
| Debug messages | Off | Logs food, consumption, days and level to the chat log when they change. |

## Requirements

Bannerlord.Harmony, ButterLib, UIExtenderEx and Mod Configuration Menu v5. Load Days of Food after them.

## How it works

| File | |
|---|---|
| `FoodDaysDisplay.cs` | Harmony postfixes on `MapInfoVM`: writes the days text and red warning after each map bar refresh, and flips the warning every frame to blink. |
| `FoodAlerts.cs` | The announcement and map notification. The notification only goes to the map's notification list, never to the campaign's saved notices. |
| `MapBarPrefabPatch.cs`, `DofMapBarValueTextWidget.cs`, `FoodColorMixin.cs` | UIExtenderEx swaps the map bar's value text for a subclass that can switch to dark/bright red, bound to a property only the food item sets. Every other value keeps its vanilla colors. |
| `DofSettings.cs` | The MCM page. |

Mods that replace the whole `MapBar` prefab may conflict.

## Building

Run `dotnet build -c Release` in `src`. The build installs the mod into `Modules\DaysOfFood` in the game folder; pass `/p:GameFolder="..."` if the game isn't in the default Steam location.

## Translations

All English text is in the code as `{=id}English`. `Module/ModuleData/Languages/std_DaysOfFood.xml` lists every string. To translate, add a language folder (for example `DE`) with a `language_data.xml` and a copy of that file with each `text` translated, keeping the ids and `{VARIABLES}`.

## License

[MIT](LICENSE)

## Support

If you enjoy the mod, you can support me on [Ko-fi](https://ko-fi.com/darkjeffrey).
