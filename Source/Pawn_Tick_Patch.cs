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

            WeaponWithLight weaponWithLight = null;
            foreach (var equipment in equipmentList)
            {
                if (equipment is WeaponWithLight)
                {
                    weaponWithLight = (WeaponWithLight) equipment;
                    break;
                }
            }

            if (weaponWithLight == null)
            {
                return;
            }

            Log.Message("tick");
            // ((ThingWithComps) stubWithLight).Tick();
            if (!weaponWithLight.LightIsOn && Find.TickManager.TicksGame < weaponWithLight.NextUpdateTick)
                return;
            weaponWithLight.NextUpdateTick = Find.TickManager.TicksGame + weaponWithLight.UpdatePeriodInTicks;


            if (weaponWithLight.NeedSynchronization)
            {
                weaponWithLight.SynchronizeLightMode();
                weaponWithLight.NeedSynchronization = false;
            }

            weaponWithLight.RefreshLightState();
        }
    }
}