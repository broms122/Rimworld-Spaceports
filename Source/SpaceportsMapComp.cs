﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Spaceports
{
    public class SpaceportsMapComp : MapComponent
    {
		private IncidentQueue incidentQueueVisitors = new IncidentQueue();
        private IncidentQueue incidentQueueTraders = new IncidentQueue();
		private IncidentDef visitorIncident = SpaceportsDefOf.Spaceports_VisitorShuttleArrival;
        private IncidentDef traderIncident = SpaceportsDefOf.Spaceports_TraderShuttleArrival;
        private float visitorInterval = 0f;
        private float traderInterval = 0f;
        private int nextQueueInspection;

		public SpaceportsMapComp(Map map) : base(map) { 
            
        }

        public override void ExposeData()
        {
			Scribe_Deep.Look(ref incidentQueueVisitors, "incidentQueueVisitors");
            Scribe_Deep.Look(ref incidentQueueTraders, "incidentQueueTraders");
            Scribe_Values.Look(ref nextQueueInspection, "nextQueueInspection", 0);
            Scribe_Values.Look(ref visitorInterval, "visitorInterval", 0f);
            Scribe_Values.Look(ref traderInterval, "traderInterval", 0f);
			base.ExposeData();
        }

        public override void MapComponentTick()
        {
			base.MapComponentTick();

            if(incidentQueueVisitors == null)
            {
                incidentQueueVisitors = new IncidentQueue();
            }

            if(incidentQueueTraders == null)
            {
                incidentQueueTraders = new IncidentQueue();
            }

            CheckIfQueueDisabled();
            CheckIfVisitorQueueTimeChanged();
            CheckIfTraderQueueTimeChanged();

            incidentQueueVisitors.IncidentQueueTick();
            incidentQueueTraders.IncidentQueueTick();

            if (GenTicks.TicksGame > nextQueueInspection)
            {
                nextQueueInspection = (int)(GenTicks.TicksGame + GenDate.TicksPerDay * 0.1f);
                if (incidentQueueVisitors.Count <= 1 && LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().regularVisitors) 
                {
                    IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(visitorIncident.category, Find.Maps.Where((Map x) => x.IsPlayerHome).RandomElement());
                    QueueIncident(new FiringIncident(visitorIncident, null, incidentParms), LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().visitorFrequencyDays, incidentQueueVisitors);
                }
                if(incidentQueueTraders.Count <= 1 && LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().regularTraders)
                {
                    IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(traderIncident.category, Find.Maps.Where((Map x) => x.IsPlayerHome).RandomElement());
                    QueueIncident(new FiringIncident(traderIncident, null, incidentParms), LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().traderFrequencyDays, incidentQueueTraders);
                }
            }
        }

        public void QueueIncident(FiringIncident incident, float afterDays, IncidentQueue queue)
        {
            var qi = new QueuedIncident(incident, (int)(Find.TickManager.TicksGame + GenDate.TicksPerDay * afterDays));
            queue.Add(qi);
        }

        public void CheckIfQueueDisabled() {
            if (incidentQueueVisitors.Count > 0 && !LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().regularVisitors)
            {
                incidentQueueVisitors.Clear();
            }
            if (incidentQueueTraders.Count > 0 && !LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().regularTraders)
            {
                incidentQueueTraders.Clear();
            }
        }

        public void CheckIfVisitorQueueTimeChanged() {
            if (visitorInterval == 0f)
            {
                visitorInterval = LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().visitorFrequencyDays;
                return;
            }
            else if (visitorInterval == LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().visitorFrequencyDays) 
            {
                return;
            }
            else if(visitorInterval != LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().visitorFrequencyDays)
            {
                incidentQueueVisitors.Clear();
                visitorInterval = LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().visitorFrequencyDays;
                return;
            }
        }

        public void CheckIfTraderQueueTimeChanged()
        {
            if (traderInterval == 0f)
            {
                traderInterval = LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().traderFrequencyDays;
                return;
            }
            else if (traderInterval == LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().traderFrequencyDays)
            {
                return;
            }
            else if (traderInterval != LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().traderFrequencyDays)
            {
                incidentQueueTraders.Clear();
                traderInterval = LoadedModManager.GetMod<SpaceportsMod>().GetSettings<SpaceportsSettings>().traderFrequencyDays;
                return;
            }
        }

    }
}
