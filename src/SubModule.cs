using System;
using Bannerlord.UIExtenderEx;
using Bannerlord.UIExtenderEx.ResourceManager;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace DaysOfFood
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                WidgetFactoryManager.Register(typeof(DofMapBarValueTextWidget));
                UIExtender uiExtender = UIExtender.Create("DaysOfFood");
                uiExtender.Register(typeof(SubModule).Assembly);
                uiExtender.Enable();

                new Harmony("darkjeffrey.daysoffood").PatchAll(typeof(SubModule).Assembly);
            }
            catch (Exception e)
            {
                Log.Error("OnSubModuleLoad", e);
            }
        }
    }
}
