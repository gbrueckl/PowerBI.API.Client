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
    public class PBIGatewayDatasource : IPBIObject
    {
        #region Constructors
        #endregion
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "gatewayId", Required = Required.Always)]
        public string GatewayID { get; set; }

        [JsonProperty(PropertyName = "datasourceType", Required = Required.Always)]
        public string DatasourceType { get; set; }

        [JsonProperty(PropertyName = "connectionDetails", Required = Required.Always)]
        public string ConnectionDetails { get; set; }
        #endregion

        #region Public Properties
        [JsonIgnore]
        public PBIGroup ParentGroup { get; set; }
        [JsonIgnore]
        public string ApiURL
        {
            get
            {
                return string.Format("/v1.0/myorg/gateways/{0}/datasources/{1}", GatewayID, Id);
            }
        }
        [JsonIgnore]
        public IPBIObject ParentObject { get; set; }

        public PBIDataset ParentDataset
        {
            get { return (PBIDataset)ParentObject; }
            set
            {
                ParentObject = value;
                //if (!ParentDataset.Tables.Contains(this))
                //   ParentDataset.Tables.Add(this);
            }
        }
        #endregion

        #region Public Methods
        public void UpdateDatatsourceInPowerBI(string newCredentials, PBIAPIClient powerBiAPI = null)
        {
            if (ParentDataset == null)
                throw new Exception("Cannot update a Gateway Datasource in PowerBI if the GatewayDatasource object is not linked to a DataSet in PowerBI!");

            if (powerBiAPI == null)
            {
                if (ParentDataset.ParentPowerBIAPI == null)
                    throw new Exception("No PowerBI API Object was supplied!");
                else
                    powerBiAPI = ParentDataset.ParentPowerBIAPI;
            }

            using (HttpWebResponse response = powerBiAPI.SendGenericWebRequest(ApiURL, "PATCH", newCredentials))
            {
                string result = response.ResponseToString();
            }
        }
        #endregion 
    }
}
