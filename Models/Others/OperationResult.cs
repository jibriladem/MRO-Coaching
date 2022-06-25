using System;
using System.Collections.Generic;

namespace MROCoatching.DataObjects.Models.Others
{
    public class OperationResult
    {
        public OperationStatus Status { get; set; }
        public string Message { get; set; }
        public string StatusAPI { get; set; }
        public string MessageAPI { get; set; }
        public Exception ex { get; set; }
        public List<string> ErrorList { get; set; }
        public DateTime Date { get { return DateTime.UtcNow; } }

        public OperationResult()
        {
            ErrorList = new List<string>();
        }
    }
}
