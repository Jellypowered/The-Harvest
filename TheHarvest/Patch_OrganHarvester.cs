using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;


namespace TheHarvest.HarmonyPatches
{
    [HarmonyPatch(typeof(Pawn), "ButcherProducts")]
    public static class Patch_OrganHarvester
    {
        public static void Postfix(ref IEnumerable<Thing> __result, ref Pawn __instance, Pawn butcher, float efficiency)
        {
            __result = __result.CompackedItems(__instance);
        }

        private static IEnumerable<Thing> CompackedItems(this IEnumerable<Thing> list, Pawn pawn)
        {
            foreach (Thing thing in list)
                yield return thing;
            foreach (Thing thing in pawn.DetachValuableItems())
                yield return thing;
        }

        private static IEnumerable<Thing> DetachValuableItems(this Pawn p)
        {
            foreach (BodyPartRecord record in p.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined))
            {
                IEnumerable<Hediff> hediffs = from x in p.health.hediffSet.hediffs
                                              where x.Part == record
                                              select x;
                if (hediffs.Any())
                {
                    foreach (Hediff hediff in hediffs)
                    {
                        if (hediff.def.spawnThingOnRemoved != null)
                        {
                            Log.Message("[The Harvest] Obtained: " + hediff.def.spawnThingOnRemoved.defName);
                            yield return ThingMaker.MakeThing(hediff.def.spawnThingOnRemoved, null);
                        }
                    }
                }
                else
                {
                    if (record.def.spawnThingOnRemoved != null &&
                        (record.def.alive == false ||
                        (Initialization.deep_harvest.Value && !p.RaceProps.Animal)))
                    {
                        p.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, p, record), null, null);
                        Log.Message("[The Harvest] Obtained: " + record.def.spawnThingOnRemoved.defName);
                        yield return ThingMaker.MakeThing(record.def.spawnThingOnRemoved, null);
                    }
                }
            }
        }
    }
}
