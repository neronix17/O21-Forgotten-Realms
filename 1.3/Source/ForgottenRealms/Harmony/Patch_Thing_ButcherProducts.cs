﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;
using AlienRace;

namespace ForgottenRealms
{

    [HarmonyPatch(typeof(Thing), "ButcherProducts")]
    public static class Patch_Thing_ButcherProducts
    {

        [HarmonyPostfix]
        public static void Postfix(Thing __instance, ref IEnumerable<Thing> __result, float efficiency)
        {
            if (ForgottenRealmsMod.settings.raceToggle_illithid)
            {
                Pawn pawn = __instance as Pawn;
                if (pawn != null && pawn.def.race.Humanlike && pawn.def.defName != "O21_FR_Illithid" && pawn.RaceProps.body == BodyDefOf.Human && pawn.Corpse.GetRotStage() == RotStage.Fresh)
                {
                    ThingDef_AlienRace raceDef = (ThingDef_AlienRace)pawn.def ?? null;
                    if (raceDef != null && raceDef.alienRace.compatibility.HasBlood && raceDef.alienRace.compatibility.IsFlesh && raceDef.alienRace.compatibility.IsSentient)
                    {
                        BodyPartRecord brain = pawn?.health?.hediffSet?.GetNotMissingParts()?.Where(x => x.def.defName == "Brain")?.FirstOrDefault();
                        if (brain != null)
                        {
                            __result = GenerateExtraProducts(__result, pawn, efficiency);
                        }
                    }
                }
            }
        }

        public static IEnumerable<Thing> GenerateExtraProducts(IEnumerable<Thing> things, Pawn pawn, float efficiency)
        {
            if (!things.EnumerableNullOrEmpty())
            {
                foreach (Thing thing in things)
                {
                    yield return thing;
                }
            }

            Thing brain = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("O21_FR_Illithid_HumanoidBrain"), null);
            brain.stackCount = 1;
            yield return brain;
        }
    }
}