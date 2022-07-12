using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;


namespace PersonaBond
{
    public class RitualOutcomeComp_RandomForWeapon : IExposable
    {
        public RitualOutcomeComp_RandomForWeapon(Thing weapon)
        {
            this.weapon = weapon;
        }
        public Thing weapon;
        public Dictionary<Pawn, float> chances = new Dictionary<Pawn,float>();
        private List<Pawn > pawnsTemp = new List<Pawn>();
        private List<float> intTemp = new List<float>();
        public virtual void ExposeData()
        {
            Scribe_References.Look(ref weapon, "weapon");
            Scribe_Collections.Look(ref chances, "chances", LookMode.Reference, LookMode.Value, ref pawnsTemp, ref intTemp);
        }
    }
}
