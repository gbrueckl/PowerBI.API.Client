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
    public class PBIDatasetParameter: DatasetParameter, IPBIObject
    {
        #region Constructors
        public PBIDatasetParameter(string name, string newValue)
        {
            this.Name = name;
            this.CurrentValue = newValue;
        }
        #endregion
        #region Private Properties for Serialization
        [JsonIgnore]
        public string Id {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [JsonProperty(PropertyName = "newValue", NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        private string Value
        {
            get
            {
                return CurrentValue;
            }
        }

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
                    return string.Format("/v1.0/myorg/imports/{0}", Id);
                else
                    return string.Format("/v1.0/myorg/groups/{0}/imports/{1}", ParentGroup.Id, Id);
            }
        }
        [JsonIgnore]
        public IPBIObject ParentObject { get; set; }

        #endregion

        #region Public Functions
        #endregion

        #region ShouldSerialize-Functions
        public bool ShouldSerialize_isRequired()
        {
            return false;
        }

        public bool ShouldSerialize_typed()
        {
            return false;
        }
        public bool ShouldSerialize_currentValue()
        {
            return false;
        }
        #endregion
    }
}
