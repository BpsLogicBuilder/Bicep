namespace OpenTelemetryGeneratorConsole.Structures
{
    internal class ExceptionScenario(string service, string path, string exceptionType, string exceptionMessage)
    {
        public string Service { get; set; } = service;
        public string Path { get; set; } = path;
        public string ExceptionType { get; set; } = exceptionType;
        public string ExceptionMessage { get; set; } = exceptionMessage;
    }
}
