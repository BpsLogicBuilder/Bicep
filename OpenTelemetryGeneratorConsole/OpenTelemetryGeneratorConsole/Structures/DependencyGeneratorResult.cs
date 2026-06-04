namespace OpenTelemetryGeneratorConsole.Structures
{
    internal class DependencyGeneratorResult
    {
        public string TargetHost { get; set; } = "";
        public string DepType{ get; set; } = "";
        public bool IsSlow { get; set; }
    }
}
