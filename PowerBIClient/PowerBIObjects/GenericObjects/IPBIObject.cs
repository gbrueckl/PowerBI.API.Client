using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    public interface IPBIObject
    {
        [JsonIgnore]
        string ApiURL { get; }
        [JsonIgnore]
        PBIGroup ParentGroup { get; }

        [JsonIgnore]
        IPBIObject ParentObject { get; }

        string Id { get; set; }
    }
}
