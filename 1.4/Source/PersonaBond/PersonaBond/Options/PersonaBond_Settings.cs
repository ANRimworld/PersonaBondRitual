using RimWorld;
using UnityEngine;
using Verse;
using System;


namespace PersonaBond
{


    public class PersonaBond_Settings : ModSettings

    {



        public static bool disableWeaponTraits = false;
        public static bool randomBondChance = false;



        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref disableWeaponTraits, "disableWeaponTraits", false, true);
            Scribe_Values.Look(ref randomBondChance, "randomBondChance", false, true);



        }

        public static void DoWindowContents(Rect inRect)
        {
            Listing_Standard ls = new Listing_Standard();


            ls.Begin(inRect);

            ls.CheckboxLabeled("PB_DiableWeaponTraits".Translate(), ref disableWeaponTraits, "PB_DiableWeaponTraits_ToolTip".Translate());

            ls.CheckboxLabeled("PB_RandomBondChance".Translate(), ref randomBondChance, "PB_RandomBondChance_ToolTip".Translate());

            ls.End();
        }



    }










}
