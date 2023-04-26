using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ModOne
{
    public class StubWithLight : ThingWithComps
    {
        public readonly int UpdatePeriodInTicks = 30;
        public int NextUpdateTick = 0;
        public bool NeedSynchronization = true;
        public Thing Light;
        public bool LightIsOn = false;
        public LightMode Mode = StubWithLight.LightMode.Automatic;

        /*public Action Action;

        public StubWithLight()
        {
            Action = SwitchLightMode;
        }*/

        public override void ExposeData()
        {
            base.ExposeData();
            Log.Message("exposeData");
            Scribe_References.Look<Thing>(ref this.Light, "Light", false);
            Scribe_Values.Look<bool>(ref this.LightIsOn, "LightIsOn", false, false);
            Scribe_Values.Look<StubWithLight.LightMode>(ref this.Mode, "Mode",
                StubWithLight.LightMode.Automatic, false);
            Scribe_Values.Look<int>(ref this.NextUpdateTick, "NextUpdateTick", 0, false);
            Scribe_Values.Look<bool>(ref this.NeedSynchronization, "NeedSynchronization", false, false);
        }

        /*public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            ((ThingWithComps) this).SpawnSetup(map, respawningAfterLoad);
            Log.Message("spawnSetup");
            this.NeedSynchronization = true;
        }*/

        public override void Tick()
        {
            Log.Message("tick");
            ((ThingWithComps) this).Tick();
            if (!this.LightIsOn && Find.TickManager.TicksGame < this.NextUpdateTick)
                return;
            this.NextUpdateTick = Find.TickManager.TicksGame + 60;
            Pawn Wearer = null;
            Pawn_EquipmentTracker pawnEquipmentTracker;
            bool flag1 = (pawnEquipmentTracker = holdingOwner?.Owner as Pawn_EquipmentTracker) != null;
            if (flag1)
            {
                Wearer = pawnEquipmentTracker.pawn;
            }
            else
            {
                Pawn_ApparelTracker pawnApparelTracker;
                bool flag2 = (pawnApparelTracker = holdingOwner?.Owner as Pawn_ApparelTracker) != null;
                if (flag2)
                {
                    Wearer = pawnApparelTracker.pawn;
                }
            }

            if (this.NeedSynchronization && Wearer != null)
            {
                this.SynchronizeLightMode();
                this.NeedSynchronization = false;
            }

            this.RefreshLightState();
        }

        public void SynchronizeLightMode()
        {
            foreach (var thing in holdingOwner)
            {
                if (thing is StubWithLight)
                {
                    ((StubWithLight) thing).Mode = this.Mode;
                }
            }
        }

        public void RefreshLightState()
        {
            Log.Message("refreshLightState");
            var flag = ComputeLightState();
            Log.Message(flag.ToString());
            if (flag)
                this.SwitchOnLight();
            else
                this.SwitchOffLight();
        }

        public bool ComputeLightState()
        {
            Pawn Wearer = null;
            Pawn_EquipmentTracker pawnEquipmentTracker;
            bool flag1 = (pawnEquipmentTracker = holdingOwner?.Owner as Pawn_EquipmentTracker) != null;
            if (flag1)
            {
                Wearer = pawnEquipmentTracker.pawn;
            }
            else
            {
                Pawn_ApparelTracker pawnApparelTracker;
                bool flag2 = (pawnApparelTracker = holdingOwner?.Owner as Pawn_ApparelTracker) != null;
                if (flag2)
                {
                    Wearer = pawnApparelTracker.pawn;
                }
            }

            Log.Message(Wearer.ToString());
            Log.Message("Dead " + Wearer.Dead.ToString());
            Log.Message("Downed " + Wearer.Downed.ToString());
            Log.Message("Awake " + RestUtility.Awake(Wearer).ToString());
            Log.Message(Wearer.CurJobDef.ToString());
            Log.Message(Mode.ToString());
            return Wearer != null && !Wearer.Dead && !Wearer.Downed && RestUtility.Awake(Wearer) &&
                   Wearer.CurJobDef != JobDefOf.Lovin && (Mode == LightMode.ForcedOn ||
                                                          Mode != LightMode.ForcedOff && Wearer.Map != null &&
                                                          (GridsUtility.Roofed(Wearer.Position, Wearer.Map) &&
                                                           Wearer.Map.glowGrid.PsychGlowAt(Wearer.Position) <=
                                                           PsychGlow.Lit ||
                                                           !GridsUtility.Roofed(Wearer.Position, Wearer.Map) &&
                                                           Wearer.Map.glowGrid.PsychGlowAt(Wearer.Position) <
                                                           PsychGlow.Overlit));
        }


        public void SwitchOnLight()
        {
            Pawn Wearer = null;
            Pawn_EquipmentTracker pawnEquipmentTracker;
            bool flag1 = (pawnEquipmentTracker = holdingOwner?.Owner as Pawn_EquipmentTracker) != null;
            if (flag1)
            {
                Wearer = pawnEquipmentTracker.pawn;
            }
            else
            {
                Pawn_ApparelTracker pawnApparelTracker;
                bool flag2 = (pawnApparelTracker = holdingOwner?.Owner as Pawn_ApparelTracker) != null;
                if (flag2)
                {
                    Wearer = pawnApparelTracker.pawn;
                }
            }

            Log.Message("wearer " + Wearer.Position.ToString());
            IntVec3 intVec3 = IntVec3Utility.ToIntVec3(((Thing) Wearer).DrawPos);
            // if (!ThingUtility.DestroyedOrNull(this.light) && IntVec3.op_Inequality(intVec3, this.light.Position))
            if (!ThingUtility.DestroyedOrNull(this.Light) && intVec3 != this.Light.Position)
                this.SwitchOffLight();
            if (ThingUtility.DestroyedOrNull(this.Light) &&
                GridsUtility.GetFirstThing(intVec3, ((Thing) Wearer).Map, ModOne.Light.LightDef) == null)
                this.Light = GenSpawn.Spawn(ModOne.Light.LightDef, intVec3, ((Thing) Wearer).Map,
                    (WipeMode) 0);
            Log.Message("light " + Light.Position.ToString());
            Log.Message(Light.def.defName);
            Log.Message(Light.def.description);
            this.LightIsOn = true;
        }

        public void SwitchOffLight()
        {
            if (!ThingUtility.DestroyedOrNull(this.Light))
            {
                this.Light.Destroy((DestroyMode) 0);
                this.Light = (Thing) null;
            }

            this.LightIsOn = false;
        }

        /*public override IEnumerable<Gizmo> GetGizmos()
        {
            Console.WriteLine("GetGizmos");
            IList<Gizmo> wornGizmos = new List<Gizmo>();
            int num = 700000101;
            Command_Action commandAction = new Command_Action();
            switch (this.Mode)
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

            ((Command) commandAction).defaultDesc = "Click to switch mode.";
            ((Command) commandAction).activateSound = SoundDef.Named("Click");
            commandAction.action = new Action(this.SwitchLightMode);
            ((Command) commandAction).groupKey = num + 1;
            wornGizmos.Add((Gizmo) commandAction);
            IEnumerable<Gizmo> gizmos = ((ThingWithComps) this).GetGizmos();
            Console.WriteLine(gizmos);
            Console.WriteLine(wornGizmos);
            return gizmos == null ? wornGizmos : gizmos.Concat<Gizmo>(wornGizmos);
        }*/

        public void SwitchLightMode()
        {
            switch (this.Mode)
            {
                case StubWithLight.LightMode.Automatic:
                    this.Mode = StubWithLight.LightMode.ForcedOn;
                    break;
                case StubWithLight.LightMode.ForcedOn:
                    this.Mode = StubWithLight.LightMode.ForcedOff;
                    break;
                case StubWithLight.LightMode.ForcedOff:
                    this.Mode = StubWithLight.LightMode.Automatic;
                    break;
            }

            this.RefreshLightState();
        }

        public enum LightMode
        {
            Automatic,
            ForcedOn,
            ForcedOff,
        }
    }
}