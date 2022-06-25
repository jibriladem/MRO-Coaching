using System;
using System.Collections.Generic;
using System.Text;

namespace MROCoatching.DataObjects.RequestModel.ServiceRequest
{
    public class AttachmentRequests
    {
        public long RequestId { get; set; }
        public List<AttachmentRequest> AttachmentList { get; set; }
        public AttachmentRequests()
        {
            AttachmentList = new List<AttachmentRequest>();
        }
    }
    public class AttachmentRequest
    {
        public string AttachmentPath { get; set; }
        public long AttachmentTypeId { get; set; }
    }

    public class AttachmentReq
    {
        public long RequestId { get; set; }
    }


    public class PersonAttachmentReq
    {
        public long PersonId { get; set; }
    }

    public class PersonAttachmentRequest
    {
        public string AttachmentPath { get; set; }
        public string AttachmentType { get; set; }
    }

    public class PersonAttachmentRequests
    {
        public long PersonId { get; set; }
        public List<PersonAttachmentRequest>  personAttachmentRequests { get; set; }
        public PersonAttachmentRequests()
        {
            personAttachmentRequests = new List<PersonAttachmentRequest>();
        }
    }

}
