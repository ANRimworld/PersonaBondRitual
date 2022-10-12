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
	public class JobGiver_Bonding : ThinkNode_JobGiver
	{


		protected override Job TryGiveJob(Pawn pawn)
		{
			Lord lord = pawn.GetLord();
			LordJob_Ritual ritual = ((lord != null) ? lord.LordJob : null) as LordJob_Ritual;
			var behavior = ritual.Ritual.behavior as RitualBehaviorWorker_BondRitual;
			Thing weapon = behavior.personaWeapon;
			
			


			Job job = JobMaker.MakeJob(InternalDefOf.PB_Bonding, weapon, pawn.mindState.duty.focusThird);

			return job;
		}


	}	
}
