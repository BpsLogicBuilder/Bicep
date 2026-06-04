namespace OpenTelemetryGeneratorConsole.Structures
{
    internal class RequestGeneratorResult
    {
        public string Service { get; set; } = "";
        public string Endpoint { get; set; } = "";
        public string Status { get; set; } = "";
        public bool Failed { get; set; }
    }
}
