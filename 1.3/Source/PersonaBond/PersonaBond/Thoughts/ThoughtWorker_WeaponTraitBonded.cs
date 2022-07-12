using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicCustomization;
using Verse;
using VanillaPersonaWeaponsExpanded;
using RimWorld;
using UnityEngine;

namespace PersonaBond
{

    
    public class ThoughtWorker_WeaponTraitBonded : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            Thing weapon = p.equipment.bondedWeapon;
            if(weapon != null)
            {
                if (PersonaBond_Settings.disableWeaponTraits) { return ThoughtState.Inactive; }
                var comp = weapon.TryGetComp<CompBladelinkWeapon>();
                if(comp != null)
                {
                    if ((comp.TraitsListForReading.Contains(InternalDefOf.PB_BadBond) && def.defName == "PB_ThoughtWeaponsHate") ||
                        ( comp.TraitsListForReading.Contains(InternalDefOf.PB_AmazingBond) && def.defName== "PB_ThoughtWeaponTrueBond"))
                    {
                        return ThoughtState.ActiveAtStage(0);
                    }
                }
            }
            return ThoughtState.Inactive;
        }

        public override string PostProcessLabel(Pawn p, string label)
        {
            Thing thing = p.equipment.bondedWeapon;
            var comp = thing.TryGetComp<CompGeneratedNames>();
            if( comp != null)
            {
                return label.Formatted(comp.Name.Named("WEAPON"));
            }
            return thing.Label;
        }
        public override string PostProcessDescription(Pawn p, string description)
        {
            return description.Formatted(p.equipment.bondedWeapon.Named("WEAPON"));
        }
    }
    public class WeaponTraitWorker_Bonds : WeaponTraitWorker
    {
        //This ended up being completely useless \o/ Leaving just in case
        //I have to have a harmony patch because by the time this gets call any reference to the weapon in pawn is gone and weapon isnt passed. Fucking whyyy
        public override void Notify_Unbonded(Pawn pawn)
        {
            base.Notify_Unbonded(pawn);
        }
    }

}
