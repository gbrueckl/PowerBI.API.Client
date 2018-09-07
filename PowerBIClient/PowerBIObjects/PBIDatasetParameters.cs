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
    public class PBIDatasetParameters
    {
        #region Constructors
        public PBIDatasetParameters()
        {
            Items = new List<PBIDatasetParameter>();
        }

        public PBIDatasetParameters(params string[] keyValues)
        {
            if(keyValues.Length % 2 != 0)
            {
                throw new NotSupportedException("An even number of parameters has to be passed to the constructor of PBIDatasetParameters. The parameters are Key-Value pairs!");
            }

            Items = new List<PBIDatasetParameter>();

            for (int i = 0; i < keyValues.Length; i+=2)
            {
                Items.Add(new PBIDatasetParameter(keyValues[i], keyValues[i + 1]));
            }
            
        }
        #endregion
        [JsonProperty(PropertyName = "updateDetails", NullValueHandling = NullValueHandling.Ignore)]
        public List<PBIDatasetParameter> Items;
    }
}
