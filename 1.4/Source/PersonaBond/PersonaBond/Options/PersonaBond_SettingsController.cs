using RimWorld;
using UnityEngine;
using Verse;


namespace PersonaBond
{



    public class PersonaBond_Mod : Mod
    {


        public PersonaBond_Mod(ModContentPack content) : base(content)
        {
            GetSettings<PersonaBond_Settings>();
        }
        public override string SettingsCategory()
        {

            return "Persona Bond Ritual";


        }



        public override void DoSettingsWindowContents(Rect inRect)
        {
            PersonaBond_Settings.DoWindowContents(inRect);
        }
    }


}
