using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using gbrueckl.PowerBI.API.PowerBIObjects;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Microsoft.PowerBI.Api.V1.Models;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    public class PBIGroup : IPBIObject
    {
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "isReadOnly", Required = Required.Always)]
        public bool IsReadOnly { get; set; }

        #endregion

        #region Public Properties
        public List<PBIDataset> Datasets
        {
            get
            {
                PBIObjectList<PBIDataset> objList = JsonConvert.DeserializeObject<PBIObjectList<PBIDataset>>(ParentPowerBIAPI.SendGETRequest(ApiURL, PBIAPI.DataSets).ResponseToString());

                foreach (var item in objList.Items)
                {
                    item.ParentPowerBIAPI = this.ParentPowerBIAPI;
                    item.ParentGroup = this;
                    item.ParentObject = this;
                }

                return objList.Items;
            }
        }     

        public List<PBIDashboard> Dashboards
        {
            get
            {
                PBIObjectList<PBIDashboard> objList = JsonConvert.DeserializeObject<PBIObjectList<PBIDashboard>>(ParentPowerBIAPI.SendGETRequest(ApiURL, PBIAPI.Dashboards).ResponseToString());

                foreach (var item in objList.Items)
                {
                    item.ParentPowerBIAPI = this.ParentPowerBIAPI;
                    item.ParentGroup = this;
                    item.ParentObject = this;
                }

                return objList.Items;
            }
        }

        public List<PBIReport> Reports
        {
            get
            {
                PBIObjectList<PBIReport> objList = JsonConvert.DeserializeObject<PBIObjectList<PBIReport>>(ParentPowerBIAPI.SendGETRequest(ApiURL, PBIAPI.Reports).ResponseToString());

                foreach (var item in objList.Items)
                {
                    item.ParentPowerBIAPI = this.ParentPowerBIAPI;
                    item.ParentGroup = this;
                    item.ParentObject = this;
                }

                return objList.Items;
            }
        }


        [JsonIgnore]
        public PBIAPIClient ParentPowerBIAPI { get; set; }

        [JsonIgnore]
        public PBIGroup ParentGroup { get; set; }
        [JsonIgnore]
        public string ApiURL
        {
            get
            {
                return string.Format("/v1.0/myorg/groups/{0}", Id);
            }
        }

        [JsonIgnore]
        public IPBIObject ParentObject { get; set; }
        #endregion

        #region Public Methods
        public PBIDataset GetDatasetByID(string id)
        {
            try
            {
                return Datasets.Single(x => string.Equals(x.Id, id, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception e)
            {
                //return null;
                throw new KeyNotFoundException(string.Format("No Dataset with ID '{0}' could be found in PowerBI!", id), e);
            }
        }

        public PBIDataset GetDatasetByName(string name)
        {
            try
            {
                return Datasets.Single(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase)); 
            }
            catch (Exception e)
            {
                //return null;
                throw new KeyNotFoundException(string.Format("No Dataset with name '{0}' could be found in PowerBI!", name), e);
            }
        }

        public PBIReport GetReportByName(string name)
        {
            try
            {
                return Reports.Single(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception e)
            {
                //return null;
                throw new KeyNotFoundException(string.Format("No Report with name '{0}' could be found in PowerBI!", name), e);
            }
        }

        public PBIDashboard GetDashboardByName(string name)
        {
            try
            {
                return Dashboards.Single(x => string.Equals(x.DisplayName, name, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception e)
            {
                //return null;
                throw new KeyNotFoundException(string.Format("No Dashboard with name '{0}' could be found in PowerBI!", name), e);
            }
        }
        #endregion

    }

    
}
