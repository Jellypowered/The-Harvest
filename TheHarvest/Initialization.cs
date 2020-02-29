using HugsLib;
using HugsLib.Settings;
using System;
using Verse;
using RimWorld;

namespace TheHarvest
{
    [StaticConstructorOnStartup]
    public class Initialization : ModBase
    {
        public static SettingHandle<bool> deep_harvest;

        public override string ModIdentifier => "TheHarvest";

        public override void DefsLoaded()
        {
            deep_harvest = Settings.GetHandle<bool>("deep_harvest", "TheHarvest.deep_harvest.name".Translate(), "TheHarvest.deep_harvest.desc".Translate(), false);
        }
    }
}