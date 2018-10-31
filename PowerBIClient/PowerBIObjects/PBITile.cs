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
using Microsoft.PowerBI.Api.V2.Models;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    [DataContract]
    public class PBITile : Tile, IPBIObject
    {
        #region Constructors
        #endregion
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "@odata.context", NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        private string ODataContext;
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
                    return string.Format("/v1.0/myorg/dashboards/{0}/tiles/{1}", ParentObject.Id, Id);
                else
                    return string.Format("/v1.0/myorg/groups/{0}/dashboards/{1}/tiles/{2}", ParentGroup.Id, ParentObject.Id, Id);
            }
        }

        [JsonIgnore]
        public IPBIObject ParentObject { get; set; }
        #endregion
    }
}
