using Microsoft.PowerBI.Api.V2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;


namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    public class PBIRow : Row 
    {
        #region Constructors
        public PBIRow(Dictionary<string, object> values)
        {
            Values = values;
        }
        #endregion
        #region Public Properties
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public new string Id { get { return null; } set { } }

        [JsonExtensionData]
        private Dictionary<string, object> Values { get; set; }
        [JsonIgnore]
        public PBITable ParentTable { get; set; }

        [JsonIgnore]
        public string JSON { get { return JsonConvert.SerializeObject(this); } }
        #endregion

        #region Public Functions
        public void PublishToPowerBI(PBIAPIClient powerBiAPI = null)
        {
            if (ParentTable == null)
                throw new Exception("Cannot add row to PowerBI table as the object is not linked to a Table in PowerBI!");
           
            ParentTable.PushRowToPowerBI(this, powerBiAPI);
        }

        public void SetValue(string key, object value)
        {
            if (!Values.ContainsKey(key))
                throw new KeyNotFoundException(string.Format("The key '{0}' was not found in the Row!"));

            Values[key] = value;
        }

        public void SetValues(Dictionary<string, object> newValues)
        {
            if(newValues.Count != Values.Count)
                throw new IndexOutOfRangeException(string.Format("The number of new Values does not match the number of Columns in the Row!"));
            
            foreach(KeyValuePair<string, object> kvp in newValues)
            {
                Values[kvp.Key] = kvp.Value;
            }
        }

        public void AddValue(string key, object value)
        {
            if (!Values.ContainsKey(key))
                throw new KeyNotFoundException(string.Format("The key '{0}' was not found in the Row!"));

            Values.Add(key, value);
        }
        #endregion
    }
}
