using Microsoft.PowerBI.Api.V2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    public class PBIGroupMember
    {
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "emailAddress", NullValueHandling = NullValueHandling.Ignore)]
        private string _emailAddress;

        [JsonProperty(PropertyName = "groupUserAccessRight", NullValueHandling = NullValueHandling.Ignore)]
        private string _groupUserAccessRight;
        #endregion

        #region Public Properties
        [JsonIgnore]
        public string Name
        {
            get
            {
                return _emailAddress;
            }

            set
            {
                _emailAddress = value;
            }
        }

        [JsonIgnore]
        public PBIGroupAccessRight AccessRight
        {
            get
            {
                return (PBIGroupAccessRight)Enum.Parse(typeof(PBIGroupAccessRight), _groupUserAccessRight, true);
            }
            set
            {
                _groupUserAccessRight = value.ToString();
            }
        }
        #endregion
    }
}
