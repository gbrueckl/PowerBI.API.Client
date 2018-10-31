using Microsoft.PowerBI.Api.V2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    public class PBITable : Table, IPBIObject
    {
        #region Constructors
        [JsonConstructor]
        public PBITable(string name)
        {
            Name = name;

            Columns = new List<PBIColumn>();
            Measures = new List<PBIMeasure>();
        }

        public PBITable(DataTable dataTable)
        {
            if (string.IsNullOrEmpty(dataTable.TableName))
                throw new NotSupportedException("A TableName has to be supplied when creating a PBITable object out of a DataTable!");

            Name = dataTable.TableName;

            Measures = new List<PBIMeasure>();

            Columns = dataTable.PBIColumns(); // extension Method

            DataRows = dataTable.PBIRows(); // extension Method
        }
        #endregion
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "columns", NullValueHandling = NullValueHandling.Ignore)]
        private List<PBIColumn> _columns;

        [JsonProperty(PropertyName = "measures", NullValueHandling = NullValueHandling.Ignore)]
        private List<PBIMeasure> _measures;

        [JsonProperty(PropertyName = "isHidden", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsHidden { get; set; }
        #endregion
        #region Public Properties
        [JsonIgnore]
        public new List<PBIColumn> Columns
        {
            get
            {
                foreach (PBIColumn col in _columns)
                {
                    col.ParentTable = this;
                }

                return _columns;
            }
            set { _columns = value; }
        }

        [JsonProperty(PropertyName = "rows", NullValueHandling = NullValueHandling.Ignore)]
        public new List<PBIRow> Rows { get { return null; } set { } }

        [JsonIgnore]
        public new List<PBIMeasure> Measures { get { return _measures; } set { _measures = value; } }

        [JsonIgnore]
        public List<PBIRow> DataRows { get; set; }

        [JsonIgnore]
        public string Id { get { return null; } set { } }

        [JsonIgnore]
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
                    return string.Format("/v1.0/myorg/datasets/{0}/tables/{1}", ParentDataset.Id, Name);
                else
                    return string.Format("/v1.0/myorg/groups/{0}/datasets/{1}/tables/{2}", ParentGroup.Id, ParentDataset.Id, Name);
            }
        }

        [JsonIgnore]
        public IPBIObject ParentObject { get; set; }
        #endregion

        #region Public Functions
        public void AddColumn(PBIColumn column)
        {
            column.ParentTable = this;
            Columns.Add(column);
        }
        public PBIColumn GetColumnByName(string columnName)
        {
            return _columns.Single<PBIColumn>(x => x.Name == columnName);
        }
        public void PublishToPowerBI(PBIAPIClient powerBiAPI = null, bool pushRows = false)
        {
            if (ParentDataset == null)
                throw new Exception("Cannot Publish Table to PowerBI as it is not linked to a DataSet in PowerBI!");

            if (powerBiAPI == null)
            {
                if (ParentDataset.ParentPowerBIAPI == null)
                    throw new Exception("No PowerBI API Object was supplied!");
                else
                    powerBiAPI = ParentDataset.ParentPowerBIAPI;
            }

            using (HttpWebResponse response = powerBiAPI.SendPUTRequest(ApiURL, PBIJsonHelper.SerializeObject(this)))
            {
                string result = response.ResponseToString();
            }

            if(pushRows)
            {
                PushRowsToPowerBI(DataRows, powerBiAPI);
            }
        }

        public void PublishToPowerBI(bool pushRows = false)
        {
            PublishToPowerBI(null, pushRows);
        }

        public PBIRow GetSampleRow()
        {
            Dictionary<string, object> sample = new Dictionary<string, object>(Columns.Count);

            if (Columns != null)
            {
                foreach (PBIColumn col in Columns)
                {
                    sample.Add(col.Name, null);
                }
            }
            PBIRow ret = new PBIRow(sample);
            ret.ParentTable = this;

            return ret;
        }

        private void PushRowsToPowerBI(string JSON, PBIAPIClient powerBiAPI = null)
        {
            if (ParentDataset == null)
                throw new Exception("Cannot push row to PowerBI table as the table is not linked to a DataSet in PowerBI!");

            if (powerBiAPI == null)
            {
                if (ParentDataset.ParentPowerBIAPI == null)
                    throw new Exception("No PowerBI API Object was supplied!");
                else
                    powerBiAPI = ParentDataset.ParentPowerBIAPI;
            }

            if (ParentDataset.PBIDefaultMode != PBIDefaultMode.Push)
                throw new Exception("PushRowsToPowerBI is only supported for Datasets in DefaultMode 'Push'!");

            if (Name == null)
                throw new Exception("Cannot add row to PowerBI table as the table does not have a name!");

            Task.Factory.StartNew(() =>
            {
                using (HttpWebResponse response = powerBiAPI.SendPOSTRequest(ApiURL, PBIAPI.Rows, JSON))
                {
                    string result = response.ResponseToString();
                }
            });
        }

        public void PushRowToPowerBI(PBIRow row, PBIAPIClient powerBiAPI = null)
        {
            PBIRows rows = new PBIRows();
            rows.ParentTable = this;
            rows.Rows = new List<PBIRow>(1);
            rows.Rows.Add(row);

            PushRowsToPowerBI(rows, powerBiAPI);
        }

        public void PushRowsToPowerBI(PBIRows rows, PBIAPIClient powerBiAPI = null)
        {
            if (DataRows == null)
                DataRows = new List<PBIRow>();

            if (rows != null)
                DataRows.AddRange(rows.Rows);

            PushRowsToPowerBI(PBIJsonHelper.SerializeObject(DataRows), powerBiAPI);
        }

        public void PushRowsToPowerBI(List<PBIRow> rows = null, PBIAPIClient powerBiAPI = null)
        {
            if (rows != null)
                DataRows = rows;

            PushRowsToPowerBI(PBIJsonHelper.SerializeObject(DataRows), powerBiAPI);
        }

        private void StreamRowsToPowerBI(string JSON, string apiKey, PBIAPIClient powerBiAPI = null)
        {
            if (ParentDataset == null)
                throw new Exception("Cannot stream row to PowerBI table as the table is not linked to a DataSet in PowerBI!");

            if (powerBiAPI == null)
            {
                if (ParentDataset.ParentPowerBIAPI == null)
                    throw new Exception("No PowerBI API Object was supplied!");
                else
                    powerBiAPI = ParentDataset.ParentPowerBIAPI;
            }

            if (ParentDataset.PBIDefaultMode == PBIDefaultMode.Push)
                throw new Exception("StreamRowsToPowerBI is only supported for Datasets in DefaultMode 'Streaming' or 'PushStreaming'!");

            if (Name == null)
                throw new Exception("Cannot add row to PowerBI table as the table does not have a name!");

            Task.Factory.StartNew(() =>
            {
                //https://api.powerbi.com/beta/<TenantID>/datasets/<DatasetID>/rows?key={MyKey}
                using (HttpWebResponse response = powerBiAPI.SendGenericWebRequest(string.Format("https://api.powerbi.com/beta/{0}/datasets/{1}/rows?key={2}", powerBiAPI.TenantId, ParentDataset.Id, apiKey), "POST", JSON))
                {
                    string result = response.ResponseToString();
                }
            });
        }

        public void StreamRowToPowerBI(PBIRow row, string apiKey, PBIAPIClient powerBiAPI = null)
        {
            if (ParentDataset == null)
                throw new Exception("Cannot add row to PowerBI table as the table is not linked to a DataSet in PowerBI!");

            if (powerBiAPI == null)
            {
                if (ParentDataset.ParentPowerBIAPI == null)
                    throw new Exception("No PowerBI API Object was supplied!");
                else
                    powerBiAPI = ParentDataset.ParentPowerBIAPI;
            }

            PBIRows rows = new PBIRows();
            rows.ParentTable = this;
            rows.Rows = new List<PBIRow>(1);
            rows.Rows.Add(row);

            StreamRowsToPowerBI(rows, apiKey, powerBiAPI);
        }

        public void StreamRowsToPowerBI(PBIRows rows, string apiKey, PBIAPIClient powerBiAPI = null)
        {
            StreamRowsToPowerBI(rows.JSON, apiKey, powerBiAPI);
        }

        public void DeleteRowsFromPowerBI(PBIAPIClient powerBiAPI = null)
        {
            if (ParentDataset == null)
                throw new Exception("Cannot delete rows from PowerBI table as the table is not linked to a DataSet in PowerBI!");

            if (Name == null)
                throw new Exception("Cannot add row to PowerBI table as the table does not have a name!");

            if (powerBiAPI == null)
            {
                if (ParentDataset.ParentPowerBIAPI == null)
                    throw new Exception("No PowerBI API Object was supplied!");
                else
                    powerBiAPI = ParentDataset.ParentPowerBIAPI;
            }

            using (HttpWebResponse response = powerBiAPI.SendDELETERequest(ApiURL, PBIAPI.Rows))
            {
                string result = response.ResponseToString();
            }
        }

        public List<PBISequenceNumber> GetSequenceNumbers(PBIAPIClient powerBiAPI = null)
        {
            if (ParentDataset == null)
                throw new Exception("Cannot delete rows from PowerBI table as the table is not linked to a DataSet in PowerBI!");

            if (Name == null)
                throw new Exception("Cannot add row to PowerBI table as the table does not have a name!");

            if (powerBiAPI == null)
            {
                if (ParentDataset.ParentPowerBIAPI == null)
                    throw new Exception("No PowerBI API Object was supplied!");
                else
                    powerBiAPI = ParentDataset.ParentPowerBIAPI;
            }

            PBIObjectList<PBISequenceNumber> objList = JsonConvert.DeserializeObject<PBIObjectList<PBISequenceNumber>>(powerBiAPI.SendGETRequest(ApiURL + "/sequenceNumbers").ResponseToString());

            foreach (var item in objList.Items)
            {
                item.ParentTable = this;
            }

            return objList.Items;
        }
        #endregion

        #region ShouldSerialize-Functions
        public bool ShouldSerialize_measures()
        {
            if (ParentDataset == null || ParentDataset.PBIDefaultMode == PBIDefaultMode.Streaming)
                return false;

            return true;
        }
        #endregion  
    }
}
