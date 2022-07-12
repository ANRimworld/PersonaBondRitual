using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using System.Threading.Tasks;

namespace PersonaBond
{
    public class RitualOutcomeComp_BondTraits : RitualOutcomeComp_QualitySingleOffset
    {
        public override RitualOutcomeComp_Data MakeData()
        {
            return new RitualOutcomeComp_BondTraitsData();
        }
        public override bool Applies(LordJob_Ritual ritual)
        {
            return true;
        }
        public override float QualityOffset(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            return Count(ritual, data);
        }
        public float CountInternal(Pawn pawn, Precept_Ritual ritual, out string qualityDesc, RitualOutcomeComp_BondTraitsData data)
        {
            qualityDesc = "";
           
            float count = 0f;
            float max = 0f;
            var behavior = ritual.behavior as RitualBehaviorWorker_BondRitual;
            if(behavior.personaWeapon == null)//can't calculate till selected
            {
                return 0f;
            }
            if (PersonaBond_Settings.randomBondChance)
            {
                return data.GetChance(pawn, behavior.personaWeapon);
            }
            StringBuilder qualityDebug = new StringBuilder();
            CompBladelinkWeapon weapon = behavior.personaWeapon.TryGetComp<CompBladelinkWeapon>();
            List<WeaponTraitDef> wepTraits = weapon.TraitsListForReading;
            foreach(WeaponTraitDef wepTrait in wepTraits)
            {
                qualityDebug.AppendInNewLine("Weapon Trait: " + wepTrait.label);
                if (wepTrait.HasModExtension<PersonaRitualComp_Extension>())
                {
                    float locQuality;
                    var extension = wepTrait.GetModExtension<PersonaRitualComp_Extension>();
                    float curveCount = 0f;
                    if (!extension.traitDefStage.NullOrEmpty())
                    {
                        int traitCount = 0;
                        max += MaxValueOfCurve(extension.traitCurve);
                        foreach (KeyValuePair<TraitDef,int> kvp in extension.traitDefStage)
                        {                            
                            if (pawn.story.traits.HasTrait(kvp.Key, kvp.Value))
                            {
                                traitCount++;
                                qualityDebug.AppendInNewLine(" Trait " + kvp.Key.ToString());
                            }
  
                        }
                        curveCount = traitCount;
                        locQuality = extension.traitCurve.Evaluate(curveCount);
                        qualityDebug.AppendInNewLine(" Traits Quality: " + locQuality.ToStringPercent() + " Max Quality " + MaxValueOfCurve(extension.traitCurve).ToStringPercent());
                        count += locQuality;
                    }
                    if (extension.stat != null)
                    {
                        max += MaxValueOfCurve(extension.statCurve);
                        curveCount = pawn.GetStatValue(extension.stat);
                        qualityDebug.AppendInNewLine(" " + extension.stat.label + " Value: " + curveCount);
                        locQuality = extension.statCurve.Evaluate(curveCount);
                        qualityDebug.AppendInNewLine(" Skill Quality: " + locQuality.ToStringPercent() + " Max Quality " + MaxValueOfCurve(extension.statCurve).ToStringPercent());
                        count += locQuality;
                    }                    
                }

            }
            if(qualityDebug.Length > 0) 
            {
                qualityDebug.AppendInNewLine("Quality: " + count.ToStringPercent() + "/" + max.ToStringPercent() + " Max quality");
                qualityDesc = qualityDebug.ToString();
            }
            return count;
        }
        
        public override float Count(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            Pawn pawn = ritual.assignments.AssignedPawns("PB_Bonder").First();
            string qualityDesc;
            float count = CountInternal(pawn, ritual.Ritual, out qualityDesc, data as RitualOutcomeComp_BondTraitsData);
            return count;
        }
        public override ExpectedOutcomeDesc GetExpectedOutcomeDesc(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
        {
            Pawn pawn = assignments.AssignedPawns("PB_Bonder").First();
            string qualityDesc;
            float count = CountInternal(pawn, ritual, out qualityDesc, data as RitualOutcomeComp_BondTraitsData);
            return new ExpectedOutcomeDesc
            {
                label = LabelForDesc,
                present = count == 0f ? false : true,
                effect = this.ExpectedOffsetDesc(true, count),
                quality = count,
                positive = (count > 0),
                priority = 1f,                
                tip = qualityDesc
            };
        }
        public float MaxValueOfCurve(SimpleCurve curve)
        {

            return curve.Points[curve.PointsCount - 1].y;
           
        }

        public bool qualityDebug = true;

    }
    
}
