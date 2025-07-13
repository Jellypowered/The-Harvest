using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace TheHarvest
{
    [StaticConstructorOnStartup]
    public static class TheHarvestPatcher
    {
        static TheHarvestPatcher()
        {
            Harmony.DEBUG = true;
            new Harmony("com.jelly.theharvest").PatchAll();
        }
    }
    public class TheHarvestMod : Mod
    {
        public static TheHarvestSettings Settings;

        public TheHarvestMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<TheHarvestSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "TheHarvest".Translate();
        }
    }

    public class TheHarvestSettings : ModSettings
    {
        public bool deep_harvest = false;

        public void DoWindowContents(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            listing.CheckboxLabeled("TheHarvest.deep_harvest.name".Translate(), ref deep_harvest, "TheHarvest.deep_harvest.desc".Translate());
            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref deep_harvest, "deep_harvest", false);
            base.ExposeData();
        }
    }
}
