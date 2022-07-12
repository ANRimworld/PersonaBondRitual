
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.AI;
using Verse.Sound;
using Verse.AI.Group;
namespace PersonaBond
{

	public class JobDriver_Bonding: JobDriver
	{
		protected Thing Takee
		{
			get
			{
				return (Thing)this.job.GetTarget(TargetIndex.A).Thing;
			}
		}
		public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
			Map.reservationManager.ReleaseAllForTarget(Takee);
			return Map.reservationManager.Reserve(pawn, job, Takee, 1, -1, null, true);
			
        }
		protected override IEnumerable<Toil> MakeNewToils()
		{
			//Copied tick action from JobDriver_GiveSpeech
			this.FailOnDestroyedOrNull(TargetIndex.A);			
			Toil toilGoto = Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
			yield return toilGoto;
			yield return new Toil//Copied from JobDriver_Equip
			{
				initAction = delegate ()
				{
					ThingWithComps thingWithComps = (ThingWithComps)this.job.targetA.Thing;
					ThingWithComps thingWithComps2;
					if (thingWithComps.def.stackLimit > 1 && thingWithComps.stackCount > 1)
					{
						thingWithComps2 = (ThingWithComps)thingWithComps.SplitOff(1);
					}
					else
					{
						thingWithComps2 = thingWithComps;
						thingWithComps2.DeSpawn(DestroyMode.Vanish);
					}
					this.pawn.equipment.MakeRoomFor(thingWithComps2);
					this.pawn.equipment.AddEquipment(thingWithComps2);
					if (thingWithComps.def.soundInteract != null)
					{
						thingWithComps.def.soundInteract.PlayOneShot(new TargetInfo(this.pawn.Position, this.pawn.Map, false));
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			
			Toil toil = Toils_General.Wait(3650, TargetIndex.None);
			Lord lord = this.pawn.GetLord();
			LordJob_Ritual lordJob_Ritual = lord.LordJob as LordJob_Ritual;
			toil.tickAction = delegate ()
			{
				this.pawn.rotationTracker.FaceTarget(lordJob_Ritual.CurrentSpectatorCrowdCenter());//use ritual target
				if (Find.TickManager.TicksGame % 250 == 0)
				{
					Vector3 vector = this.pawn.TrueCenter();
					vector += (job.targetB.Thing.TrueCenter() - vector) * Rand.Value;
					FleckMaker.Static(vector, this.pawn.Map, FleckDefOf.PsycastAreaEffect, 0.5f);
				}
				if (Find.TickManager.TicksGame % 150 == 0)
				{
/*					Vector3 vector = this.pawn.TrueCenter();
					MoteMaker.MakeStaticMote(vector, this.pawn.Map, DefDatabase<ThingDef>.GetNamed("Mote_PsychicEmanatorEffect"), 1f);*/
				}

			};			
			yield return toil;
			yield break;
		}

		private const TargetIndex TakeeIndex = TargetIndex.A;
		private IntVec3 facingCellCached = IntVec3.Invalid;

	}
}
