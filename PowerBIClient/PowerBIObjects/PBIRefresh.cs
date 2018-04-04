using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    public class PBIRefresh
    {
        #region Public Properties for Serialization
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "refreshType", NullValueHandling = NullValueHandling.Ignore)]
        public string RefreshType { get; set; }

        [JsonProperty(PropertyName = "startTime", NullValueHandling = NullValueHandling.Ignore)]
        public string StartTime { get; set; }

        [JsonProperty(PropertyName = "endTime", NullValueHandling = NullValueHandling.Ignore)]
        public string EndTime { get; set; }

        [JsonProperty(PropertyName = "status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "serviceExceptionJson", NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceExceptionJson { get; set; }
        #endregion
    }
}
