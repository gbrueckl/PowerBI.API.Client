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
    public class PBIReport : Report, IPBIObject
    {
        /*
        {
            "id":"123599a9-0d69-4c2a-a42c-1ef98fd5ab9c",
            "modelId":0,
            "name":"MyReport",
            "webUrl":"https://app.powerbi.com/reports/123599a9-0d69-4c2a-a42c-1ef98fd5ab9c",
            "embedUrl":"https://app.powerbi.com/reportEmbed?reportId=123599a9-0d69-4c2a-a42c-1ef98fd5ab9c",
            "isOwnedByMe":true,
            "isOriginalPbixReport":false,
            "datasetId":"456baf28-3edb-4ec0-af1c-942995dc3e8a"
        }
        */

        #region Constructors
        public PBIReport()
        { }
        public PBIReport(Report report, IPBIObject pbiObject)
        {
            Id = report.Id;
            Name = report.Name;
            EmbedUrl = report.EmbedUrl;
            WebUrl = report.WebUrl;
            DatasetId = report.DatasetId;

            ParentPowerBIAPI = pbiObject.ParentPowerBIAPI;
            ParentObject = pbiObject.ParentObject;
            ParentGroup = pbiObject.ParentGroup;
        }
        #endregion

        #region Private Properties for Serialization

        [JsonProperty(PropertyName = "@odata.context", NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        private string ODataContext;

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public new string Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public new string Name { get; set; }

        [JsonProperty(PropertyName = "webUrl", Required = Required.Default)]
        [JsonIgnoreSerialize]
        public new string WebUrl { get; private set; }
        
        [JsonProperty(PropertyName = "embedUrl", Required = Required.Default)]
        [JsonIgnoreSerialize]
        public new string EmbedUrl { get; private set; }

        [JsonProperty(PropertyName = "isOwnedByMe", Required = Required.Default)]
        [JsonIgnoreSerialize]
        public bool IsOwnedByMe { get; private set; }


        [JsonProperty(PropertyName = "isOriginalPbixReport", Required = Required.Default)]
        [JsonIgnoreSerialize]
        public bool IsOriginalPbixReport { get; private set; }
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
                    return string.Format("/v1.0/myorg/reports/{0}", Id);
                else
                    return string.Format("/v1.0/myorg/groups/{0}/reports/{1}", ParentGroup.Id, Id);
            }
        }
        [JsonIgnore]
        public IPBIObject ParentObject { get; set; }

        public PBIDataset Dataset
        {
            get
            {
                return ((PBIGroup)ParentObject).GetDatasetByID(DatasetId);
            }
            set
            {
                Rebind(value);
            }
        }
        #endregion

        #region Public Functions
        public void Rebind(PBIDataset newDataset)
        {
            if (ParentPowerBIAPI == null)
                throw new Exception("No PowerBI API Object was supplied!");

            using (HttpWebResponse response = ParentPowerBIAPI.SendPOSTRequest(ApiURL + "/Rebind", "{\"datasetId\": \"" + newDataset.Id + "\"}"))
            {
                string result = response.ResponseToString();
            }
        }

        public void Rebind(string newDatasetId)
        {
            Rebind(ParentPowerBIAPI.GetDatasetByID(newDatasetId));
        }

        public void Clone(string newReportName, PBIGroup targetGroup, PBIDataset targetDataset)
        {
            if (ParentPowerBIAPI == null)
                throw new Exception("No PowerBI API Object was supplied!");

            string body;

            body = "{\"name\": \"" + newReportName + "\", \"targetModelId\": \"" + targetDataset.Id + "\"";

            if(targetGroup != null)
            {
                body = body + ", \"targetWorkspaceId\": \"" + targetGroup.Id + "\"";
            }
            body = body + "}";

            using (HttpWebResponse response = ParentPowerBIAPI.SendPOSTRequest(ApiURL + "/Clone", body))
            {
                string result = response.ResponseToString();
            }
        }

        public void Export(string outputFilePath)
        {
            if (ParentPowerBIAPI == null)
                throw new Exception("No PowerBI API Object was supplied!");

            using (HttpWebResponse response = ParentPowerBIAPI.SendGETRequest(ApiURL + "/Export"))
            {
                using (Stream file = File.Create(outputFilePath))
                {
                    response.GetResponseStream().CopyTo(file);
                }

                string result = response.ResponseToString();
            }
        }

        public void Delete()
        {
            if (ParentPowerBIAPI == null)
                throw new Exception("No PowerBI API Object was supplied!");

            using (HttpWebResponse response = ParentPowerBIAPI.SendDELETERequest(ApiURL))
            {
                string result = response.ResponseToString();
            }
        }
        #endregion
    }
}
