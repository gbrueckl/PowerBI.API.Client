using Microsoft.PowerBI.Api.V1.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;


namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    public class PBISequenceNumber 
    {
        #region Constructors
        #endregion
        #region Public Properties for Serialization
        [JsonProperty(PropertyName = "clientId", Required = Required.Always)]
        public int ClientId { get; set; }

        [JsonProperty(PropertyName = "sequenceNumber", Required = Required.Always)]
        public int SequenceNumber { get; set; }
        #endregion

        #region Public Properties
        public PBITable ParentTable { get; set; }
        #endregion
    }
}
