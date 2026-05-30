using Microsoft.AspNetCore.Mvc;
using OpenTelemetryWebApplication.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTelemetryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetryController(IDocumentProcessor documentProcessor) : ControllerBase
    {
        private readonly IDocumentProcessor documentProcessor = documentProcessor;

        [HttpPost(Name = "ProcessDocuments")]
        public async Task<IActionResult> ProcessDocuments()
        {
            try
            {
                List<ProcessingResults> results = await documentProcessor.ProcessorDocuments(5);
                int slowCount = results.Count(r => (bool)r.Enrich["slow"]);
                return Ok($"Processed {results.Count} document(s). {slowCount} experienced high enrichment latency.");
            }
            catch (Exception ex)
            {
                return Problem($"Error processing documents: {ex}");
            }
        }
    }
}
