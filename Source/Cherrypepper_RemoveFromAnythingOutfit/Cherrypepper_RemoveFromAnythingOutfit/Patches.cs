using RimWorld;
using Verse;
using Harmony;
using System.Reflection;

namespace Randolph_Cherrypepper
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.randolphcherrypepper.rimworld.mod.removefromanythingoutfit");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(OutfitDatabase), "GenerateStartingOutfits")]
    public static class CherrypepperPatch_RimWorld_OutfitDatabase_GenerateStartingOutfits
    {
        [HarmonyPostfix]
        public static void RemoveFromAnythingOutfit(OutfitDatabase __instance)
        {
            if (__instance == null)
            {
                Log.Warning("RemoveFromAnythingOutfit cannot access OutfitDatabase.");
                return;
            }

            string outfitAnythingLabel = "OutfitAnything".Translate();
            if (outfitAnythingLabel == null || outfitAnythingLabel.Length == 0)
            {
                Log.Warning("RemoveFromAnythingOutfit cannot determine a valid label for OutfitAnything.");
                return;
            }

            // Find the Anything outfit. It should be the first, but let's be proper about it and check the identifying label.
            Outfit outfitAnything = null;
            foreach (Outfit outfit in __instance.AllOutfits)
            {
                if (outfit.label == outfitAnythingLabel)
                {
                    outfitAnything = outfit;
                    break;
                }
            }
            if (outfitAnything == null)
            {
                Log.Warning("RemoveFromAnythingOutfit cannot find the Anything outfit in the OutfitDatabase.");
                return;
            }

            // We should have the Anything outfit now. Let sort all apparel with RemoveFromAnything and disallow it.
            int count = 0;
            foreach (ThingDef thingdef in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingdef.apparel != null && thingdef.apparel.defaultOutfitTags != null && thingdef.apparel.defaultOutfitTags.Contains("RemoveFromAnything"))
                {
                    outfitAnything.filter.SetAllow(thingdef, false);
                    count += 1;
                }
            }
            if (count == 0)
                Log.Message("RemoveFromAnythingOutfit found no Apparel with RemoveFromAnything defaultOutfitTag.");
        }
    }
}