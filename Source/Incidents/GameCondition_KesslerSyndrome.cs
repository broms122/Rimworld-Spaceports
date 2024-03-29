﻿using RimWorld;
using Verse;

namespace Spaceports.Incidents
{
    public class GameCondition_KesslerSyndrome : GameCondition
    {
        public override void PostMake()
        {
            base.PostMake();
            if (!LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().eventsEnabled || !LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().KesslerSyndrome)
            {
                End();
            }
        }
    }
}
