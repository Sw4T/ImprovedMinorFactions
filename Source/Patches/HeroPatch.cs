﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace ImprovedMinorFactionsBeta.Source.Patches
{
    // patch to prevent crashes from assumption about Gang Leaders living in Towns. Use Helpers.IsMFGangLeader instead.
    [HarmonyPatch(typeof(Hero), "get_IsGangLeader")]
    public class HeroIsGangLeaderPatch
    {
        static void Postfix(Hero __instance, ref bool __result)
        {
            if (__result == true && Helpers.IsMFHideout(__instance.CurrentSettlement))
                __result = false;
        }
    }

    [HarmonyPatch(typeof(Hero), "get_IsNotable")]
    public class HeroIsNotablePatch
    {
        static void Postfix(Hero __instance, ref bool __result)
        {
            if (__result == false && Helpers.IsMFGangLeader(__instance))
                __result = true;
        }
    }

    [HarmonyPatch(typeof(Hero), "SetTraitLevel")]
    public class HeroSetTraitLevelPatch
    {
        static bool Prefix(TraitObject trait)
        {
            if (trait == null)
                return false;
            return true;
        }
    }
}
