﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Spaceports.Incidents
{
    public class IncidentWorker_MysteryCargo : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            if (!Utils.CheckIfSpaceport((Map)parms.target))
            {
                return false;
            }
            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Letters.MysteryCargoLetter letter = (Letters.MysteryCargoLetter)LetterMaker.MakeLetter(def.letterLabel, "Spaceports_MysteryCargoLetter".Translate(), def.letterDef);
            letter.title = "Spaceports_MysteryCargo".Translate();
            letter.radioMode = true;
            letter.map = map;
            letter.StartTimeout(5000);
            Find.LetterStack.ReceiveLetter(letter);
            return true;
        }
    }
}