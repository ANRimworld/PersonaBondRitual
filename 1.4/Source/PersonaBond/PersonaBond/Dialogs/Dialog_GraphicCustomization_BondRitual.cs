using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GraphicCustomization;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace PersonaBond
{
	public class Dialog_GraphicCustomization_BondRitual : Dialog_GraphicCustomization
	{

        public Dialog_GraphicCustomization_BondRitual(CompGraphicCustomization comp, Action onClose,Pawn pawn = null) : base(comp,pawn)
        {
            Init(comp);
            this.pawn = pawn;
            forcePause = true;
            this.onClose = onClose;
        }
        public override void Close(bool doCloseSound = true)
        {
            onClose();
            base.Close(doCloseSound);
        }
        Action onClose;
    }	
}
