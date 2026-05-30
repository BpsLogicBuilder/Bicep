using System.Collections.Generic;

namespace OpenTelemetryWebApplication.Structures
{
    public class ProcessingResults
    {
        public string DocId { get; set; } = "";
        public Dictionary<string, object> Validate { get; set; } = [];
        public Dictionary<string, object> Enrich { get; set; } = [];
        public Dictionary<string, object> Store{ get; set; } = [];
    }
}
