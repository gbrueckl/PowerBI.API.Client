using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    [DataContract]
    public class PBIReport : IPBIObject
    {
        #region Constructors
        #endregion
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "@odata.context", NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        private string ODataContext;

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "webUrl", Required = Required.Default)]
        public string WebURL { get; set; }

        [JsonProperty(PropertyName = "embedUrl", Required = Required.Always)]
        public string EmbedURL { get; set; }
        #endregion

        #region Public Properties
        [JsonIgnore]
        public PBIAPIClient ParentPowerBIAPI { get; set; }

        [JsonIgnore]
        public PBIGroup ParentGroup { get; set; }
        [JsonIgnore]
        public string ApiURL
        {
            get
            {
                if (ParentGroup == null)
                    return "v1.0/myorg/reports";
                else
                    return string.Format("v1.0/myorg/groups/0}/reports", ParentGroup.Id);
            }
        }
        [JsonIgnore]
        public IPBIObject ParentObject { get; set; }
        #endregion


    }
}
