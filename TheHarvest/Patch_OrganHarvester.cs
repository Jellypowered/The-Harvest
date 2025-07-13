using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace TheHarvest.HarmonyPatches
{
    [HarmonyPatch(typeof(Corpse), nameof(Corpse.ButcherProducts))]
    public static class Patch_OrganHarvester
    {
        public static void Postfix(ref IEnumerable<Thing> __result, Corpse __instance)
        {
            var innerPawn = __instance?.InnerPawn;
            if (innerPawn?.health?.hediffSet == null) return;

            var extraItems = innerPawn.DetachValuableItems().ToList();
            if (extraItems.Count == 0) return;

            __result = __result.Concat(extraItems);
        }

    private static readonly string[] RJWBodyPartKeywords = new[]
    {
    "Genital", "Penis", "Vagina", "Anus", "Breast", "Uterus", "Ovary", "Testicle"
    };
        private static IEnumerable<Thing> DetachValuableItems(this Pawn pawn)
        {
            var hediffSet = pawn.health.hediffSet;

            foreach (var part in hediffSet.GetNotMissingParts())
            {
                // ❌ Skip RJW body parts
                if (IsRJWBodyPart(part.def)) continue;

                // Case 1: part has hediffs that can spawn items
                var hediffs = hediffSet.hediffs.Where(h => h.Part == part);
                foreach (var hediff in hediffs)
                {
                    var spawnDef = hediff.def.spawnThingOnRemoved;
                    if (spawnDef != null)
                    {
                        yield return ThingMaker.MakeThing(spawnDef);
                    }
                }

                // Case 2: no hediffs, raw body part removal
                if (!hediffs.Any() &&
                    part.def.spawnThingOnRemoved != null &&
                    (!part.def.alive || (TheHarvestMod.Settings.deep_harvest && !pawn.RaceProps.Animal)))
                {
                    pawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, pawn, part));
                    yield return ThingMaker.MakeThing(part.def.spawnThingOnRemoved);
                }
            }
        }
        private static bool IsRJWBodyPart(BodyPartDef def)
        {
            if (def == null || string.IsNullOrEmpty(def.defName))
                return false;

            return RJWBodyPartKeywords.Any(keyword =>
                def.defName.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}

