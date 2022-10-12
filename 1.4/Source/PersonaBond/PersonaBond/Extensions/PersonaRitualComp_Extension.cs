using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicCustomization;
using Verse;
using RimWorld;
using UnityEngine;

namespace PersonaBond
{
    public class PersonaRitualComp_Extension : DefModExtension
    {
        public StatDef stat = null;
        public SimpleCurve statCurve = null;
        public Dictionary<TraitDef,int> traitDefStage= new Dictionary<TraitDef, int>();
        public SimpleCurve traitCurve = null;
        public WeaponTraitDef weaponTrait;

    }
    
}
