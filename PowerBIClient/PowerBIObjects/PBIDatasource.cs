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
    public class PBIDatasource : IPBIObject
    {
        #region Constructors
        #endregion
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "@odata.context", NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        private string ODataContext;

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "connectionString", Required = Required.Always)]
        public string ConnectionString { get; set; }
        #endregion

        #region Public Properties
        
        [JsonIgnore]
        public PBIAPIClient ParentPowerBIAPI { get; set; }

        [JsonIgnore]
        public PBIDataset ParentDataset { get; set; }

        [JsonIgnore]
        public string Id { get => null; set { } }
        [JsonIgnore]
        public PBIGroup ParentGroup { get; set; }
        [JsonIgnore]
        public string ApiURL
        {
            get
            {
                if (ParentGroup == null)
                    return string.Format("/v1.0/myorg/datasources/{0}", Name);
                else
                    return string.Format("/v1.0/myorg/groups/0}/dashboards/datasources/{1}", ParentGroup.Id, Name);
            }
        }
        [JsonIgnore]
        public IPBIObject ParentObject { get; set; }
        #endregion
    }
}
