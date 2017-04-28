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
    public class PBITile : IPBIObject
    {
        #region Constructors
        #endregion
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "@odata.context", NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        private string ODataContext;

        [DataMember(Name = "id", IsRequired = true)]
        private string _id;

        [DataMember(Name = "title", IsRequired = true)]
        private string _title;

        [DataMember(Name = "subTitle", IsRequired = false)]
        private string _subTitle;

        [DataMember(Name = "embedUrl", IsRequired = true)]
        private string _embedUrl;

        [DataMember(Name = "embedData", IsRequired = false)]
        private string _embedData;

        [DataMember(Name = "rowSpan", IsRequired = false)]
        private int _rowSpan;

        [DataMember(Name = "colSpan", IsRequired = false)]
        private int _colSpan;
        #endregion

        #region Public Properties
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string SubTitle
        {
            get { return _subTitle; }
            set { _subTitle = value; }
        }

        public string EmbedURL
        {
            get { return _embedUrl; }
            set { _embedUrl = value; }
        }

        public string EmbedData
        {
            get 
            {
                if (_embedData == null)
                    LoadDetailsFromPowerBI();
                return _embedData; 
            }
            set { _embedData = value; }
        }  

        public int RowSpan
        {
            get { return _rowSpan; }
            set { _rowSpan = value; }
        }
        
        public int ColSpan
        {
            get { return _colSpan; }
            set { _colSpan = value; }
        }
        [JsonIgnore]
        public PBIGroup ParentGroup { get; set; }
        [JsonIgnore]
        public string ApiURL
        {
            get
            {
                if (ParentGroup == null)
                    return string.Format("/v1.0/myorg/dashboards{0}/tiles/{1}", ParentObject.Id, Id);
                else
                    return string.Format("v1.0/myorg/groups/0}/dashboards/{1}/tiles/{2}", ParentGroup.Id, ParentObject.Id, Id);
            }
        }
        [JsonIgnore]
        public IPBIObject ParentObject { get; set; }

        #endregion

        public void LoadDetailsFromPowerBI()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PBITile));

            PBITile pbitile = (PBITile)serializer.ReadObject(ParentGroup.ParentPowerBIAPI.SendGETRequest(ApiURL).GetResponseStream());

            _embedData = pbitile.EmbedData;
        }
    }
}
