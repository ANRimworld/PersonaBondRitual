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
            if (PersonaBond_Settings.disableWeaponTraits) { return ThoughtState.Inactive; }
            Thing weapon = p.equipment.bondedWeapon;
            if(weapon != null)
            {
                if(p.equipment.Primary != weapon) { return ThoughtState.Inactive; }
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
        public override float MoodMultiplier(Pawn p) //A way to make the negative traits get better countered by true bond. 
        {
            if (PersonaBond_Settings.disableWeaponTraits) { return base.MoodMultiplier(p); }
            Thing weapon = p.equipment.bondedWeapon;
            if (weapon != null)
            {
                var comp = weapon.TryGetComp<CompBladelinkWeapon>();
                if (comp.TraitsListForReading.Contains(InternalDefOf.PB_AmazingBond))
                {
                    float value = comp.TraitsListForReading.Where(x=>x.bondedThought != null || x.killThought != null).Sum(x => x.marketValueOffset);
                    return MoodMultiplierCurve.Evaluate(value);
                }
                
            }
            return base.MoodMultiplier(p);
        }
        //between amazing base 6 to 12 to counteract things wailing if you have a true bond with it based on marketValue offset of traits
        private static readonly SimpleCurve MoodMultiplierCurve = new SimpleCurve
        {
            {
                new CurvePoint(-1000f, 2f),
                true
            },
            {
                new CurvePoint(-600f, 1.5f),
                true
            },
            {
                new CurvePoint(-200f, 1.1f),
                true
            },
            {
                new CurvePoint(0f, 1f),
                true
            }
        };

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
