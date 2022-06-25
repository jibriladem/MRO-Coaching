using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MROCoatching.DataObjects.Models
{


	[Serializable]
	public class OperationStatusResponse
	{
		public string Message { get; set; }
		public string BusinessErrorCode { get; set; }
		public OperationStatus Status { get; set; }
		public OperationStatus StatusAPI { get; set; }
		public OperationStatus MessageAPI { get; set; }
		public List<string> MessageList { get; set; }
		public List<ValidationError> Errors { get; set; }

	}

	public class ValidationError
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Field { get; set; }

		public string Message { get; set; }

		public ValidationError(string field, string message)
		{
			Field = field != string.Empty ? field : null;
			Message = message;
		}
	}
}
