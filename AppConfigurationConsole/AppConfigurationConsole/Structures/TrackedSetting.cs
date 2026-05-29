namespace AppConfigurationConsole.Structures
{
    internal class TrackedSetting
    {
        internal string Key { get; set; } = "";
        internal string Before { get; set; } = "";
        internal string After { get; set; } = "";
        internal bool Changed { get; set; }
    }
}
