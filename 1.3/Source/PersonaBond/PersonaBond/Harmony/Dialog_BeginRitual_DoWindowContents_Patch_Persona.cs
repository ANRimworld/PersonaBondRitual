using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using VanillaPersonaWeaponsExpanded;
using GraphicCustomization;
using Verse.Steam;


namespace PersonaBond
{

    [HarmonyPatch(typeof(Dialog_BeginRitual))]
    [HarmonyPatch("DoWindowContents")]

    //This patch adds stuff selection
    public static class Dialog_BeginRitual_DoWindowContents_Patch_Persona
    {
        public static void Postfix(Dialog_BeginRitual __instance, Rect inRect, Precept_Ritual ___ritual)
        {

            Precept_Ritual ritual = ___ritual;
            if (ritual?.def.defName != "PB_BondRitual")
            {
                return;
            }

            List<Thing> weapons = new List<Thing>();

            RitualBehaviorWorker_BondRitual behavior = (RitualBehaviorWorker_BondRitual)ritual.behavior;


            var selectWeapon = new Rect(inRect.xMax - buttonDimensions.x, inRect.yMax - 76f - buttonDimensions.y, buttonDimensions.x, buttonDimensions.y);

            DrawButton(selectWeapon, behavior.personaWeapon?.TryGetComp<CompGeneratedNames>().Name ?? "Select Weapon", delegate
            {
                var floatOptions = new List<FloatMenuOption>();
                List<ThingDef> thingDefs = behavior.AllPersonaWeapons.ToList();
                for(int i = 0; i < thingDefs.Count; i++)
                {
                    List<Thing> temp = Find.CurrentMap.listerThings.ThingsOfDef(thingDefs[i]);
                    weapons.AddRange(temp);
                }
                foreach (Thing thing in weapons)
                {
                    if (!thing.TryGetComp<CompBladelinkWeapon>().Biocoded && thing.TryGetComp<CompBladelinkWeapon>().Biocodable)
                    {
                        floatOptions.Add(new FloatMenuOption(thing.LabelCap, delegate
                        {
                            behavior.personaWeapon = thing;
                        },thing.def
                        ));
                    }

                }
                Find.WindowStack.Add(new FloatMenu(floatOptions,"PB_SelectPersonaWeapon".Translate(),true));
            });
            if(behavior.personaWeapon != null)
            {
                string trait = string.Join(", ", behavior.personaWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Select(x => x.label).ToList());
                Vector2 vec = Text.CalcSize(trait);
                var traitDisplay = new Rect(inRect.xMax - vec.x, selectWeapon.y - selectWeapon.height, vec.x, vec.y);
                Widgets.Label(traitDisplay, trait);
            }

        }

        private static void DrawButton(Rect rect, string label, Action action)
        {
            if (Widgets.ButtonText(rect, label))
            {
                action();
            }

        }


        private static Vector2 buttonDimensions = new Vector2(200f, 24f);
    }


}
