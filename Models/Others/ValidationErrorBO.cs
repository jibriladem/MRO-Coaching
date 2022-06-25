using System.Collections.Generic;

namespace MROCoatching.DataObjects.Models.Others
{
    public class ValidationErrorBO
    {
        public string ProcessStatus { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Exceptions { get; set; }
    }
}