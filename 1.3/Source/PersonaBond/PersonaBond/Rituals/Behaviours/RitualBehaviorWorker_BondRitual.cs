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
    public class RitualBehaviorWorker_BondRitual : RitualBehaviorWorker
    {
        public RitualBehaviorWorker_BondRitual()
        {
        }
        public RitualBehaviorWorker_BondRitual(RitualBehaviorDef def) : base(def)
        {
        }

        public override void TryExecuteOn(TargetInfo target, Pawn organizer, Precept_Ritual ritual, RitualObligation obligation, RitualRoleAssignments assignments, bool playerForced = false)
        {
            Pawn pawn = assignments.FirstAssignedPawn("PB_Bonder");
            if (!EquipmentUtility.CanEquip(personaWeapon, pawn))
            {
                Messages.Message("PB_UnableToEquip", MessageTypeDefOf.RejectInput);
                return;
            }
            CompGraphicCustomization comp = personaWeapon.TryGetComp<CompGraphicCustomization>();
            if (comp != null)
            {
                Action onClose = () =>
                {                   
                    if (pawn.jobs.curJob.def != GraphicCustomization_DefOf.VEF_CustomizeItem)//How I can tell it was confirmed
                    {                       
                        return;
                    }
                    TryExecuteInternal(target, organizer, ritual, obligation, assignments, playerForced);

                    RitualOutcomeComp_BondTraits outcomeComp = ritual.outcomeEffect.GetComp<RitualOutcomeComp_BondTraits>();//Yes I made an extension method for this
                    storedQuality = outcomeComp.CountInternal(pawn, ritual, out string qualityDesc, ritual.outcomeEffect.DataForComp(outcomeComp) as RitualOutcomeComp_BondTraitsData);
                    storedQualDesc = qualityDesc;
                };
                
                var window = new Dialog_GraphicCustomization_BondRitual(comp, onClose,pawn);
                Find.WindowStack.Add(window);

            }

           
        }
        private void TryExecuteInternal(TargetInfo target, Pawn organizer, Precept_Ritual ritual, RitualObligation obligation, RitualRoleAssignments assignments, bool playerForced = false)
        {
            base.TryExecuteOn(target, organizer, ritual, obligation, assignments, playerForced);
        }
        public override string CanStartRitualNow(TargetInfo target, Precept_Ritual ritual, Pawn selectedPawn = null, Dictionary<string, Pawn> forcedForRole = null)
        {
            //Cant put it here because I need it to not show gizmo unless avaliable.
/*            if (Find.TickManager.TicksGame < checkTime)//This gets called constantly so not checking constantly
            {
                return lastResult;
            }
                checkTime = Find.TickManager.TicksGame + 200;
                bool flag = false;
                foreach (ThingDef def in AllPersonaWeapons)
                {
                    if (target.Map.listerThings.ThingsOfDef(def).Any())
                    {
                        flag = true;
                        break;
                    }
                }

            if (!flag)
            {
                return lastResult = null;
            }*/
            return  base.CanStartRitualNow(target, ritual, selectedPawn, forcedForRole);

        }
        public int checkTime;
        public string lastResult;
        public override bool CanExecuteOn(TargetInfo target, RitualObligation obligation)
        {
            return base.CanExecuteOn(target, obligation);
           
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref personaWeapon, "personaWeapon");
        }
        public override void Cleanup(LordJob_Ritual ritual)
        {
            base.Cleanup(ritual);
            personaWeapon = null;
            storedQuality = 0f;
        }
        public IEnumerable<ThingDef> AllPersonaWeapons
        {
            get
            {
                foreach (var def in DefDatabase<ThingDef>.AllDefs)
                {
                    if (def.GetCompProperties<CompProperties_GraphicCustomization>() != null
                        && def.GetCompProperties<CompProperties_BladelinkWeapon>() != null)
                    {
                        yield return def;
                    }
                }
            }
        }
        public Thing personaWeapon;
        public float storedQuality; //I have to store it because the pawn bonds mid ritual, so if the trait affects stats it would change quality from start to finish
        public string storedQualDesc;
        
    }
}
