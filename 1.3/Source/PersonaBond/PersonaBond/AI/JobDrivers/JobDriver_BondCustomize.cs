
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using GraphicCustomization;
using Verse.AI;
using Verse.AI.Group;
namespace PersonaBond
{
	[StaticConstructorOnStartup]
	public class JobDriver_BondCustomize : JobDriver
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
			Toil toilGoto = Toils_Goto.GotoCell(TargetIndex.C, PathEndMode.OnCell).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
			yield return toilGoto;
			Toil toil = new Toil();
			toil.tickAction = delegate ()
			{
				this.pawn.GainComfortFromCellIfPossible(false);
				this.pawn.skills.Learn(SkillDefOf.Social, 0.3f, false);
				Lord lord = this.pawn.GetLord();
				LordJob_Ritual lordJob_Ritual = lord.LordJob as LordJob_Ritual;
				if (this.ticksTillSocialInteraction <= 0)
				{
					if (this.job.showSpeechBubbles)
					{
						MoteMaker.MakeSpeechBubble(this.pawn, moteIcon);
					}

					if (this.job.interaction != null)
					{
						
						if ((lordJob_Ritual = (((lord != null) ? lord.LordJob : null) as LordJob_Ritual)) != null)
						{
							InteractionUtility.ImitateSocialInteractionWithManyPawns(this.pawn, lordJob_Ritual.lord.ownedPawns, this.job.interaction);
						}
						
					}
					this.ticksTillSocialInteraction = SocialInteractionInterval;
				}
				IntVec3 face = Takee.Position;
				pawn.rotationTracker.FaceTarget(face);

				this.ticksTillSocialInteraction--;
			};
			if (ModsConfig.IdeologyActive)
			{
				toil.PlaySustainerOrSound(delegate ()
				{
					if (this.pawn.gender != Gender.Female)
					{
						return this.job.speechSoundMale;
					}
					return this.job.speechSoundFemale;
				}, this.pawn.story.VoicePitchFactor);
			}
			toil.defaultCompleteMode = ToilCompleteMode.Delay;
			toil.defaultDuration = 1400;//End just before next ritual stage 
			toil.handlingFacing = true;
			yield return toil;
			yield return Toils_General.Do(() =>
			{
				var comp = TargetA.Thing.TryGetComp<CompGraphicCustomization>();
				comp.Customize();
			});
			yield break;
		}

		private int ticksTillSocialInteraction = 60;
		private static readonly int SocialInteractionInterval = 480;

		public static readonly Texture2D moteIcon = ContentFinder<Texture2D>.Get("Things/Mote/SpeechSymbols/Speech", true);
		private const TargetIndex TargetCellIndex = TargetIndex.B;
	}
}
