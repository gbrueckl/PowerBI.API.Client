using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    [JsonArray]
    public class PBIRows : PBIObjectList<PBIRow>
    {
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "rows", NullValueHandling = NullValueHandling.Ignore)]
        public List<PBIRow> Rows { get => Items; set => Items = value; }

        [JsonIgnore]
        public PBITable ParentTable { get; set; }

        [JsonIgnore]
        public string JSON { get => JsonConvert.SerializeObject(this); }
        #endregion
    }
}
