using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
using RimWorld.Planet;
using System;


namespace PersonaBond
{

    [HarmonyPatch(typeof(Dialog_BeginRitual))]
    [HarmonyPatch("DrawQualityFactors")]
    //I welcome you to the most effort for the smallest thing i've done yet. Which may be hard to top.
    //Patch to allow an override of how the quality is displayed because you can change it in the effect worker but it doesnt display that to user at all \o/
    //string str2 = "  - " + outcomeChance.label + ": " + f.ToStringPercent(); is what i'm looking to replace
    //In the loop where its building widget labels for each outcome

    public static class Dialog_BeginRitual_DrawQualityFactors_Patch
    {
        static MethodInfo Concat = AccessTools.Method(typeof(string), nameof(string.Concat), new Type[] { typeof(string), typeof(string), typeof(string), typeof(string) });
        static MethodInfo predictedQuality = AccessTools.Method(typeof(Dialog_BeginRitual), "PredictedQuality");
        static FieldInfo outcomeField = AccessTools.Field(typeof(Dialog_BeginRitual), "outcomeChances");
        static FieldInfo ritualField = AccessTools.Field(typeof(Dialog_BeginRitual), "ritual");
        static FieldInfo outcomeLabel = AccessTools.Field(typeof(OutcomeChance), "label");
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            List<CodeInstruction> codes = instructions.ToList();
            object locString= null;
            object locOutcome = null;
            object locQuality = null;
            var startIndex = -1;
            var endIndex = -1;
            bool found = false;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].Calls(predictedQuality)) //Grab the local quality
                {
                    locQuality = codes[i - 1].operand;
                }
                if (codes[i].LoadsField(outcomeField) && codes[i+1].opcode == OpCodes.Callvirt)//Load field for this gets called a lot. This filters out of a few but not all
                {
                    //Start search to find the loop we want
                    startIndex = i+1;
                    for(int j = startIndex; j <codes.Count; j++)
                    {
                        if (codes[j].LoadsField(outcomeField))//stop searching not it
                        {
                            startIndex = -1;
                            break;
                        }
                        
                        if (codes[j].LoadsField(outcomeLabel))
                        {
                            locOutcome = codes[j-1].operand;//This wont always be the right one, but its the outcomeChance of current loop. Will be set to right one before found though
                        }
                        if(codes[j].opcode == OpCodes.Call && codes[j].operand == (object)Concat && codes[j-1].opcode == OpCodes.Call) //99% sure this and is uncessary but being careful
                        {
                            found = true;
                            endIndex = j+1;//This will be the stLoc
                            locString = codes[endIndex].operand;                            
                            break;
                        }
                    }
                }                
                if (found) { break; }
            }
            if (found)
            {

                List<CodeInstruction> newInstruction= new List<CodeInstruction>();
                //Loading the stored local variables and fields to callstack.
                newInstruction.Add(new CodeInstruction(OpCodes.Ldloc, locString));
                newInstruction.Add(new CodeInstruction(OpCodes.Ldloc, locOutcome));
                newInstruction.Add(new CodeInstruction(OpCodes.Ldarg_0));//Guess how long this line took me T_T
                newInstruction.Add(new CodeInstruction(OpCodes.Ldfld, ritualField));
                newInstruction.Add(new CodeInstruction(OpCodes.Ldloc_3));
                MethodInfo myMethod = AccessTools.Method(typeof(Dialog_BeginRitual_DrawQualityFactors_Patch), "GetOutcomeChanceOverride");
                newInstruction.Add(new CodeInstruction(OpCodes.Call, myMethod));//Call my method
                //Now set local string to my return
                newInstruction.Add(new CodeInstruction(OpCodes.Stloc, locString));

                codes.InsertRange(endIndex+1, newInstruction);//Now to see if it worked
            }
            else
            {
                Log.Error("Persona_Ritual_Dialog_BeginRitual_DrawQualityFactors_Patch could not locate transpiler hook custom ritual chances will not be displayed");
            }


            return codes.AsEnumerable();


        }

        //string concat, OutcomeChance outcome,  Precept_Ritual ritual, float Quality
        public static string GetOutcomeChanceOverride(string concat, OutcomeChance outcome,Precept_Ritual ritual, float quality)
        {
            if(ritual == null)
            {
                return concat;
            }
            if (ritual.outcomeEffect is RitualOutcomeEffectWorker_Persona worker)
            {
                return worker.DialogOutcomeLabelOverride(outcome, quality);
            }

            return concat;


        }

    }

}
