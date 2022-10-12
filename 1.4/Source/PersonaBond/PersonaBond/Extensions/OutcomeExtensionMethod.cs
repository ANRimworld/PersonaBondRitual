using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace PersonaBond
{
    public static class OutcomeExtensionMethod
    {
        public static T GetComp<T>(this RitualOutcomeEffectWorker worker) where T : RitualOutcomeComp
        {
            if (worker.def.comps != null)
            {
                int index = 0;
                int count = worker.def.comps.Count;
                while (index < count)
                {
                    T comp = worker.def.comps[index] as T;
                    if (comp != null)
                    {
                        return comp;
                    }
                    index++;
                }
            }
            return default(T);
        }
    }
}
