using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace YouTingParking.Models
{
    [DataContract]
    public class OpenLockResponse
    {
        [DataMember]
        public bool SuccessSend { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}