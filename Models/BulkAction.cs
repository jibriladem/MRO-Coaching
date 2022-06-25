using System;
using System.Collections.Generic;
using System.Text;
using MROCoatching.DataObjects.Models.Others;

namespace MROCoatching.DataObjects.Models
{
  public class BulkAction
    {
        public List<long> Ids { get; set; }
        public RecordStatus RecordStatus { get; set; }
    }
}
