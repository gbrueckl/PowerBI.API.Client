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
    public class PBIMeasure
    {
        #region Constructors
        public PBIMeasure(string name, string expressionFormat, params object[] args)
        {
            Name = name;
            Expression = string.Format(expressionFormat, args);
        }

        public PBIMeasure (string name, string expression)
        {
            Name = name;
            Expression = expression;
        }
        public PBIMeasure(string name) : this(name, null) { }
        #endregion
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "expression", Required = Required.Always)]
        public string Expression { get; set; }

        [JsonProperty(PropertyName = "formatString", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string FormatString { get; set; }

        [JsonProperty(PropertyName = "isHidden", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsHidden { get; set; }
        #endregion

        #region Public Properties
        [JsonIgnore]
        public PBITable ParentTable { get; set; }
        #endregion


    }
}
