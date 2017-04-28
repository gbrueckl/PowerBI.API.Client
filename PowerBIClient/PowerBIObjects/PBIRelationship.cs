using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    [DataContract]
    public class PBIRelationship
    {
        private PBIDataset _parentDataset;
        #region Constructors
        public PBIRelationship(string name, PBIColumn fromColumn, PBIColumn toColumn, PBICrossFilteringBehavior crossFiltering = PBICrossFilteringBehavior.Automatic)
        {
            _name = name;
            _fromTable = fromColumn.ParentTable.Name;
            _fromColumn = fromColumn.Name;

            _toTable = toColumn.ParentTable.Name;
            _toColumn = toColumn.Name;

            CrossFilteringBehavior = crossFiltering;
        }
        #endregion
        #region Private Properties for Serialization
        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = false)]
        private string _name;

        [DataMember(Name = "fromTable", IsRequired = true, EmitDefaultValue = false)]
        private string _fromTable;

        [DataMember(Name = "fromColumn", IsRequired = true, EmitDefaultValue = false)]
        private string _fromColumn;

        [DataMember(Name = "toTable", IsRequired = true, EmitDefaultValue = false)]
        private string _toTable;

        [DataMember(Name = "toColumn", IsRequired = true, EmitDefaultValue = false)]
        private string _toColumn;

        [DataMember(Name = "crossFilteringBehavior", IsRequired = true, EmitDefaultValue = false)]
        private string _crossFilteringBehavior;

        #endregion

        #region Public Properties
        public string Name
        {
            get => _name; set => _name = value;
        }

        public PBIColumn FromColumn
        {
            set
            {
                _fromTable = value.ParentTable.Name;
                _fromColumn = value.Name;
            }
        }

        public PBIColumn ToColumn
        {
            set
            {
                _toTable = value.ParentTable.Name;
                _toColumn = value.Name;
            }
        }

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

        public PBIDataset ParentDataset
        {
            get => _parentDataset;
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
