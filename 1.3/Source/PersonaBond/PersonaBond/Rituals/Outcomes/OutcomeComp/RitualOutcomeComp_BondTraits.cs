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
        public override string GetDesc(LordJob_Ritual ritual = null, RitualOutcomeComp_Data data = null)
        {
            if(ritual != null && data !=null)
            {
                Pawn pawn = ritual.assignments.AssignedPawns("PB_Bonder").First();
                var behavior = ritual.Ritual.behavior as RitualBehaviorWorker_BondRitual;
                float quality = behavior.storedQuality;
                StringBuilder sb = new StringBuilder("PB_CompQualityDesc".Translate(LabelForDesc,quality.ToStringPercent()));
                sb.AppendInNewLine(behavior.storedQualDesc);
                return sb.ToString();
            }
            return base.GetDesc(ritual, data);
        }
        public override bool Applies(LordJob_Ritual ritual)
        {
            return true;
        }
        public override float QualityOffset(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            var behavior = ritual.Ritual.behavior as RitualBehaviorWorker_BondRitual;
            return behavior.storedQuality;
        }
        public float CountInternal(Pawn pawn, Precept_Ritual ritual, out string qualityDesc, RitualOutcomeComp_BondTraitsData data)
        {
            //Complicated way of deciding bond strength based on pawns traits and stats compared to the weapon trat def
            //Alternative is random
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
                qualityDesc = "Random Chance Enabled";
                return data.GetChance(pawn, behavior.personaWeapon);
            }
            StringBuilder qualityDebug = new StringBuilder();
            CompBladelinkWeapon weapon = behavior.personaWeapon.TryGetComp<CompBladelinkWeapon>();
            List<WeaponTraitDef> wepTraits = weapon.TraitsListForReading;
            foreach(WeaponTraitDef wepTrait in wepTraits) //Probably should separate these out to their own methods at some point
            {
                qualityDebug.AppendInNewLine("Weapon Trait: " + wepTrait.label);
                if (wepTrait.HasModExtension<PersonaRitualComp_Extension>())
                {
                    float locQuality;
                    var extension = wepTrait.GetModExtension<PersonaRitualComp_Extension>();
                    float curveCount = 0f;
                    if (!extension.traitDefStage.NullOrEmpty())//Traits check
                    {
                        int traitCount = 0;
                        max += MaxValueOfCurve(extension.traitCurve);
                        foreach (KeyValuePair<TraitDef,int> kvp in extension.traitDefStage)
                        {                            
                            if (pawn.story.traits.HasTrait(kvp.Key, kvp.Value))
                            {
                                string label = kvp.Key.DataAtDegree(kvp.Value).GetLabelFor(pawn);
                                traitCount++;
                                qualityDebug.AppendInNewLine(" Trait " + label);
                            }
  
                        }
                        curveCount = traitCount;
                        locQuality = extension.traitCurve.Evaluate(curveCount);
                        qualityDebug.AppendInNewLine(" Traits Quality: " + locQuality.ToStringPercent() + " Max Quality " + MaxValueOfCurve(extension.traitCurve).ToStringPercent());
                        count += locQuality;
                    }
                    if (extension.stat != null) //Stat check
                    {
                        max += MaxValueOfCurve(extension.statCurve);
                        curveCount = pawn.GetStatValue(extension.stat);
                        qualityDebug.AppendInNewLine(" " + extension.stat.label + " Value: " + curveCount);
                        locQuality = extension.statCurve.Evaluate(curveCount);
                        qualityDebug.AppendInNewLine(" Stat Quality: " + locQuality.ToStringPercent() + " Max Quality " + MaxValueOfCurve(extension.statCurve).ToStringPercent());
                        count += locQuality;
                    }                    
                }
            }
            //Bonus for Pyro and FireSwords
            if(behavior.personaWeapon.def.tools.Any(x=>x.extraMeleeDamages?.Any(y=>y.def == DamageDefOf.Flame)?? false)) 
            {
                if (pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac))
                {
                    count += 0.2f;
                    max += 0.2f;
                    qualityDebug.AppendInNewLine("Pyromaniac with plasmasword " + 0.2f.ToStringPercent());
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


    }
    
}
