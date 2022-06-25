using System.Collections.Generic;

namespace MROCoatching.DataObjects.Models.Operational
{
    public class ErrorResponse
    {
        public string ProcessStatus { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Exceptions { get; set; }
    }
}
