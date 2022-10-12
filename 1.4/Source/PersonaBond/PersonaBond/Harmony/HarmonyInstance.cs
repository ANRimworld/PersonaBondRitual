using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;





namespace PersonaBond
{
    //Setting the Harmony instance
    [StaticConstructorOnStartup]
    public class Main
    {
        static Main()
        {
            var harmony = new Harmony("com.PersonaBond");
            harmony.PatchAll(Assembly.GetExecutingAssembly());


        }


        
    }

}
