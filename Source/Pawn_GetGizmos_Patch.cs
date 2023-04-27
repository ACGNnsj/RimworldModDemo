using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace ModOne
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
    class Pawn_GetGizmos_Patch
    {
        static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> gizmos, Pawn __instance)
        {
            int num = 700000101;
            Command_Action commandAction = new Command_Action();
            var equipmentList = __instance.equipment.AllEquipmentListForReading;
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
                foreach (var gizmo in gizmos)
                {
                    yield return gizmo;
                }

                yield break;
            }

            switch (weaponWithLight.Mode)
            {
                case LightMode.Automatic:
                    commandAction.icon = ContentFinder<Texture2D>.Get(weaponWithLight.GizmoInfo.AutoPath);
                    commandAction.defaultLabel = weaponWithLight.GizmoInfo.AutoLabel;
                    break;
                case LightMode.ForcedOn:
                    commandAction.icon = ContentFinder<Texture2D>.Get(weaponWithLight.GizmoInfo.OnPath);
                    commandAction.defaultLabel = weaponWithLight.GizmoInfo.OnLabel;
                    break;
                case LightMode.ForcedOff:
                    commandAction.icon = ContentFinder<Texture2D>.Get(weaponWithLight.GizmoInfo.OffPath);
                    commandAction.defaultLabel = weaponWithLight.GizmoInfo.OffLabel;
                    break;
            }

            commandAction.defaultDesc = weaponWithLight.GizmoInfo.Desc;
            commandAction.activateSound = SoundDef.Named(weaponWithLight.GizmoInfo.SoundDefName);
            commandAction.action = weaponWithLight.SwitchLightMode;
            commandAction.groupKey = num + 1;
            foreach (var gizmo in gizmos)
            {
                if (gizmo != commandAction)
                {
                    yield return gizmo;
                }
            }

            yield return commandAction;
        }
    }
}