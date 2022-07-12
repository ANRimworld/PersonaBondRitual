using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace PersonaBond
{
    public class RitualOutcomeComp_BondTraitsData : RitualOutcomeComp_Data
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref randomForWeapon,"randomForWeapon",Array.Empty<object>());
        }
        public List<RitualOutcomeComp_RandomForWeapon> randomForWeapon = new List<RitualOutcomeComp_RandomForWeapon>();

        public float GetChance(Pawn pawn, Thing weapon)
        {
            var weaponChance = randomForWeapon.FirstOrDefault(x=>x.weapon == weapon);
            float chance;
            if(weaponChance != null)
            {
                if(!weaponChance.chances.TryGetValue(pawn,out chance))
                {
                    chance = Rand.Range(0f, 0.75f);
                    weaponChance.chances.Add(pawn, chance);
                    return chance;
                }
                return chance;
            }
            else
            {
                weaponChance = new RitualOutcomeComp_RandomForWeapon(weapon);
                chance = Rand.Range(0f, 0.75f);
                randomForWeapon.Add(weaponChance);
                weaponChance.chances.Add(pawn, chance);
                return chance;
            }
        }
    }
}
