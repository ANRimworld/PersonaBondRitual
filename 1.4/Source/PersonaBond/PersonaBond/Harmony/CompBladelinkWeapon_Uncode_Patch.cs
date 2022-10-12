using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;

using System;


namespace PersonaBond
{

    [HarmonyPatch(typeof(CompBladelinkWeapon))]
    [HarmonyPatch("UnCode")]
    //This patch exists because they dont want you having any relevant info on your weapontrait worker when a pawn dies. Exaclty none
    public static class CompBladelinkWeapon_Uncode_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(CompBladelinkWeapon __instance)
        {
            //doing it as a prefix so nothing gets removed
            foreach(WeaponTraitDef trait in __instance.TraitsListForReading.ToList())
            {
                if(trait == InternalDefOf.PB_AmazingBond || trait == InternalDefOf.PB_BadBond) //Remove ritual trails on death
                {
                    __instance.TraitsListForReading.Remove(trait);
                }
            }

        }

    }


}
