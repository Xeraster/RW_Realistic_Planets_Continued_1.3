using System;
using RimWorld;
using Verse;

namespace TradeShipsNoMatterWhat
{

    public class TSNMWSettingDef : Def
    {
        public int minDays = 5;
        public int maxDays = 5;
        public int forceBulkGoodsUranium = 0;

        static TSNMWSettingDef()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(TSNMWSettingDef));
            DefOfHelper.EnsureInitializedInCtor(typeof(TSNMWSettingDefOf));
        }

    }
}