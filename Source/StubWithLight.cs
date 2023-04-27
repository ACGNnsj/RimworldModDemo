namespace ModOne
{
    public class StubWithLight : WeaponWithLight
    {
        public StubWithLight()
        {
            GizmoInfo = new GizmoInfo("Ui/Commands/CommandButton_LightModeAutomatic",
                "Ui/Commands/CommandButton_LightModeForcedOn", "Ui/Commands/CommandButton_LightModeForcedOff",
                "Light: automatic", "Light: on", "Light: off", "Click to switch mode.", "Click");
            LightDef = ModOne.Light.LightDef;
        }
    }
}