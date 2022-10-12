using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;
namespace PersonaBond
{
	public class JobGiver_TakeWeaponToCell : JobGiver_GotoTravelDestination
	{
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_TakeWeaponToCell JobGiver_TakeWeaponToCell = (JobGiver_TakeWeaponToCell)base.DeepCopy(resolve);	
			return JobGiver_TakeWeaponToCell;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			Lord lord = pawn.GetLord();
			LordJob_Ritual ritual = ((lord != null) ? lord.LordJob : null) as LordJob_Ritual;
			var behavior = ritual.Ritual.behavior as RitualBehaviorWorker_BondRitual;
			Thing weapon = behavior.personaWeapon;
			if (!pawn.CanReach(weapon, PathEndMode.Touch, PawnUtility.ResolveMaxDanger(pawn, this.maxDanger)))
			{
				return null;
			}
			if (ritual.PawnTagSet(pawn, "Arrived"))
			{
				return null;
			}
			IntVec3 cell = pawn.mindState.duty.focusThird.Thing.InteractionCell;
			cell += new IntVec3(0, 0, -1);
			if (!pawn.CanReach(cell, PathEndMode.Touch, PawnUtility.ResolveMaxDanger(pawn, this.maxDanger)))
			{
				cell = pawn.mindState.duty.focusThird.Thing.InteractionCell;
			}
			


			Job job = JobMaker.MakeJob(InternalDefOf.PB_TakeWeaponToCell, weapon, cell, pawn.mindState.duty.focusThird);
			job.locomotionUrgency = PawnUtility.ResolveLocomotion(pawn, this.locomotionUrgency);
			job.expiryInterval = this.jobMaxDuration;
			job.count = 1;
			job.ritualTag = "Arrived";
			return job;
		}


	}	
}
