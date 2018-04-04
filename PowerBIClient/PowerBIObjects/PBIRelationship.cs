using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    public class PBIRelationship
    {
        private PBIDataset _parentDataset;
        #region Constructors
        [JsonConstructor]
        public PBIRelationship(string name, PBIColumn fromColumn, PBIColumn toColumn, PBICrossFilteringBehavior crossFiltering = PBICrossFilteringBehavior.Automatic)
        {
            Name = name;
            _fromTable = fromColumn.ParentTable.Name;
            _fromColumn = fromColumn.Name;

            _toTable = toColumn.ParentTable.Name;
            _toColumn = toColumn.Name;

            CrossFilteringBehavior = crossFiltering;
        }
        #endregion
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "fromTable", Required = Required.Always)]
        private string _fromTable;

        [JsonProperty(PropertyName = "fromColumn", Required = Required.Always)]
        private string _fromColumn;

        [JsonProperty(PropertyName = "toTable", Required = Required.Always)]
        private string _toTable;

        [JsonProperty(PropertyName = "toColumn", Required = Required.Always)]
        private string _toColumn;

        [JsonProperty(PropertyName = "crossFilteringBehavior", Required = Required.Always)]
        private string _crossFilteringBehavior;

        #endregion

        #region Public Properties
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonIgnore]
        public PBIColumn FromColumn
        {
            set
            {
                _fromTable = value.ParentTable.Name;
                _fromColumn = value.Name;
            }
        }

        [JsonIgnore]
        public PBIColumn ToColumn
        {
            set
            {
                _toTable = value.ParentTable.Name;
                _toColumn = value.Name;
            }
        }

        [JsonIgnore]
        public PBICrossFilteringBehavior CrossFilteringBehavior
        {
            get
            {
                if (string.IsNullOrEmpty(_crossFilteringBehavior))
                    return PBICrossFilteringBehavior.Automatic;

                return (PBICrossFilteringBehavior)Enum.Parse(typeof(PBICrossFilteringBehavior), _crossFilteringBehavior, true);
            }
            set
            {
                _crossFilteringBehavior = value.ToString();
            }
        }

        [JsonIgnore]
        public PBIDataset ParentDataset
        {
            get { return _parentDataset; }
            set
            {
                _parentDataset = value;

                if(!_parentDataset.Relationships.Contains(this))
                    _parentDataset.Relationships.Add(this);
            }
        }
        #endregion
    }
}
