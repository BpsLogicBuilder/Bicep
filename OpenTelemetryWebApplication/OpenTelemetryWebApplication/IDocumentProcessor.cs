using OpenTelemetryWebApplication.Structures;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenTelemetryWebApplication
{
    public interface IDocumentProcessor
    {
        Task<List<ProcessingResults>> ProcessorDocuments(int documentCount);
    }
}
