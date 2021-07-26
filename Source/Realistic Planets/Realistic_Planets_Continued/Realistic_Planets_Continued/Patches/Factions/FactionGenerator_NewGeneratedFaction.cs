using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Linq;
using Verse;

namespace Planets_Code.Factions
{
	[HarmonyPatch(typeof(FactionGenerator), "NewGeneratedFaction", new Type[] { typeof(FactionGeneratorParms) })]
	public static class FactionGenerator_NewGeneratedFaction {
		[HarmonyPriority(Priority.High)]
        //old overload = FactionDef facDef, ref Faction __result
		//public static bool Prefix(FactionDef facDef, ref Faction __result) {
        public static bool Prefix(FactionGeneratorParms parms, ref Faction __result) {
			if (Controller.Settings.usingFactionControl.Equals(true)) {
				return true;
			}
            //restore the old facDef variable
            FactionDef facDef = parms.factionDef;
            parms.ideoGenerationParms.forFaction = facDef;
            Faction faction = new Faction();
			faction.def = facDef;
			faction.loadID = Find.UniqueIDsManager.GetNextFactionID();
			faction.colorFromSpectrum = FactionGenerator.NewRandomColorFromSpectrum(faction);
            faction.hidden = parms.hidden;

            if (facDef.humanlikeFaction)
            {
                faction.ideos = new FactionIdeosTracker(faction);
                if (!faction.IsPlayer || !ModsConfig.IdeologyActive || !Find.GameInitData.startedFromEntry)
                {
                    faction.ideos.ChooseOrGenerateIdeo(parms.ideoGenerationParms);
                }
            }

            if (!facDef.isPlayer)
            {
                if (facDef.fixedName != null)
                {
                    faction.Name = facDef.fixedName;
                }
                else
                {
                    string text = "";
                    for (int i = 0; i < 10; i++)
                    {
                        string text2 = NameGenerator.GenerateName(faction.def.factionNameMaker, Find.FactionManager.AllFactionsVisible.Select((Faction fac) => fac.Name));
                        if (text2.Length <= 20)
                        {
                            text = text2;
                        }
                    }
                    if (text.NullOrEmpty())
                    {
                        text = NameGenerator.GenerateName(faction.def.factionNameMaker, Find.FactionManager.AllFactionsVisible.Select((Faction fac) => fac.Name));
                    }
                    faction.Name = text;
                }
            }

            faction.centralMelanin = Rand.Value;
			foreach (Faction allFactionsListForReading in Find.FactionManager.AllFactionsListForReading) {
				faction.TryMakeInitialRelationsWith(allFactionsListForReading);
			}
            faction.TryGenerateNewLeader();
			if (!facDef.hidden && !facDef.isPlayer) {
				Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
				settlement.SetFaction(faction);
				settlement.Tile = TileFinder.RandomSettlementTileFor(faction, false, null);
				settlement.Name = SettlementNameGenerator.GenerateSettlementName(settlement, null);
				Find.WorldObjects.Add(settlement);
			}
			__result = faction;
			return false;
		}
	}
	
}
