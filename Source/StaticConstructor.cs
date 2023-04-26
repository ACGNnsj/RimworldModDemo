using System.Reflection;
using HarmonyLib;
using Verse;

namespace ModOne
{
    [StaticConstructorOnStartup]
    public static class StaticConstructor
    {
       static StaticConstructor()
        {
            Log.Message("StaticConstructor");
            Harmony harmony = new Harmony("modone.staticconstructor");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}