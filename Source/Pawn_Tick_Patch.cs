using HarmonyLib;
using RimWorld;
using Verse;

namespace ModOne
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Tick))]
    class Pawn_Tick_Patch
    {
        static void Postfix(Pawn __instance)
        {
            var equipmentList = __instance.equipment?.AllEquipmentListForReading;

            if (equipmentList == null)
            {
                return;
            }

            StubWithLight stubWithLight = null;
            foreach (var equipment in equipmentList)
            {
                if (equipment is StubWithLight)
                {
                    stubWithLight = (StubWithLight) equipment;
                    break;
                }
            }

            if (stubWithLight == null)
            {
                return;
            }

            Log.Message("tick");
            // ((ThingWithComps) stubWithLight).Tick();
            if (!stubWithLight.LightIsOn && Find.TickManager.TicksGame < stubWithLight.NextUpdateTick)
                return;
            stubWithLight.NextUpdateTick = Find.TickManager.TicksGame + stubWithLight.UpdatePeriodInTicks;


            if (stubWithLight.NeedSynchronization)
            {
                stubWithLight.SynchronizeLightMode();
                stubWithLight.NeedSynchronization = false;
            }

            stubWithLight.RefreshLightState();
        }
    }
}