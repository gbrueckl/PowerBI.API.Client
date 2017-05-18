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
    public class PBIDashboard : IPBIObject
    {
        #region Constructors
        #endregion
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "@odata.context", NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        private string ODataContext;

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "displayName", Required = Required.Always)]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "isReadOnly", Required = Required.Default)]
        public bool IsReadOnly;

        [JsonIgnore]
        private List<PBITile> _tiles;
        #endregion

        #region Public Properties

        [JsonIgnore]
        public List<PBITile> Tiles
        {
            get
            {
                if (_tiles == null)
                {
                    if (Id != null)
                    {
                        LoadTilesFromPowerBI();
                    }
                    else
                    {
                        _tiles = new List<PBITile>();
                    }
                }
                return _tiles;
            }
            set
            {
                foreach (PBITile item in value)
                {
                    item.ParentObject = this;
                    item.ParentGroup = this.ParentGroup;
                }
                _tiles = value;
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
                    return string.Format("/v1.0/myorg/dashboards/{0}", Id);
                else
                    return string.Format("v1.0/myorg/groups/{0}/dashboards/{1}", ParentGroup.Id, Id);
            }
        }
        [JsonIgnore]
        public IPBIObject ParentObject { get; set; }

        #endregion

        #region Public Functions
        public PBITile GetTileByName(string name)
        {
            foreach (PBITile tile in Tiles)
            {
                if (tile.Title == name)
                    return tile;
            }

            return null;
        }

        public void LoadTilesFromPowerBI()
        {
            PBIObjectList<PBITile> objList = JsonConvert.DeserializeObject<PBIObjectList<PBITile>>(ParentPowerBIAPI.SendGETRequest(ApiURL, PBIAPI.Tiles).ResponseToString());

            foreach (var item in objList.Items)
            {
                item.ParentGroup = this.ParentGroup;
                item.ParentObject = this;
            }

            Tiles = objList.Items;
        }
        #endregion 
    }
}
