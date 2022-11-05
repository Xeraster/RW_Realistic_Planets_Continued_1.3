using System;
using RimWorld;
using Verse;
namespace TradeShipsNoMatterWhat
{
    [DefOf]
    public static class TSNMWSettingDefOf
    {
        public static TSNMWSettingDef defaultSettings;

        static TSNMWSettingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(TSNMWSettingDefOf));
        }
    }
}