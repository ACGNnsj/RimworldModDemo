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
                foreach (var gizmo in gizmos)
                {
                    yield return gizmo;
                }

                yield break;
            }

            switch (stubWithLight.Mode)
            {
                case StubWithLight.LightMode.Automatic:
                    ((Command) commandAction).icon =
                        (Texture) ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LightModeAutomatic", true);
                    ((Command) commandAction).defaultLabel = "Light: automatic";
                    break;
                case StubWithLight.LightMode.ForcedOn:
                    ((Command) commandAction).icon =
                        (Texture) ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LightModeForcedOn", true);
                    ((Command) commandAction).defaultLabel = "Light: on";
                    break;
                case StubWithLight.LightMode.ForcedOff:
                    ((Command) commandAction).icon =
                        (Texture) ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LightModeForcedOff", true);
                    ((Command) commandAction).defaultLabel = "Light: off";
                    break;
            }

            commandAction.defaultDesc = "Click to switch mode.";
            commandAction.activateSound = SoundDef.Named("Click");
            commandAction.action = new Action(stubWithLight.SwitchLightMode);
            commandAction.groupKey = num + 1;
            foreach (var gizmo in gizmos)
            {
                if (gizmo != commandAction)
                {
                    yield return gizmo;
                }
            }

            yield return (Gizmo) commandAction;
        }
    }
}