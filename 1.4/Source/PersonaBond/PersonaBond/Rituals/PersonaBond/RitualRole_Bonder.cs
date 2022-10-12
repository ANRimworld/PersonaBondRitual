using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace PersonaBond
{
    public class RitualRoleBonder : RitualRole
    {
		public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
		{
			reason = null;			
			if (!p.Faction.IsPlayerSafe())
			{
				if (!skipReason)
				{
					
				}
				return false;
			}	
			if (p.equipment.bondedWeapon != null)
			{
				if (!skipReason)
				{
					reason = "PB_AleadyBonded".Translate();
				}
				return false;
			}	
			if (p.WorkTagIsDisabled(WorkTags.Violent))
			{
				if (!skipReason)
				{
					reason = "PB_CapableOfViolence".Translate();
				}
				return false;
			}
			return true;
		}

		
		public override bool AppliesToRole(Precept_Role role, out string reason, Precept_Ritual ritual = null, Pawn p = null, bool skipReason = false)
		{
			reason = null;
			return false;
		}
		
	}
}

