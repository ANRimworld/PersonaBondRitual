using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;

namespace PersonaBond
{
    
    public class RitualOutcomeEffectWorker_Persona : RitualOutcomeEffectWorker_FromQuality
    {
        public RitualOutcomeEffectWorker_Persona() 
        { 
        }
        public RitualOutcomeEffectWorker_Persona(RitualOutcomeEffectDef def) : base(def)
        {
        }


        //Extends the functionality of apply extra outcome
        protected override void ApplyExtraOutcome(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
        {

            extraOutcomeDesc = null;
            Pawn pawn = jobRitual.assignments.FirstAssignedPawn("PB_Bonder");
            var behavior = jobRitual.Ritual.behavior as RitualBehaviorWorker_BondRitual;
            Thing weapon = behavior.personaWeapon;
            CompBladelinkWeapon bladeComp = weapon.TryGetComp<CompBladelinkWeapon>();
            //I wanted to make it go up or down in quality based on this. But turns out that's basically impossible since i'd want to reset it after unbonded and i'd have to create my own comp for that work which is just compatability nightmare.
            //Instead I'm creating my own trait that has stat offsets as close to quality as possible
            if (OutcomeChanceWorst(outcome))
            {
                if (!PersonaBond_Settings.disableWeaponTraits)
                {
                    bladeComp.TraitsListForReading.Add(InternalDefOf.PB_BadBond);
                }
                extraOutcomeDesc = "PB_RitualWorst".Translate(jobRitual.Ritual.Label.Named("RITUAL"), pawn.Name.Named("PAWN"));
            }
            if (outcome.BestPositiveOutcome(jobRitual))
            {
                if (!PersonaBond_Settings.disableWeaponTraits)
                {
                    bladeComp.TraitsListForReading.Add(InternalDefOf.PB_AmazingBond);
                }
                extraOutcomeDesc = "PB_RitualBest".Translate(jobRitual.Ritual.Label.Named("RITUAL"), pawn.Name.Named("PAWN"));
            }      


        }
        public override void Apply(float progress, Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual)
        {
            //For testing my outcome changes leaving for when I think rng is broken again :P   
/*            float quality = GetQuality(jobRitual, progress);                     
            for (int i = 0; i < 500; i++)
            {
                OutcomeChance outcome = GetOutcome(quality, jobRitual);
                Log.Message(outcome.positivityIndex.ToString());
            }*/
            base.Apply(progress, totalPresence, jobRitual);
        }
        public override OutcomeChance GetOutcome(float quality, LordJob_Ritual ritual)
        {
            return (from o in this.def.outcomeChances
                    where this.OutcomePossible(o, ritual)
                    select o).RandomElementByWeight(delegate (OutcomeChance c)
                    {
                        return GetIndividualWeight(c, quality); //Moving this out for resuablility 
                    });
        }
        public virtual float GetIndividualWeight(OutcomeChance chance, float quality)
        {
            if (!chance.Positive)
            {
                return (chance.chance * (1f - quality)); //Changing how outcome is calculated on this ritual even chances at .5 quality
            }
            return Mathf.Max(chance.chance * quality, 0f);
        }
        //Bond will work different to other ritual qualities. Due to how the comp works and difficulty in getting high quality
        protected override bool OutcomePossible(OutcomeChance chance, LordJob_Ritual ritual)
        {
            float quality = GetQuality(ritual, 1f);
            if(quality >= 1f && !chance.BestPositiveOutcome(ritual))//100% is always true bond
            {
                return false;
            }
            return base.OutcomePossible(chance, ritual);
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        //For overriding the dialog % chance outcome
        //Writing this out for reference and wiki aftercleanup
        //Base game is doing this "  - " + outcomeChance.label + ": " + f.ToStringPercent();
        //float f = outcomeChance.Positive ? outcomeChance.chance * num1 / num5 : outcomeChance.chance / num5;
        //Num 1 = quality
        //Num 5 = the total weight calculated below which is essentially sum of bad outcomes and good outcomes * quality
        //num5 += outcomeChance.Positive ? outcomeChance.chance * num1 : outcomeChance.chance;
        public virtual string DialogOutcomeLabelOverride(OutcomeChance outcomeChance, float quality)
        {
            float chance;
            float weight = GetChanceWeight(quality);
            if (!ExpectedOutcomePossible(outcomeChance, quality))
            {
                chance = 0f;
            }
            else
            {
                chance = GetIndividualWeight(outcomeChance,quality)/weight;
            }
            return "  - " + outcomeChance.label + ": " + chance.ToStringPercent(); //Absolutely no effort to change that one number

        }
        public virtual float GetChanceWeight(float quality)
        {
            float weight = 0f;
            foreach(var outcome in def.outcomeChances)
            {
                if (ExpectedOutcomePossible(outcome, quality))//Vanilla adds this to weight regardless of its possible. So if you ever felt like the % chance it showed was lying to you... Well it probably wasn't with exception of duel
                {
                    weight += (outcome.Positive ? (outcome.chance * quality) : outcome.chance * (1 - quality));//Change compared to vanilla is reducing negative outcome chance
                }               
            }
            return weight;
        }

        public virtual bool ExpectedOutcomePossible(OutcomeChance chance, float quality)
        {
            //Its same as Outcome Possible just detaching the ritual so I can do it before thats started
            if (quality >= 1f && !OutcomeChanceBest(chance))
            {
                return false;
            }
            return true;
        }
        //Redone both of these because OutcomeChance method needs lordjob which is silly
        public bool OutcomeChanceWorst(OutcomeChance outcome)
        {
            using (List<OutcomeChance>.Enumerator enumerator = def.outcomeChances.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.positivityIndex < outcome.positivityIndex)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool OutcomeChanceBest(OutcomeChance outcome) 
        {
            using (List<OutcomeChance>.Enumerator enumerator = def.outcomeChances.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.positivityIndex > outcome.positivityIndex)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        //Outcome worker doesnt have this and it was annoying me needing for loops always
        public T GetComp<T>() where T : RitualOutcomeComp
        {
            if (def.comps != null)
            {
                int index = 0;
                int count = def.comps.Count;
                while (index < count)
                {
                    T comp = def.comps[index] as T;
                    if (comp != null)
                    {
                        return comp;
                    }
                    index++;
                }
            }
            return default(T);
        }


        
        
    }
}
