using System;
using System.Collections.Generic;
using Verse;
using System.Linq;
using RimWorld;

namespace PersonaBond
{
    public class RitualObligationTargetWorker_Persona : RitualObligationTargetWorker_AnyRitualSpotOrAltar
    {
        public RitualObligationTargetWorker_Persona()
        {
        }
        public RitualObligationTargetWorker_Persona(RitualObligationTargetFilterDef def) : base(def)
        {
        }
        protected override RitualTargetUseReport CanUseTargetInternal(TargetInfo target, RitualObligation obligation)
        {
                      
            if (!base.CanUseTargetInternal(target, obligation).canUse)
            {
                if (!(target.Thing is Building_Throne)) //Adding Throne to possible spots
                {
                    return base.CanUseTargetInternal(target, obligation);
                }
                        
            }
            //I have no choice to do it here because I need it to not show gizmo.
            if (Find.TickManager.TicksGame < checkTime)
            {
                return flag;
            }
            flag = false;
            checkTime = Find.TickManager.TicksGame + 600; //Long time between checks
            var behavior = parent.behavior as RitualBehaviorWorker_BondRitual;
            flag = false;
            foreach(ThingDef def in behavior.AllPersonaWeapons)
            {
                if (target.Map.listerThings.ThingsOfDef(def).Any())
                {
                    flag = true;
                    break;
                }
            }
            
            return flag;
		}


        public bool flag = false;
        public int checkTime;
    }
}
