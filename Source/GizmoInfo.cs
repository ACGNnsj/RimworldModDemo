namespace ModOne
{
    public class GizmoInfo
    {
        public string AutoPath;
        public string OnPath;
        public string OffPath;
        public string AutoLabel;
        public string OnLabel;
        public string OffLabel { get; }
        public string Desc;
        public string SoundDefName;

        public GizmoInfo(string autoPath, string onPath, string offPath, string autoLabel, string onLabel,
            string offLabel, string desc, string soundDefName)
        {
            AutoPath = autoPath;
            OnPath = onPath;
            OffPath = offPath;
            AutoLabel = autoLabel;
            OnLabel = onLabel;
            OffLabel = offLabel;
            Desc = desc;
            SoundDefName = soundDefName;
        }
    }
}