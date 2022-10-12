using System;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace PersonaBond
{
	[DefOf]
	public static class InternalDefOf
	{
		static InternalDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
		}


		public static JobDef PB_Bonding;
		public static JobDef PB_TakeWeaponToCell;
		public static JobDef PB_BondingCustomize;
		public static WeaponTraitDef PB_BadBond;
		public static WeaponTraitDef PB_AmazingBond;


	}
}
